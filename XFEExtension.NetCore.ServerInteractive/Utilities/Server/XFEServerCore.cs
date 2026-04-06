using System.Diagnostics;
using System.Net;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心
/// </summary>
[CreateImpl]
public abstract class XFEServerCore : ServerCoreServiceBase
{
    /// <summary>
    /// 是否自动对Json字符串进行反转义处理，默认为true
    /// </summary>
    public bool AutoUnescapeJson { get; set; } = true;
    /// <summary>
    /// 是否接收非标准的Json字符串
    /// </summary>
    public bool AcceptNonStandardJson { get; set; } = true;
    /// <summary>
    /// 是否接收GET请求
    /// </summary>
    public bool AcceptGet { get; set; }
    /// <summary>
    /// 是否接收POST请求
    /// </summary>
    public bool AcceptPost { get; set; } = true;
    /// <summary>
    /// 服务器核心错误委托
    /// </summary>
    public XFEEventHandler<XFEServerCore, ServerCoreErrorEventArgs>? ServerCoreError { get; set; }
    /// <summary>
    /// 获取IP地址的函数，默认为从请求事件参数中获取客户端IP地址
    /// </summary>
    public Func<CyberCommRequestEventArgs, string> GetIPFunction { get; set; } = args => args.ClientIP;
    /// <summary>
    /// 主入口点路径（ServerCore主要入口点），默认为空字符串（为空时直接使用次级入口点）
    /// </summary>
    public string MainEntryPoint { get; set; } = string.Empty;
    /// <summary>
    /// 核心服务列表
    /// </summary>
    internal readonly List<IServerCoreOriginalService> ServerCoreServiceList = [];
    /// <summary>
    /// 核心校验服务列表
    /// </summary>
    internal List<Func<IServerCoreVerifyService>> ServerCoreVerifyServiceList = [];
    /// <summary>
    /// 核心标准服务工厂字典（按路由路径创建实例）
    /// </summary>
    internal Dictionary<string, Func<IServerCoreStandardService>> StandardCoreServiceDictionary = [];
    /// <summary>
    /// 通配符路由服务工厂列表（模式 → 工厂）
    /// </summary>
    internal List<(string Pattern, Func<IServerCoreStandardService> Factory)> WildcardCoreServiceList = [];
    /// <summary>
    /// 网络通讯服务器
    /// </summary>
    public CyberCommServer CyberCommServer { get; internal set; } = new();

    private async void CyberCommServer_RequestReceived(object? sender, CyberCommRequestEventArgs e)
    {
        var r = new ServerCoreReturnArgs
        {
            Args = e
        };
        try
        {
            var clientIP = e.ClientIP;
            try { clientIP = GetIPFunction(e); } catch (Exception ex) { Console.WriteLine($"[WARN]获取IP地址失败：{ex.Message}"); }
            r.ClientIP = clientIP;
            try
            {
                foreach (var serverCoreVerifyService in ServerCoreVerifyServiceList.Select(serverCoreVerifyFactory => serverCoreVerifyFactory()))
                {
                    serverCoreVerifyService.ReturnArgs = r;
                    serverCoreVerifyService.Args = e;
                    if (!serverCoreVerifyService.VerifyRequest() || !await serverCoreVerifyService.VerifyRequestAsync())
                        return;
                }
            }
            catch (Exception ex)
            {
                ServerCoreError?.Invoke(this, new()
                {
                    StatusCode = r.StatusCode,
                    Handled = r.Handled,
                    ReturnArgs = r,
                    ServerException = new ProcessStandardRequestException("请求校验失败", ex)
                });
                return;
            }

            // 从URL路径中提取路由信息
            QueryableJsonNode? queryableJsonNode = null;
            try
            {
                // 尝试解析请求体为JSON（可选）
                if (e.RequestBody is not null)
                {
                    queryableJsonNode = e.RequestBody;
                }
            }
            catch (Exception ex)
            {
                if (!AcceptNonStandardJson)
                {
                    ServerCoreError?.Invoke(this, new()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReturnArgs = r,
                        ServerException = new ProcessStandardRequestException("无法转换为QueryableJsonNode", ex)
                    });
                    return;
                }
            }

            try
            {
                // 从URL中提取路由：www.xxx.com/[mainEntryPoint?][/subEntryPoint?]*
                var url = e.Request.Url;
                if (url is null)
                {
                    ServerCoreError?.Invoke(this, new()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReturnArgs = r,
                        ServerException = new ProcessStandardRequestException("请求URL为空")
                    });
                    return;
                }

                var segments = url.Segments.Skip(1).Select(s => s.TrimEnd('/')).Where(s => !string.IsNullOrEmpty(s)).ToArray();

                // 如果MainEntryPoint不为空，则需要匹配主入口点
                string route;
                if (!string.IsNullOrEmpty(MainEntryPoint))
                {
                    if (segments.Length == 0 || segments[0] != MainEntryPoint)
                    {
                        ServerCoreError?.Invoke(this, new()
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            ReturnArgs = r,
                            ServerException = new ProcessStandardRequestException($"请求路径不匹配主入口点: {MainEntryPoint}")
                        });
                        return;
                    }
                    // 跳过主入口点，获取次级路由
                    route = string.Join("/", segments.Skip(1));
                }
                else
                {
                    // 直接使用所有segments作为路由
                    route = string.Join("/", segments);
                }

                if (route.IsNullOrEmpty())
                {
                    ServerCoreError?.Invoke(this, new()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        ReturnArgs = r,
                        ServerException = new ProcessStandardRequestException("请求路由为空")
                    });
                    return;
                }

                Console.Write($"({ServerCoreName})【{clientIP}】请求路由-{route}：");
                var stopWatch = Stopwatch.StartNew();

                // 在字典中查找对应的服务（先精确匹配，再通配符匹配）
                string? matchedPattern = null;

                if (StandardCoreServiceDictionary.TryGetValue(route, out var serviceFactory))
                {
                    matchedPattern = route;
                }
                else
                {
                    // 尝试通配符匹配：在所有命中的候选中选择最具体的模式（字面量段越多越优先），避免结果依赖注册顺序
                    static int GetWildcardPatternPriority(string pattern)
                    {
                        var segments = pattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
                        var literalSegmentCount = 0;
                        var wildcardSegmentCount = 0;
                        foreach (var segment in segments)
                        {
                            if (segment == "*")
                                wildcardSegmentCount++;
                            else
                                literalSegmentCount++;
                        }
                        return (literalSegmentCount * 1000) - (wildcardSegmentCount * 10) + pattern.Length;
                    }

                    var bestPriority = int.MinValue;
                    foreach (var (pattern, factory) in WildcardCoreServiceList)
                    {
                        if (!RouteMatchHelper.MatchWildcardRoute(pattern, route)) continue;
                        var currentPriority = GetWildcardPatternPriority(pattern);
                        if (currentPriority <= bestPriority) continue;
                        bestPriority = currentPriority;
                        matchedPattern = pattern;
                        serviceFactory = factory;
                    }
                }

                if (serviceFactory is not null && matchedPattern is not null)
                {
                    var serviceInstance = serviceFactory();
                    try
                    {
                        serviceInstance.XFEServerCore = this;
                        serviceInstance.Route = route;
                        serviceInstance.Json = queryableJsonNode;
                        serviceInstance.Args = e;
                        serviceInstance.ReturnArgs = r;
                        serviceInstance.Initialize();

                        // 根据匹配的模式调用对应的处理方法（同步与异步互斥，优先同步）
                        if (serviceInstance.SyncEntryPoints.TryGetValue(matchedPattern, out var syncHandler))
                            syncHandler();
                        else if (serviceInstance.AsyncEntryPoints.TryGetValue(matchedPattern, out var asyncHandler))
                            await asyncHandler();
                        else
                        {
                            ServerCoreError?.Invoke(this, new()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                ReturnArgs = r,
                                ServerException = new ExecutionUnregisteredException($"服务已注册但路由未找到对应处理方法-{route}")
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerCoreError?.Invoke(this, new()
                        {
                            StatusCode = r.StatusCode,
                            Handled = r.Handled,
                            ReturnArgs = r,
                            ServerException = new XFEServerCoreRequestInnerException($"请求异常-{route}", ex)
                        });
                    }
                    stopWatch.Stop();
                    Console.WriteLine($"\t[耗时 {InteractiveHelper.GetStopWatchTime(stopWatch)}]");
                    return;
                }

                ServerCoreError?.Invoke(this, new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReturnArgs = r,
                    ServerException = new ExecutionUnregisteredException($"请求的路由未注册-{route}")
                });
            }
            catch (Exception ex)
            {
                ServerCoreError?.Invoke(this, new()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReturnArgs = r,
                    ServerException = new ProcessStandardRequestException("处理标准请求时发生异常", ex)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]处理请求时发生未捕获的异常：{ex.Message}");
            Console.WriteLine($"[TRACE] {ex.StackTrace}");
            ServerCoreError?.Invoke(this, new()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ReturnArgs = r,
                ServerException = new ProcessStandardRequestException("处理请求时发生未捕获的异常", ex)
            });
        }
    }

    /// <summary>
    /// 启动核心服务
    /// </summary>
    /// <returns></returns>
    public override async Task StartServerCore()
    {
        CyberCommServer.ServerUrlArray = [.. BindingIPAddressList];
        CyberCommServer.RequestReceived += CyberCommServer_RequestReceived;
        await CyberCommServer.StartCyberCommServer();
    }
}
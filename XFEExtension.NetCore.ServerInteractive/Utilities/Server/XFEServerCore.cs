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
    static int serverCount = 1;
    /// <summary>
    /// 服务器核心错误委托
    /// </summary>
    public XFEEventHandler<XFEServerCore, ServerCoreErrorEventArgs>? ServerCoreError { get; set; }
    /// <summary>
    /// 获取IP地址的函数，默认为从请求事件参数中获取客户端IP地址
    /// </summary>
    public Func<CyberCommRequestEventArgs, string> GetIpFunction { get; set; } = args => args.ClientIP;
    /// <summary>
    /// 核心服务列表
    /// </summary>
    internal List<IServerCoreOriginalService> serverCoreServiceList = [];
    /// <summary>
    /// 核心校验服务列表
    /// </summary>
    internal List<IServerCoreVerifyService> serverCoreVerifyServiceList = [];
    /// <summary>
    /// 核心异步校验服务列表
    /// </summary>
    internal List<IServerCoreVerifyAsyncService> serverCoreVerifyAsyncServiceList = [];
    /// <summary>
    /// 核心标准服务工厂字典（按请求创建实例）
    /// </summary>
    internal Dictionary<string, Func<IServerCoreStandardService>> standardCoreServiceDictionary = [];
    /// <summary>
    /// 核心多重标准服务工厂字典（按请求创建实例）
    /// </summary>
    internal Dictionary<List<string>, Func<IServerCoreStandardService>> standardMultiCoreServiceDictionary = [];
    /// <summary>
    /// 网络通讯服务器
    /// </summary>
    public CyberCommServer CyberCommServer { get; internal set; } = new();

    /// <summary>
    /// XFE核心服务器
    /// </summary>
    public XFEServerCore() => ServerCoreName = $"XFE服务器-{serverCount++}";

    private async void CyberCommServer_RequestReceived(object? sender, CyberCommRequestEventArgs e)
    {
        var r = new ServerCoreReturnArgs
        {
            Args = e
        };
        var clientIP = e.ClientIP;
        try { clientIP = GetIpFunction(e); } catch (Exception ex) { Console.WriteLine($"[WARN]获取IP地址失败：{ex.Message}"); }
        r.ClientIP = clientIP;
        try
        {
            foreach (var serverCoreVerifyService in serverCoreVerifyServiceList)
                if (!serverCoreVerifyService.VerifyRequest(sender, e, r))
                    return;
            foreach (var serverCoreVerifyAsyncService in serverCoreVerifyAsyncServiceList)
                if (!await serverCoreVerifyAsyncService.VerifyRequestAsync(sender, e, r))
                    return;
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
        string execute = string.Empty;
        QueryableJsonNode? queryableJsonNode = null;
        try
        {
            queryableJsonNode = e.RequestBody ?? throw new ProcessStandardRequestException("请求的API接口不正确");
            execute = queryableJsonNode["execute"];
        }
        catch (Exception ex)
        {
            ServerCoreError?.Invoke(this, new()
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReturnArgs = r,
                ServerException = new ProcessStandardRequestException("无法转换为QueryableJsonNode", ex)
            });
            return;
        }
        try
        {
            if (queryableJsonNode is null)
                throw new ProcessStandardRequestException("QueryableJsonNode为空");
            if (!execute.IsNullOrEmpty())
            {
                //Console.WriteLine($"【{clientIP}】请求方法：{execute}");
                Console.Write($"【{clientIP}】请求方法-{execute}：");
                var stopWatch = Stopwatch.StartNew();
                if (standardCoreServiceDictionary.TryGetValue(execute, out var serviceFactory))
                {
                    var serviceInstance = serviceFactory();
                    // inject server core reference and set context
                    try
                    {
                        serviceInstance.XFEServerCore = this;
                        serviceInstance.Execute = execute;
                        serviceInstance.Json = queryableJsonNode;
                        serviceInstance.ReturnArgs = r;
                        serviceInstance.Initialize();
                        serviceInstance.RequestReceive();
                        await serviceInstance.RequestReceiveAsync();
                    }
                    catch (Exception ex)
                    {
                        ServerCoreError?.Invoke(this, new()
                        {
                            StatusCode = r.StatusCode,
                            Handled = r.Handled,
                            ReturnArgs = r,
                            ServerException = new XFEServerCoreRequestInnerException($"请求异常-{execute}", ex)
                        });
                    }
                    //Console.WriteLine($"【{clientIP}】请求处理完成：{execute}");
                    stopWatch.Stop();
                    Console.WriteLine($"\t[耗时 {InteractiveHelper.GetStopWatchTime(stopWatch)}]");
                    return;
                }
                foreach (var key in standardMultiCoreServiceDictionary.Keys)
                {
                    if (key.Contains(execute))
                    {
                        var factory = standardMultiCoreServiceDictionary[key];
                        var instance = factory();
                        try
                        {
                            instance.XFEServerCore = this;
                            instance.Execute = execute;
                            instance.Json = queryableJsonNode;
                            instance.ReturnArgs = r;
                            instance.Initialize();
                            instance.RequestReceive();
                            await instance.RequestReceiveAsync();
                        }
                        catch (Exception ex)
                        {
                            ServerCoreError?.Invoke(this, new()
                            {
                                StatusCode = r.StatusCode,
                                Handled = r.Handled,
                                ReturnArgs = r,
                                ServerException = new XFEServerCoreRequestInnerException($"请求异常-{execute}", ex)
                            });
                        }
                        stopWatch.Stop();
                        Console.WriteLine($"\t[耗时 {InteractiveHelper.GetStopWatchTime(stopWatch)}]");
                        return;
                    }
                }
                ServerCoreError?.Invoke(this, new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ReturnArgs = r,
                    ServerException = new ExecutionUnregisteredException($"请求的方法未注册-{execute}")
                });
            }
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

    /// <summary>
    /// 启动核心服务
    /// </summary>
    /// <returns></returns>
    public override async Task StartServerCore()
    {
        CyberCommServer.ServerURLs = [BindingIPAddress];
        CyberCommServer.RequestReceived += CyberCommServer_RequestReceived;
        await CyberCommServer.StartCyberCommServer();
    }
}
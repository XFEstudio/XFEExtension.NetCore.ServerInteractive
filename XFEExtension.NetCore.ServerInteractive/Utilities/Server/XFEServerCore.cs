using System.Net;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Implements.ServerService;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心
/// </summary>
[CreateImpl]
public abstract class XFEServerCore : CoreServerServiceBase
{
    static int serverCount = 1;
    /// <summary>
    /// 服务器核心错误委托
    /// </summary>
    public XFEEventHandler<XFEServerCore, ServerCoreErrorEventArgs>? ServerCoreError { get; set; }
    /// <summary>
    /// 核心服务列表
    /// </summary>
    public List<IServerCoreRegisterService> ServerCoreServiceList { get; internal set; } = [];
    /// <summary>
    /// 核心校验服务列表
    /// </summary>
    public List<IServerCoreVerifyService> ServerCoreVerifyServiceList { get; internal set; } = [];
    /// <summary>
    /// 核心异步校验服务列表
    /// </summary>
    public List<IServerCoreVerifyAsyncService> ServerCoreVerifyAsyncServiceList { get; internal set; } = [];
    /// <summary>
    /// 核心标准服务字典
    /// </summary>
    public Dictionary<string, IServerCoreStandardRegisterService> StandardCoreServiceDictionary { get; internal set; } = [];
    /// <summary>
    /// 核心标准服务字典
    /// </summary>
    public Dictionary<string, IServerCoreStandardRegisterAsyncService> StandardCoreAsyncServiceDictionary { get; internal set; } = [];
    /// <summary>
    /// 核心多重标准服务字典
    /// </summary>
    public Dictionary<List<string>, IServerCoreStandardRegisterService> StandardMultiCoreServiceDictionary { get; internal set; } = [];
    /// <summary>
    /// 核心多重标准服务字典
    /// </summary>
    public Dictionary<List<string>, IServerCoreStandardRegisterAsyncService> StandardMultiCoreAsyncServiceDictionary { get; internal set; } = [];
    /// <summary>
    /// 网络通讯服务器
    /// </summary>
    public CyberCommServer CyberCommServer { get; internal set; } = new();

    /// <summary>
    /// XFE核心服务器
    /// </summary>
    public XFEServerCore() => CoreServerName = $"XFE服务器-{serverCount++}";

    private async void CyberCommServer_RequestReceived(object? sender, CyberCommRequestEventArgs e)
    {
        var r = new ServerCoreReturnArgs
        {
            Args = e
        };
        try
        {
            foreach (var serverCoreVerifyService in ServerCoreVerifyServiceList)
                if (!serverCoreVerifyService.VerifyRequest(sender, e, r))
                    return;
            foreach (var serverCoreVerifyAsyncService in ServerCoreVerifyAsyncServiceList)
                if (!await serverCoreVerifyAsyncService.VerifyRequestAsync(sender, e, r))
                    return;
        }
        catch (Exception ex)
        {
            ServerCoreError?.Invoke(this, new()
            {
                StatusCode = r.StatusCode,
                Handled = r.Handled,
                CyberCommRequestEventArgs = e,
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
                CyberCommRequestEventArgs = e,
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
                Console.WriteLine($"【{e.ClientIP}】请求方法：{execute}");
                if (StandardCoreServiceDictionary.TryGetValue(execute, out var service))
                {
                    try
                    {
                        service.StandardRequestReceived(execute, queryableJsonNode, r);
                    }
                    catch (Exception ex)
                    {
                        ServerCoreError?.Invoke(this, new()
                        {
                            StatusCode = r.StatusCode,
                            Handled = r.Handled,
                            CyberCommRequestEventArgs = e,
                            ServerException = new XFEServerCoreRequestInnerException($"请求异常：{execute}", ex)
                        });
                    }
                    Console.WriteLine($"【{e.ClientIP}】请求处理完成：{execute}");
                    return;
                }
                else if (StandardCoreAsyncServiceDictionary.TryGetValue(execute, out var serviceAsync))
                {
                    try
                    {
                        await serviceAsync.StandardRequestReceived(execute, queryableJsonNode, r);
                    }
                    catch (Exception ex)
                    {
                        ServerCoreError?.Invoke(this, new()
                        {
                            StatusCode = r.StatusCode,
                            Handled = r.Handled,
                            CyberCommRequestEventArgs = e,
                            ServerException = new XFEServerCoreRequestInnerException($"请求异常：{execute}", ex)
                        });
                    }
                    Console.WriteLine($"【{e.ClientIP}】请求处理完成：{execute}");
                    return;
                }
                foreach (var key in StandardMultiCoreServiceDictionary.Keys)
                {
                    if (key.Contains(execute))
                    {
                        try
                        {
                            StandardMultiCoreServiceDictionary[key].StandardRequestReceived(execute, queryableJsonNode, r);
                        }
                        catch (Exception ex)
                        {
                            ServerCoreError?.Invoke(this, new()
                            {
                                StatusCode = r.StatusCode,
                                Handled = r.Handled,
                                CyberCommRequestEventArgs = e,
                                ServerException = new XFEServerCoreRequestInnerException($"请求异常：{execute}", ex)
                            });
                        }
                        Console.WriteLine($"【{e.ClientIP}】请求处理完成：{execute}");
                        return;
                    }
                }
                foreach (var key in StandardMultiCoreAsyncServiceDictionary.Keys)
                {
                    if (key.Contains(execute))
                    {
                        try
                        {
                            await StandardMultiCoreAsyncServiceDictionary[key].StandardRequestReceived(execute, queryableJsonNode, r);
                        }
                        catch (Exception ex)
                        {
                            ServerCoreError?.Invoke(this, new()
                            {
                                StatusCode = r.StatusCode,
                                Handled = r.Handled,
                                CyberCommRequestEventArgs = e,
                                ServerException = new XFEServerCoreRequestInnerException($"请求异常：{execute}", ex)
                            });
                        }
                        Console.WriteLine($"【{e.ClientIP}】请求处理完成：{execute}");
                        return;
                    }
                }
                ServerCoreError?.Invoke(this, new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    CyberCommRequestEventArgs = e,
                    ServerException = new ExecutionUnregisteredException($"请求的方法未注册：{execute}")
                });
            }
        }
        catch (Exception ex)
        {
            ServerCoreError?.Invoke(this, new()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                CyberCommRequestEventArgs = e,
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

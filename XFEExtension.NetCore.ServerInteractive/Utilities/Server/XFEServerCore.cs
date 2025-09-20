using System.Net;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerCoreService;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心
/// </summary>
[CreateImpl]
public abstract class XFEServerCore : IAsyncServerService
{
    /// <summary>
    /// 服务器核心错误
    /// </summary>
    public event XFEEventHandler<XFEServerCore, Exception>? ServerCoreError;
    /// <summary>
    /// 核心服务列表
    /// </summary>
    public List<IServerCoreService> ServerCoreServiceList { get; internal set; } = [];
    /// <summary>
    /// 核心标准服务字典
    /// </summary>
    public Dictionary<string, IServerStandardCoreService> StandardCoreServiceDictionary { get; internal set; } = [];
    /// <summary>
    /// 核心多重标准服务字典
    /// </summary>
    public Dictionary<List<string>, IServerStandardCoreService> StandardMultiCoreServiceDictionary { get; internal set; } = [];
    /// <summary>
    /// 网络通讯服务器
    /// </summary>
    public CyberCommServer CyberCommServer { get; internal set; } = new();
    /// <summary>
    /// XFE服务器
    /// </summary>
    public XFEServer XFEServer { get; set; } = new();

    private void CyberCommServer_RequestReceived(object? sender, CyberCommRequestEventArgs e)
    {
        string execute = string.Empty;
        QueryableJsonNode? queryableJsonNode = null;
        try
        {
            queryableJsonNode = e.RequestBody ?? throw new ProcessStandardRequestException("请求的API接口不正确");
            execute = queryableJsonNode["execute"];
        }
        catch (Exception ex)
        {
            ServerCoreError?.Invoke(this, new ProcessStandardRequestException("无法转换为QueryableJsonNode", ex));
        }
        try
        {
            if (!execute.IsNullOrEmpty())
            {
                if (StandardCoreServiceDictionary.TryGetValue(execute, out var service))
                {
                    service.StandardRequestReceived(sender, execute, queryableJsonNode, e);
                    return;
                }
                foreach (var key in StandardMultiCoreServiceDictionary.Keys)
                {
                    if (key.Contains(execute))
                    {
                        StandardMultiCoreServiceDictionary[key].StandardRequestReceived(sender, execute, queryableJsonNode, e);
                        return;
                    }
                }
                ServerCoreError?.Invoke(this, new ExecutionUnregisteredException($"请求的方法未注册：{execute}"));
            }
        }
        catch (Exception ex)
        {
            ServerCoreError?.Invoke(this, new ProcessStandardRequestException("处理标准请求时发生异常", ex));
        }
    }

    /// <summary>
    /// 启动核心服务
    /// </summary>
    /// <returns></returns>
    public async Task StartServiceAsync(string ipAddress = "http://*:8080/")
    {
        CyberCommServer = new(ipAddress);
        CyberCommServer.RequestReceived += CyberCommServer_RequestReceived;
        await CyberCommServer.StartCyberCommServer();
    }
}

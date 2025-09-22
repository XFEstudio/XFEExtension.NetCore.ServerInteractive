using System.Text.Json;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequester
{
    readonly JsonSerializerOptions jsonSerializerOptions = new();
    internal Dictionary<string, IRequestService> requestServiceDictionary = [];
    internal Dictionary<string, IXFERequestService> xFERequestServiceDictionary = [];
    /// <summary>
    /// 请求消息返回事件
    /// </summary>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;
    /// <summary>
    /// 表格请求器
    /// </summary>
    public TableRequester TableRequester { get; set; } = new();

    /// <summary>
    /// XFE客户端请求器
    /// </summary>
    public XFEClientRequester()
    {
        jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
        TableRequester.MessageReceived += TableRequester_MessageReceived;
    }

    private void TableRequester_MessageReceived(object? sender, ServerInteractiveEventArgs e) => MessageReceived?.Invoke(sender, e);

    public async Task<T> Request<T>(string serviceName, params object[] parameters)
    {
        if (requestServiceDictionary.TryGetValue(serviceName, out var service))
        {
            return await service.Request<T>();
        }
        else if (xFERequestServiceDictionary.TryGetValue(serviceName, out var xFEService))
        {

        }
        throw new XFERequesterException();
    }
}

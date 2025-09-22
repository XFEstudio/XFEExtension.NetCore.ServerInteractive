using System.Text.Json;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.DelegateExtension;
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
    /// <summary>
    /// 请求服务
    /// </summary>
    public List<IRequestService> RequestServices { get; set; } = [];
    /// <summary>
    /// XFE请求服务
    /// </summary>
    public List<IXFERequestService> XFERequestServices { get; set; } = [];
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
}

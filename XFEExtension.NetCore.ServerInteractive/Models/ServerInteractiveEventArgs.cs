using System.Net;
using XFEExtension.NetCore.AutoImplement;

namespace XFEExtension.NetCore.ServerInteractive.Models;

/// <summary>
/// 服务器交互事件参数
/// </summary>
/// <param name="message">消息内容</param>
/// <param name="httpStatusCode">状态码</param>
[CreateImpl]
public abstract class ServerInteractiveEventArgs(string? message, HttpStatusCode httpStatusCode) : EventArgs
{
    /// <summary>
    /// 消息
    /// </summary>
    public string? Message { get; set; } = message;
    /// <summary>
    /// 状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = httpStatusCode;
}

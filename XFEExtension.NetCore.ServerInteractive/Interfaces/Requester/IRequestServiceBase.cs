using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求器基础
/// </summary>
public interface IRequestServiceBase
{
    /// <summary>
    /// XFE客户端请求器
    /// </summary>
    XFEClientRequester XFEClientRequester { get; set; }
}
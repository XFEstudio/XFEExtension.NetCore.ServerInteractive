using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求器基础
/// </summary>
public interface IRequestServiceBase
{
    /// <summary>
    /// 获取请求地址
    /// </summary>
    Func<string> GetRequestAddress { get; set; }
    /// <summary>
    /// XFE客户端请求器
    /// </summary>
    XFEClientRequester XFEClientRequester { get; set; }
}
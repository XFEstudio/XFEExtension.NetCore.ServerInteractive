using System.Net;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;

/// <summary>
/// 请求结果基接口
/// </summary>
public interface IRequestResultBase
{
    /// <summary>
    /// 状态码
    /// </summary>
    HttpStatusCode StatusCode { get; set; }
    /// <summary>
    /// 消息
    /// </summary>
    string Message { get; set; }
    /// <summary>
    /// 结果
    /// </summary>
    object? Result { get; set; }
}

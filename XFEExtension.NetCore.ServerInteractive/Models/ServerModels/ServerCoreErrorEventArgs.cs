using System.Net;
using XFEExtension.NetCore.CyberComm;

namespace XFEExtension.NetCore.ServerInteractive.Models.ServerModels;

/// <summary>
/// 服务器核心错误事件参数
/// </summary>
public class ServerCoreErrorEventArgs
{
    /// <summary>
    /// 异常错误
    /// </summary>
    public Exception? ServerException { get; set; }
    /// <summary>
    /// 是否已经处理完成
    /// </summary>
    public bool Handled { get; set; } = false;
    /// <summary>
    /// 状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
    /// <summary>
    /// 本次异常服务器参数
    /// </summary>
    public CyberCommRequestEventArgs? CyberCommRequestEventArgs { get; set; }

    /// <summary>
    /// 服务器核心错误事件参数
    /// </summary>
    public ServerCoreErrorEventArgs() { }

    /// <summary>
    /// 服务器核心错误事件参数
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="httpStatusCode"></param>
    public ServerCoreErrorEventArgs(Exception exception, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
    {
        ServerException = exception;
        StatusCode = httpStatusCode;
    }

    /// <summary>
    /// 设置返回代码
    /// </summary>
    /// <param name="code"></param>
    public void SetCode(HttpStatusCode code = HttpStatusCode.InternalServerError)
    {
        StatusCode = code;
    }
}

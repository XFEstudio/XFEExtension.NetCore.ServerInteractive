using System.Net;
using XFEExtension.NetCore.CyberComm;

namespace XFEExtension.NetCore.ServerInteractive.Models.ServerModels;

/// <summary>
/// 返回码
/// </summary>
public class ServerCoreReturnArgs : Exception
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public string ReturnMessage { get; set; } = string.Empty;
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
    public required CyberCommRequestEventArgs Args { get; set; }

    /// <summary>
    /// 结束与客户端的通讯
    /// </summary>
    /// <param name="code"></param>
    public void Close(HttpStatusCode code = HttpStatusCode.OK)
    {
        StatusCode = code;
        Handled = true;
        Args.Close(StatusCode);
        throw this;
    }

    /// <summary>
    /// 以异常结束与客户端的通讯
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    public void Error(string message, HttpStatusCode code = HttpStatusCode.InternalServerError) => throw GetError(message, code);

    /// <summary>
    /// 以异常结束与客户端的通讯
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    /// <param name="handled"></param>
    public ServerCoreReturnArgs GetError(string message, HttpStatusCode code = HttpStatusCode.InternalServerError, bool handled = false)
    {
        ReturnMessage = message;
        Handled = handled;
        StatusCode = code;
        return this;
    }
}

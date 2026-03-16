using System.Net;
using System.Text.Json;
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
    /// 是否是标准错误（如果是标准错误，服务器将自动处理并返回给客户端；如果不是，服务器将输出报错及堆栈信息）
    /// </summary>
    public bool IsStandardError { get; set; }
    /// <summary>
    /// 客户端IP地址
    /// </summary>
    public string ClientIP { get; set; } = string.Empty;
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
    /// Sends a reply message asynchronously using the current context.
    /// </summary>
    /// <param name="message">The message text to send as a reply. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public async Task Send(string message) => await Args.ReplyMessage(message);

    /// <summary>
    /// Sends a binary message using the specified buffer asynchronously.
    /// </summary>
    /// <param name="buffer">The byte array containing the data to be sent as the binary message. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public async Task Send(byte[] buffer) => await Args.ReplyBinaryMessage(buffer);

    /// <summary>
    /// Sends the contents of the specified stream as a binary message asynchronously.
    /// </summary>
    /// <param name="stream">The stream containing the data to send. The stream must be readable and positioned at the beginning of the data
    /// to be sent.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public async Task Send(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        await Args.ReplyBinaryMessage(memoryStream.ToArray());
    }

    /// <summary>
    /// Sends an object serialized as JSON asynchronously using the current context.
    /// </summary>
    /// <param name="data">The object to serialize and send.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    public async Task Send(object data) => await Send(JsonSerializer.Serialize(data));

    /// <summary>
    /// 正常结束与客户端的通讯
    /// </summary>
    public void OK() => Args.Close();

    /// <summary>
    /// 结束与客户端的通讯
    /// </summary>
    /// <param name="message"></param>
    public async Task Close(string message) => await Args.ReplyAndClose(message);

    /// <summary>
    /// 结束与客户端的通讯
    /// </summary>
    /// <param name="buffer"></param>
    public async Task Close(byte[] buffer)
    {
        await Send(buffer);
        OK();
    }

    /// <summary>
    /// 结束与客户端的通讯
    /// </summary>
    /// <param name="stream"></param>
    public async Task Close(Stream stream)
    {
        await Send(stream);
        OK();
    }

    /// <summary>
    /// 结束与客户端的通讯（对象将自动序列化为 JSON）
    /// </summary>
    /// <param name="data"></param>
    public async Task Close(object data) => await Close(JsonSerializer.Serialize(data));

    /// <summary>
    /// 以异常结束与客户端的通讯
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    public async Task CloseWithError(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        ReturnMessage = message;
        Handled = true;
        StatusCode = code;
        await Args.ReplyAndClose(message, code);
    }

    /// <summary>
    /// 以异常结束与客户端的通讯
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    /// <param name="handled"></param>
    public ServerCoreReturnArgs Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest, bool handled = false)
    {
        ReturnMessage = message;
        Handled = handled;
        IsStandardError = true;
        StatusCode = code;
        return this;
    }
}

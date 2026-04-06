using System.Net;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// XFE服务器核心标准服务基类接口
/// </summary>
public interface IXFEStandardServerCoreServiceBase : IXFEServerCoreServiceBase
{
    /// <summary>
    /// 当前请求的路由路径
    /// </summary>
    string Route { get; set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    string ClientIP { get; set; }

    /// <summary>
    /// 是否是标准错误（如果是标准错误，服务器将自动处理并返回给客户端；如果不是，服务器将输出报错及堆栈信息）
    /// </summary>
    bool IsStandardError { get; set; }

    /// <summary>
    /// 错误抛出是否已经处理完成（如果为true，表示错误已经被处理，不需要ErrorProcessor再次处理）
    /// </summary>
    bool Handled { get; set; }

    /// <summary>
    /// 当前请求的 json 节点
    /// </summary>
    QueryableJsonNode? Json { get; set; }

    /// <summary>
    /// 当前请求的返回参数
    /// </summary>
    ServerCoreReturnArgs ReturnArgs { get; set; }

    /// <summary>
    /// 当前请求的事件参数
    /// </summary>
    CyberCommRequestEventArgs Args { get; set; }

    /// <summary>
    /// 当前请求对象。
    /// </summary>
    [Obsolete("Use Args.Request instead. This compatibility shim will be removed in a future release.")]
    HttpListenerRequest? Request => Args?.Request;

    /// <summary>
    /// Sends a reply message asynchronously using the current context.
    /// </summary>
    /// <param name="message">The message text to send as a reply. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task Send(string message);

    /// <summary>
    /// Sends a binary message using the specified buffer asynchronously.
    /// </summary>
    /// <param name="buffer">The byte array containing the data to be sent as the binary message. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task Send(byte[] buffer);

    /// <summary>
    /// Sends the contents of the specified stream as a binary message asynchronously.
    /// </summary>
    /// <param name="stream">The stream containing the data to send. The stream must be readable and positioned at the beginning of the data
    /// to be sent.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task Send(Stream stream);

    /// <summary>
    /// Sends an object as JSON asynchronously.
    /// </summary>
    /// <param name="data">The object to serialize to JSON and send.</param>
    Task Send(object data);

    /// <summary>
    /// 关闭并返回 OK
    /// </summary>
    void OK();

    /// <summary>
    /// 返回信息并关闭
    /// </summary>
    /// <param name="message">要返回的信息</param>
    Task Close(string message);

    /// <summary>
    /// 结束与客户端的通讯
    /// </summary>
    /// <param name="buffer"></param>
    Task Close(byte[] buffer);

    /// <summary>
    /// 结束与客户端的通讯
    /// </summary>
    /// <param name="stream"></param>
    Task Close(Stream stream);

    /// <summary>
    /// 返回错误信息并关闭
    /// </summary>
    /// <param name="message">要返回的错误信息</param>
    /// <param name="code">HTTP 状态码</param>
    Task CloseWithError(string message, HttpStatusCode code = HttpStatusCode.BadRequest);

    /// <summary>
    /// 以异常结束与客户端的通讯
    /// </summary>
    /// <param name="message"></param>
    /// <param name="code"></param>
    /// <param name="handled"></param>
    ServerCoreReturnArgs Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest, bool handled = false);

    /// <summary>
    /// Sends the specified object serialized as JSON and closes the connection.
    /// </summary>
    /// <param name="data">The object to serialize to JSON and send.</param>
    Task Close(object data);
}
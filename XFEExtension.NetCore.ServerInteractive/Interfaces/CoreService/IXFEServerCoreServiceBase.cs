using System.Net;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;

/// <summary>
/// XFE服务器核心服务
/// </summary>
public interface IXFEServerCoreServiceBase
{
    /// <summary>
    /// 服务器核心
    /// </summary>
    XFEServerCore XFEServerCore { get; set; }

    /// <summary>
    /// 当前请求的执行语句
    /// </summary>
    string Execute { get; set; }

    /// <summary>
    /// 当前请求的 json 节点
    /// </summary>
    QueryableJsonNode Json { get; set; }

    /// <summary>
    /// 当前请求的返回参数
    /// </summary>
    ServerCoreReturnArgs ReturnArgs { get; set; }

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
}
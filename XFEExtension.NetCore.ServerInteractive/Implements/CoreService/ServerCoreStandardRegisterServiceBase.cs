using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

/// <summary>
/// 服务器标准核心服务基类
/// </summary>
public abstract class ServerCoreStandardRegisterServiceBase : IServerCoreStandardRegisterService
{
    /// <inheritdoc/>
    public XFEServerCore XFEServerCore { get; set; } 
    /// <summary>
    /// 当前请求的执行语句
    /// </summary>
    public string Execute { get; set; } = string.Empty;
    /// <summary>
    /// 当前请求的 json 节点
    /// </summary>
    public QueryableJsonNode? QueryableJsonNode { get; set; }
    /// <summary>
    /// 当前请求的返回参数
    /// </summary>
    public ServerCoreReturnArgs? ReturnArgs { get; set; }

    /// <summary>
    /// 处理请求（无参，子类直接使用属性）
    /// </summary>
    public abstract void StandardRequestReceived();

    /// <summary>
    /// 关闭并返回 OK
    /// </summary>
    public void OK()
    {
        ReturnArgs?.Close(System.Net.HttpStatusCode.OK);
    }

    /// <summary>
    /// 返回错误信息并关闭
    /// </summary>
    public void Close(string message, System.Net.HttpStatusCode code = System.Net.HttpStatusCode.InternalServerError)
    {
        if (ReturnArgs is null)
            return;
        ReturnArgs.Error(message, code);
    }
}

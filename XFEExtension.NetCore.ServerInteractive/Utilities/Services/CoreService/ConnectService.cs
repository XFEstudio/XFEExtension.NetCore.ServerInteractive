using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

/// <summary>
/// 连接校验服务
/// </summary>
public class ConnectService : ServerCoreStandardRegisterAsyncServiceBase
{
    /// <inheritdoc/>
    public override async Task StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r) => await r.Args.ReplyAndClose(DateTime.Now.ToString());
}

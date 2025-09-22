using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// XFE数据表格管理器服务
/// </summary>
public class XFEDataTableManagerService : ServerCoreStandardRegisterAsyncServiceBase
{
    /// <summary>
    /// 数据表格管理器
    /// </summary>
    public XFEDataTableManager TableManager { get; set; }

    /// <inheritdoc/>
    public override async Task StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r) => await TableManager.Execute(execute, queryableJsonNode, r);
}
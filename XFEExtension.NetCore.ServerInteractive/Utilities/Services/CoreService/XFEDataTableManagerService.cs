using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

/// <summary>
/// XFE数据表格管理器服务
/// </summary>
public class XFEDataTableManagerService : ServerCoreStandardRegisterServiceBase
{
    /// <summary>
    /// 数据表格管理器
    /// </summary>
    public XFEDataTableManager TableManager { get; set; } = new XFEDataTableManagerImpl();

    /// <inheritdoc/>
    public override async void StandardRequestReceived(object? sender, string execute, QueryableJsonNode queryableJsonNode, CyberCommRequestEventArgs e) => await TableManager.Execute(execute, queryableJsonNode, e);
}
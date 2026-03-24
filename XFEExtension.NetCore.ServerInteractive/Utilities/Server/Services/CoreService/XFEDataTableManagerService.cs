using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// XFE数据表格管理器服务
/// </summary>
public class XFEDataTableManagerService : ServerCoreStandardServiceBase
{
    /// <summary>
    /// 数据表格管理器
    /// </summary>
    public XFEDataTableManager TableManager { get; set; }

    /// <inheritdoc/>
    public override async Task RequestReceiveAsync() => await TableManager.Execute(Execute, Json ?? new(new()), ReturnArgs);
}
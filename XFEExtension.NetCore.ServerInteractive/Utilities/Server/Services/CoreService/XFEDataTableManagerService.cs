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
    public override void Initialize()
    {
        // Dynamically populate the async entry point for the current route
        // This works because a new service instance is created for each request
        AsyncEntryPoints[Route] = HandleOperation;
    }

    /// <summary>
    /// 处理数据表格操作（由XFEServerCore通过字典调用）
    /// </summary>
    private async Task HandleOperation()
    {
        // Convert route from "table/{operation}/{tableName}" to "{operation}_{tableName}"
        var routeParts = Route.Split('/');
        if (routeParts.Length >= 3)
        {
            var execute = $"{routeParts[1]}_{routeParts[2]}";
            await TableManager.Execute(execute, Json ?? new(new()), ReturnArgs);
        }
        else
        {
            throw Error("Invalid table operation route format. Expected: table/{operation}/{tableName}");
        }
    }
}
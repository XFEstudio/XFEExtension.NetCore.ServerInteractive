using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities;

/// <summary>
/// XFE服务器核心构建器扩展
/// </summary>
public static class XFEServerCoreBuilderExtensions
{
    /// <summary>
    /// 使用XFE数据表格管理器
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="xFEDataTableManagerBuilder">数据表格构建器</param>
    /// <returns></returns>
    public static XFEServerCoreBuilder UseDataTableManager(this XFEServerCoreBuilder xFEServerCoreBuilder, XFEDataTableManagerBuilder xFEDataTableManagerBuilder)
    {
        xFEServerCoreBuilder.RegisterStandardService<XFEDataTableManagerService>(xFEDataTableManagerBuilder.ExecuteList);
        return xFEServerCoreBuilder;
    }
}
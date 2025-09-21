using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
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
    public static XFEServerCoreBuilder AddDataTableManager(this XFEServerCoreBuilder xFEServerCoreBuilder, XFEDataTableManagerBuilder xFEDataTableManagerBuilder)
    {
        xFEServerCoreBuilder.RegisterStandardAsyncService<XFEDataTableManagerService>(xFEDataTableManagerBuilder.ExecuteList);
        return xFEServerCoreBuilder;
    }

    /// <summary>
    /// 添加用户基本参数
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="getUserFunction"></param>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="addEncryptedUserLoginModelFunction"></param>
    /// <param name="removeEncryptedUserLoginModelFunction"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddUserParameterBase(this XFEServerCoreBuilder xFEServerCoreBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction)
    {
        xFEServerCoreBuilder.AddParameter("GetUserFunction", getUserFunction);
        xFEServerCoreBuilder.AddParameter("GetEncryptedUserLoginModelFunction", getEncryptedUserLoginModelFunction);
        xFEServerCoreBuilder.AddParameter("AddEncryptedUserLoginModelFunction", addEncryptedUserLoginModelFunction);
        xFEServerCoreBuilder.AddParameter("RemoveEncryptedUserLoginModelFunction", removeEncryptedUserLoginModelFunction);
        return xFEServerCoreBuilder;
    }

    /// <summary>
    /// 使用XFE标准登录服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="getUserFunction">获取用户列表方法</param>
    /// <param name="getEncryptedUserLoginModelFunction">获取加密用户模型方法</param>
    /// <param name="addEncryptedUserLoginModelFunction">添加加密用户模型方法</param>
    /// <param name="removeEncryptedUserLoginModelFunction">移除加密用户模型方法</param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddStandardLoginServer(this XFEServerCoreBuilder xFEServerCoreBuilder)
    {
        xFEServerCoreBuilder.RegisterStandardAsyncService<UserLoginService>("login");
        xFEServerCoreBuilder.RegisterStandardAsyncService<UserReloginService>("relogin");
        xFEServerCoreBuilder.AddService<UserLoginAutoCleanService>();
        return xFEServerCoreBuilder;
    }

    /// <summary>
    /// 添加IP封禁服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddIpBannerService(this XFEServerCoreBuilder xFEServerCoreBuilder)
    {
        xFEServerCoreBuilder.RegisterStandardAsyncService<IpBannerService>(["get_bannedIpList", "add_bannedIp", "remove_bannedIp"]);
        return xFEServerCoreBuilder;
    }
}
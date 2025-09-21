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
    /// <param name="getUserFunction"></param>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddDataTableManager(this XFEServerCoreBuilder xFEServerCoreBuilder, XFEDataTableManagerBuilder xFEDataTableManagerBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction) => xFEServerCoreBuilder.AddParameter("TableManager", xFEDataTableManagerBuilder.Build(getUserFunction, getEncryptedUserLoginModelFunction)).RegisterStandardAsyncService<XFEDataTableManagerService>(xFEDataTableManagerBuilder.ExecuteList);

    /// <summary>
    /// 添加用户基本参数
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="getUserFunction"></param>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="addEncryptedUserLoginModelFunction"></param>
    /// <param name="removeEncryptedUserLoginModelFunction"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddUserParameterBase(this XFEServerCoreBuilder xFEServerCoreBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction) => xFEServerCoreBuilder.AddParameter("GetUserFunction", getUserFunction)
                            .AddParameter("GetEncryptedUserLoginModelFunction", getEncryptedUserLoginModelFunction)
                            .AddParameter("AddEncryptedUserLoginModelFunction", addEncryptedUserLoginModelFunction)
                            .AddParameter("RemoveEncryptedUserLoginModelFunction", removeEncryptedUserLoginModelFunction);

    /// <summary>
    /// 使用XFE标准登录服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddStandardLoginService(this XFEServerCoreBuilder xFEServerCoreBuilder) => xFEServerCoreBuilder.RegisterStandardAsyncService<UserLoginService>("login")
            .RegisterStandardAsyncService<UserReloginService>("relogin")
            .AddService<UserLoginAutoCleanService>();

    /// <summary>
    /// 添加IP封禁服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddIpBannerService(this XFEServerCoreBuilder xFEServerCoreBuilder) => xFEServerCoreBuilder.RegisterStandardAsyncService<IpBannerService>(["get_bannedIpList", "add_bannedIp", "remove_bannedIp"]);

    /// <summary>
    /// 添加日期统计服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddDailyCounterService(this XFEServerCoreBuilder xFEServerCoreBuilder) => xFEServerCoreBuilder.AddService<DailyCounterService>();

    /// <summary>
    /// 添加XFE异常处理服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddXFEErrorProcessService(this XFEServerCoreBuilder xFEServerCoreBuilder) => xFEServerCoreBuilder.AddService<CoreServerExceptionProcessService>();

    /// <summary>
    /// 添加连接检查服务
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddConnectService(this XFEServerCoreBuilder xFEServerCoreBuilder) => xFEServerCoreBuilder.RegisterStandardAsyncService<ConnectService>("check_connect");

    /// <summary>
    /// 添加服务器入口点校验
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddEntryPotinVerify(this XFEServerCoreBuilder xFEServerCoreBuilder) => xFEServerCoreBuilder.AddVerifyService<EntryPointVerifyServer>();

    /// <summary>
    /// 使用XFE标准服务器核心
    /// </summary>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="getUserFunction"></param>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="addEncryptedUserLoginModelFunction"></param>
    /// <param name="removeEncryptedUserLoginModelFunction"></param>
    /// <param name="xFEDataTableManagerBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder UseXFEStandardServerCore(this XFEServerCoreBuilder xFEServerCoreBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction, XFEDataTableManagerBuilder xFEDataTableManagerBuilder) => xFEServerCoreBuilder.AddUserParameterBase(getUserFunction, getEncryptedUserLoginModelFunction, addEncryptedUserLoginModelFunction, removeEncryptedUserLoginModelFunction)
            .AddDataTableManager(xFEDataTableManagerBuilder, getUserFunction, getEncryptedUserLoginModelFunction)
            .AddEntryPotinVerify()
            .AddDailyCounterService()
            .AddXFEErrorProcessService()
            .AddConnectService()
            .AddStandardLoginService()
            .AddIpBannerService();
}
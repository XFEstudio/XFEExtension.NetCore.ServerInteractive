using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

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
    /// <typeparam name="T">登录返回用户接口类型</typeparam>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="getUserFunction"></param>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="addEncryptedUserLoginModelFunction"></param>
    /// <param name="removeEncryptedUserLoginModelFunction"></param>
    /// <param name="getLoginKeepDays"></param>
    /// <param name="loginResultConvertFunction"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddUserParameterBase<T>(this XFEServerCoreBuilder xFEServerCoreBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction, Func<int> getLoginKeepDays, Func<object, T> loginResultConvertFunction) where T : class => xFEServerCoreBuilder.AddParameter("GetUserFunction", getUserFunction)
                            .AddParameter("GetEncryptedUserLoginModelFunction", getEncryptedUserLoginModelFunction)
                            .AddParameter("AddEncryptedUserLoginModelFunction", addEncryptedUserLoginModelFunction)
                            .AddParameter("RemoveEncryptedUserLoginModelFunction", removeEncryptedUserLoginModelFunction)
                            .AddParameter("GetLoginKeepDays", getLoginKeepDays)
                            .AddParameter("LoginResultConvertFunction", loginResultConvertFunction);

    /// <summary>
    /// 使用XFE标准登录服务
    /// </summary>
    /// <typeparam name="T">登录返回用户接口类型</typeparam>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder AddStandardLoginService<T>(this XFEServerCoreBuilder xFEServerCoreBuilder) where T : class => xFEServerCoreBuilder.RegisterStandardAsyncService<UserLoginService<T>>("login")
            .RegisterStandardAsyncService<UserReloginService<T>>("relogin")
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
    /// <typeparam name="T">登录返回用户接口类型</typeparam>
    /// <param name="xFEServerCoreBuilder"></param>
    /// <param name="getUserFunction"></param>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="addEncryptedUserLoginModelFunction"></param>
    /// <param name="removeEncryptedUserLoginModelFunction"></param>
    /// <param name="getLoginKeepDays"></param>
    /// <param name="xFEDataTableManagerBuilder"></param>
    /// <param name="loginResultConvertFunction"></param>
    /// <returns></returns>
    public static XFEServerCoreBuilder UseXFEStandardServerCore<T>(this XFEServerCoreBuilder xFEServerCoreBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction, Func<int> getLoginKeepDays, XFEDataTableManagerBuilder xFEDataTableManagerBuilder, Func<object, T> loginResultConvertFunction) where T : class => xFEServerCoreBuilder.AddUserParameterBase(getUserFunction, getEncryptedUserLoginModelFunction, addEncryptedUserLoginModelFunction, removeEncryptedUserLoginModelFunction, getLoginKeepDays, loginResultConvertFunction)
            .AddDataTableManager(xFEDataTableManagerBuilder, getUserFunction, getEncryptedUserLoginModelFunction)
            .AddEntryPotinVerify()
            .AddDailyCounterService()
            .AddXFEErrorProcessService()
            .AddConnectService()
            .AddStandardLoginService<T>()
            .AddIpBannerService();
}
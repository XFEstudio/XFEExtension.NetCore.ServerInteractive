using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Options;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;

/// <summary>
/// XFE服务器核心构建器扩展
/// </summary>
public static class XFEServerCoreBuilderExtensions
{
    /// <param name="xFEServerCoreBuilder"></param>
    extension(XFEServerCoreBuilder xFEServerCoreBuilder)
    {
        /// <summary>
        /// 使用XFE数据表格管理器
        /// </summary>
        /// <param name="xFEDataTableManagerBuilder">数据表格构建器</param>
        /// <param name="getUserFunction"></param>
        /// <param name="getEncryptedUserLoginModelFunction"></param>
        /// <returns></returns>
        public XFEServerCoreBuilder AddDataTableManager(XFEDataTableManagerBuilder xFEDataTableManagerBuilder, Func<IEnumerable<User>> getUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction)
        {
            xFEServerCoreBuilder.AddParameter("TableManager", xFEDataTableManagerBuilder.Build(getUserFunction, getEncryptedUserLoginModelFunction));

            // Register the service for each table operation route
            // XFEDataTableManagerService uses dynamic routing, so we use AddServiceWithRoute
            foreach (var execute in xFEDataTableManagerBuilder.ExecuteList)
            {
                var parts = execute.Split('_', 2);
                if (parts.Length != 2)
                    throw new InvalidOperationException($"执行语句格式无效：'{execute}'，期望格式为 '{{operation}}_{{tableName}}'");
                var route = $"table/{parts[0]}/{parts[1]}";
                xFEServerCoreBuilder.AddServiceWithRoute<XFEDataTableManagerService>(route);
            }

            return xFEServerCoreBuilder;
        }

        /// <summary>
        /// 添加用户基本参数
        /// </summary>
        /// <typeparam name="T">登录返回用户接口类型</typeparam>
        /// <param name="getUserFunction"></param>
        /// <param name="addUserFunction"></param>
        /// <param name="getEncryptedUserLoginModelFunction"></param>
        /// <param name="addEncryptedUserLoginModelFunction"></param>
        /// <param name="removeEncryptedUserLoginModelFunction"></param>
        /// <param name="getLoginKeepDays"></param>
        /// <param name="loginResultConvertFunction"></param>
        /// <returns></returns>
        public XFEServerCoreBuilder AddUserParameterBase<T>(Func<IEnumerable<User>> getUserFunction, Action<User> addUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction, Func<int> getLoginKeepDays, Func<object, T> loginResultConvertFunction) where T : class => xFEServerCoreBuilder.AddParameter("GetUserFunction", getUserFunction)
            .AddParameter("AddUserFunction", addUserFunction)
            .AddParameter("GetEncryptedUserLoginModelFunction", getEncryptedUserLoginModelFunction)
            .AddParameter("AddEncryptedUserLoginModelFunction", addEncryptedUserLoginModelFunction)
            .AddParameter("RemoveEncryptedUserLoginModelFunction", removeEncryptedUserLoginModelFunction)
            .AddParameter("GetLoginKeepDays", getLoginKeepDays)
            .AddParameter("LoginResultConvertFunction", loginResultConvertFunction);

        /// <summary>
        /// 使用XFE标准登录服务
        /// </summary>
        /// <typeparam name="T">登录返回用户接口类型</typeparam>
        /// <returns></returns>
        public XFEServerCoreBuilder AddStandardLoginService<T>() where T : class => xFEServerCoreBuilder.AddService<UserLoginService<T>>()
            .AddService<UserReloginService<T>>()
            .AddOriginalService<UserLoginAutoCleanService>();

        /// <summary>
        /// 添加IP封禁服务
        /// </summary>
        /// <returns></returns>
        public XFEServerCoreBuilder AddIPBannerService() => xFEServerCoreBuilder.AddService<IPBannerService>();

        /// <summary>
        /// 添加日期统计服务
        /// </summary>
        /// <returns></returns>
        public XFEServerCoreBuilder AddDailyCounterService() => xFEServerCoreBuilder.AddOriginalService<DailyCounterService>();

        /// <summary>
        /// 添加XFE异常处理服务
        /// </summary>
        /// <returns></returns>
        public XFEServerCoreBuilder AddXFEErrorProcessService() => xFEServerCoreBuilder.AddOriginalService<ServerCoreExceptionProcessService>();

        /// <summary>
        /// 添加连接检查服务
        /// </summary>
        /// <returns></returns>
        public XFEServerCoreBuilder AddConnectService() => xFEServerCoreBuilder.AddService<ConnectService>();

        /// <summary>
        /// 添加服务器入口点校验
        /// </summary>
        /// <returns></returns>
        public XFEServerCoreBuilder AddEntryPointVerify() => xFEServerCoreBuilder.AddVerifyService<EntryPointVerifyService>();

        /// <summary>
        /// 添加服务器日志请求
        /// </summary>
        /// <returns></returns>
        public XFEServerCoreBuilder AddServerLogService() => xFEServerCoreBuilder.AddService<CoreLogService>();

        /// <summary>
        /// 使用XFE标准服务器核心
        /// </summary>
        /// <typeparam name="T">登录返回用户接口类型</typeparam>
        /// <param name="getUserFunction"></param>
        /// <param name="addUserFunction"></param>
        /// <param name="getEncryptedUserLoginModelFunction"></param>
        /// <param name="addEncryptedUserLoginModelFunction"></param>
        /// <param name="removeEncryptedUserLoginModelFunction"></param>
        /// <param name="getLoginKeepDays"></param>
        /// <param name="xFEDataTableManagerBuilder"></param>
        /// <param name="loginResultConvertFunction"></param>
        /// <returns></returns>
        public XFEServerCoreBuilder UseXFEStandardServerCore<T>(Func<IEnumerable<User>> getUserFunction, Action<User> addUserFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> addEncryptedUserLoginModelFunction, Action<EncryptedUserLoginModel> removeEncryptedUserLoginModelFunction, Func<int> getLoginKeepDays, Func<object, T> loginResultConvertFunction, XFEDataTableManagerBuilder xFEDataTableManagerBuilder) where T : class => xFEServerCoreBuilder.AddUserParameterBase(getUserFunction, addUserFunction, getEncryptedUserLoginModelFunction, addEncryptedUserLoginModelFunction, removeEncryptedUserLoginModelFunction, getLoginKeepDays, loginResultConvertFunction)
            .AddDataTableManager(xFEDataTableManagerBuilder, getUserFunction, getEncryptedUserLoginModelFunction)
            .AddEntryPointVerify()
            .AddDailyCounterService()
            .AddXFEErrorProcessService()
            .AddConnectService()
            .AddStandardLoginService<T>()
            .AddServerLogService()
            .AddIPBannerService();

        /// <summary>
        /// 使用XFE标准服务器核心（Options）
        /// </summary>
        public XFEServerCoreBuilder UseXFEStandardServerCore<T>(XFEStandardServerCoreOptions<T>? options) where T : class
        {
            if (options is null) return xFEServerCoreBuilder;

            // start with base builder
            var builder = xFEServerCoreBuilder;

            // If user-related functions are provided, add user parameters and login services
            var hasUserFunctions = options.GetUserFunction is not null && options.AddUserFunction is not null && options.GetEncryptedUserLoginModelFunction is not null && options.AddEncryptedUserLoginModelFunction is not null && options.RemoveEncryptedUserLoginModelFunction is not null && options.LoginResultConvertFunction is not null;

            if (hasUserFunctions)
            {
                builder = builder.AddUserParameterBase(options.GetUserFunction!, options.AddUserFunction!, options.GetEncryptedUserLoginModelFunction!, options.AddEncryptedUserLoginModelFunction!, options.RemoveEncryptedUserLoginModelFunction!, options.GetLoginKeepDays, options.LoginResultConvertFunction!);
            }

            // DataTableManager requires both user functions and a builder
            if (options.DataTableManagerBuilder is not null && options.GetUserFunction is not null && options.GetEncryptedUserLoginModelFunction is not null)
            {
                builder = builder.AddDataTableManager(options.DataTableManagerBuilder, options.GetUserFunction, options.GetEncryptedUserLoginModelFunction);
            }

            // Common services that don't strictly require user functions
            if (options.UseEntryPointVerifyService)
            {
                builder = builder.AddEntryPointVerify();
            }
            if (options.UseDailyCounterService)
            {
                builder = builder.AddDailyCounterService();
            }
            if (options.UseXFEErrorProcessService)
            {
                builder = builder.AddXFEErrorProcessService();
            }
            if (options.UseConnectService)
            {
                builder = builder.AddConnectService();
            }
            // Add services that depend on user functions if available
            if (hasUserFunctions)
            {
                builder = builder.AddStandardLoginService<T>().AddServerLogService().AddIPBannerService();
            }

            return builder;
        }

        /// <summary>
        /// 使用 XFE 标准服务器核心，提供 options lambda 初始化器
        /// </summary>
        public XFEServerCoreBuilder UseXFEStandardServerCore<T>(Action<XFEStandardServerCoreOptions<T>> optionsBuilder) where T : class
        {
            var options = new XFEStandardServerCoreOptions<T>();
            optionsBuilder.Invoke(options);
            return xFEServerCoreBuilder.UseXFEStandardServerCore(options);
        }
    }
}

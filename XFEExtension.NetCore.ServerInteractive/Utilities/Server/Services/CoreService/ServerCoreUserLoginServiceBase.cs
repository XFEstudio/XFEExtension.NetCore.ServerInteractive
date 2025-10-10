namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 服务器核心用户登录服务基类
/// </summary>
/// <typeparam name="T">登录返回类型</typeparam>
public abstract class ServerCoreUserLoginServiceBase<T> : ServerCoreUserServiceBase where T : class
{
    /// <summary>
    /// 用户登录结果转换函数
    /// </summary>
    public Func<object, T> LoginResultConvertFunction { get; set; } = user => (T)user;
}

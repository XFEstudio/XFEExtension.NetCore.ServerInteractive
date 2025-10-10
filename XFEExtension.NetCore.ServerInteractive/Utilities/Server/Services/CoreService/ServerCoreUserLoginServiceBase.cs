using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

///<inheritdoc/>
public abstract class ServerCoreUserLoginServiceBase<T, F> : ServerCoreUserServiceBase<T> where T : IUserInfo where F : class
{
    /// <summary>
    /// 登录结果转换方法
    /// </summary>
    public Func<T, F> LoginResultConverter { get; set; } = static user => (F)(object)user;
}

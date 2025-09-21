namespace XFEExtension.NetCore.ServerInteractive.Interfaces;

/// <summary>
/// 用户信息
/// </summary>
public interface IUserInfo : IUserFaceInfo
{
    /// <summary>
    /// 用户名，登录使用
    /// </summary>
    string UserName { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    string Password { get; set; }
    /// <summary>
    /// 用户是否启用
    /// </summary>
    bool Enable { get; set; }
}

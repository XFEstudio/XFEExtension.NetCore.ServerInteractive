using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Models.UserModels;

/// <summary>
/// 用户信息模型
/// </summary>
public class User : IUserInfo
{
    public string ID { get; set; } = Guid.NewGuid().ToString("N");
    public string NickName { get; set; } = string.Empty;
    public int PermissionLevel { get; set; } = 0;
    /// <summary>
    /// 用户名，登录使用
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// 用户是否启用
    /// </summary>
    public bool Enable { get; set; } = true;
}
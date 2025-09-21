using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Models.UserModels;

/// <summary>
/// 用户标准模型
/// </summary>
public class User : IUserInfo
{
    /// <inheritdoc/>
    public string ID { get; set; } = Guid.NewGuid().ToString("N");
    /// <inheritdoc/>
    public string NickName { get; set; } = string.Empty;
    /// <inheritdoc/>
    public int PermissionLevel { get; set; } = 0;
    /// <inheritdoc/>
    public string UserName { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string Password { get; set; } = string.Empty;
    /// <inheritdoc/>
    public bool Enable { get; set; } = true;
}
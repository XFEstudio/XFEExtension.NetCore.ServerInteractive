using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Models;

/// <summary>
/// 用户接口信息
/// </summary>
public class UserFaceInfo : IUserFaceInfo
{
    /// <inheritdoc/>
    public string ID { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string NickName { get; set; } = string.Empty;
    /// <inheritdoc/>
    public int PermissionLevel { get; set; }
}

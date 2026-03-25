using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces;

/// <summary>
/// 用户信息
/// </summary>
public interface IUserFaceInfo : IIdModel
{
    /// <summary>
    /// 用户ID，唯一标识符
    /// </summary>
    new string Id { get; set; }
    /// <summary>
    /// 昵称，显示使用
    /// </summary>
    string NickName { get; set; }
    /// <summary>
    /// 用户权限等级
    /// </summary>
    int PermissionLevel { get; set; }
}

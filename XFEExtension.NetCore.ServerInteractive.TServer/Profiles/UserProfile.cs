using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Profiles;

public partial class UserProfile : XFEProfile
{
    /// <summary>
    /// 用户列表
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current._userTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._userTable")]
    private ProfileList<User> _userTable = [new()
    {
        NickName = "XFEstudio",
        UserName = "Admin",
        Password = "123456",
        PermissionLevel = (int)UserRole.经理
    }];
    /// <summary>
    /// 加密的用户登录列表
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current._encryptedUserLoginModelTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._encryptedUserLoginModelTable")]
    private ProfileList<EncryptedUserLoginModel> _encryptedUserLoginModelTable = [];
}

using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;

namespace XFEExtension.NetCore.ServerInteractive.TServer;

public partial class UserProfile : XFEProfile
{
    /// <summary>
    /// 用户列表
    /// </summary>
    [ProfileProperty]
    [ProfilePropertyAddGet("Current.userTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.userTable")]
    private ProfileList<User> userTable = [new()
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
    [ProfilePropertyAddGet("Current.encryptedUserLoginModelTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.encryptedUserLoginModelTable")]
    private ProfileList<EncryptedUserLoginModel> encryptedUserLoginModelTable = [];
}

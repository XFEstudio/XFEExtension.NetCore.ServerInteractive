using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 用户登录校验服务
/// </summary>
public class UserReloginService<T> : ServerCoreUserLoginServiceBase<T> where T : class
{
    /// <inheritdoc/>
    public override async Task RequestReceiveAsync()
    {
        Console.Write("校验登录请求：");
        var session = Regex.Unescape(Json?["session"]?.ToString() ?? string.Empty);
        Console.Write(session[..10]);
        var deviceInfo = Json?["deviceInfo"]?.ToString();
        if (session.IsNullOrWhiteSpace()) throw Error("Session值不能为空");
        if (deviceInfo.IsNullOrWhiteSpace()) throw Error("电脑信息不能为空");
        var split = session.Split('|');
        if (GetEncryptedUserLoginModelFunction().FirstOrDefault(user => user.UserLoginModel.Uid == split[0]) is not { } encryptedUserLoginModel) throw Error("Session值不正确或已过期", HttpStatusCode.Forbidden);
        if (encryptedUserLoginModel.UserLoginModel.DeviceInfo != deviceInfo) throw Error("电脑信息不匹配");
        var userLoginModel = UserHelper.Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]);
        if (userLoginModel.Uid.IsNullOrWhiteSpace() || userLoginModel.Uid != encryptedUserLoginModel.UserLoginModel.Uid)
            throw Error("登录用户ID不匹配");
        var user = UserHelper.GetUser(userLoginModel.Uid, GetUserFunction()) ?? throw Error("用户ID未注册", HttpStatusCode.Forbidden);
        if (userLoginModel.LastIPAddress != ReturnArgs.Args.ClientIP || userLoginModel.LastIPAddress != encryptedUserLoginModel.UserLoginModel.LastIPAddress) throw Error("IP地址不匹配");
        if (userLoginModel.EndDateTime < DateTime.Now || userLoginModel.EndDateTime != encryptedUserLoginModel.UserLoginModel.EndDateTime) throw Error("登录已过期");
        if (userLoginModel.DeviceInfo != encryptedUserLoginModel.UserLoginModel.DeviceInfo) throw Error("电脑信息不匹配");
        await Close(JsonSerializer.Serialize(LoginResultConvertFunction(user), JsonSerializerOptions));
    }
}

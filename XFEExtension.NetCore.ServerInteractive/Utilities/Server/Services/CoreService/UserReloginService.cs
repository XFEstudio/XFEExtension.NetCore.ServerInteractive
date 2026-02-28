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
    public override async Task StandardRequestReceived()
    {
        Console.Write($"校验登录请求：");
        var session = Regex.Unescape(Json["session"].ToString());
        Console.WriteLine(session);
        var computerInfo = Json["computerInfo"].ToString();
        if (session.IsNullOrWhiteSpace()) Error("Session值不能为空");
        if (computerInfo.IsNullOrWhiteSpace()) Error("电脑信息不能为空");
        var split = session.Split('|');
        if (GetEncryptedUserLoginModelFunction().FirstOrDefault(user => user.UserLoginModel.UID == split[0]) is not EncryptedUserLoginModel encryptedUserLoginModel) throw ReturnArgs.GetError("Session值不正确或已过期", HttpStatusCode.Forbidden);
        if (encryptedUserLoginModel.UserLoginModel.ComputerInfo != computerInfo) Error("电脑信息不匹配");
        var userLoginModel = UserHelper.Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]);
        if (userLoginModel.UID.IsNullOrWhiteSpace() || userLoginModel.UID != encryptedUserLoginModel.UserLoginModel.UID)
            Error("登录用户ID不匹配");
        var user = UserHelper.GetUser(userLoginModel.UID, GetUserFunction()) ?? throw ReturnArgs.GetError("用户ID未注册", HttpStatusCode.Forbidden);
        if (userLoginModel.LastIPAddress != ReturnArgs.Args.ClientIP) Error("IP地址不匹配");
        if (userLoginModel.LastIPAddress != encryptedUserLoginModel.UserLoginModel.LastIPAddress) Error("IP地址不匹配");
        if (userLoginModel.EndDateTime < DateTime.Now) Error("登录已过期");
        if (userLoginModel.EndDateTime != encryptedUserLoginModel.UserLoginModel.EndDateTime) Error("登录已过期");
        if (userLoginModel.ComputerInfo != encryptedUserLoginModel.UserLoginModel.ComputerInfo) Error("电脑信息不匹配");
        await Close(JsonSerializer.Serialize(LoginResultConvertFunction(user), JsonSerializerOptions));
    }
}

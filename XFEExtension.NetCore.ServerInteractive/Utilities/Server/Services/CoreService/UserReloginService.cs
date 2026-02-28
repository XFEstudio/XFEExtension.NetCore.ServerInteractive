using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 用户登录校验服务
/// </summary>
public class UserReloginService<T> : ServerCoreUserLoginServiceBase<T> where T : class
{
    /// <inheritdoc/>
    public override async Task StandardRequestReceived()
    {
        Console.Write($"【{ReturnArgs!.Args.ClientIP}】校验登录请求：");
        var session = Regex.Unescape(QueryableJsonNode!["session"].ToString());
        Console.WriteLine(session);
        var computerInfo = QueryableJsonNode!["computerInfo"].ToString();
        if (session.IsNullOrWhiteSpace()) Close("Session值不能为空", HttpStatusCode.BadRequest);
        if (computerInfo.IsNullOrWhiteSpace()) Close("电脑信息不能为空", HttpStatusCode.BadRequest);
        var split = session.Split('|');
        if (GetEncryptedUserLoginModelFunction().FirstOrDefault(user => user.UserLoginModel.UID == split[0]) is not EncryptedUserLoginModel encryptedUserLoginModel) throw ReturnArgs.GetError("Session值不正确或已过期", HttpStatusCode.Forbidden);
        if (encryptedUserLoginModel.UserLoginModel.ComputerInfo != computerInfo) Close("电脑信息不匹配", HttpStatusCode.Forbidden);
        var userLoginModel = UserHelper.Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]);
        if (userLoginModel.UID.IsNullOrWhiteSpace() || userLoginModel.UID != encryptedUserLoginModel.UserLoginModel.UID)
            Close("登录用户ID不匹配", HttpStatusCode.Forbidden);
        var user = UserHelper.GetUser(userLoginModel.UID, GetUserFunction()) ?? throw ReturnArgs.GetError("用户ID未注册", HttpStatusCode.Forbidden);
        if (userLoginModel.LastIPAddress != ReturnArgs.Args.ClientIP) Close("IP地址不匹配", HttpStatusCode.Forbidden);
        if (userLoginModel.LastIPAddress != encryptedUserLoginModel.UserLoginModel.LastIPAddress) Close("IP地址不匹配", HttpStatusCode.Forbidden);
        if (userLoginModel.EndDateTime < DateTime.Now) Close("登录已过期", HttpStatusCode.Forbidden);
        if (userLoginModel.EndDateTime != encryptedUserLoginModel.UserLoginModel.EndDateTime) Close("登录已过期", HttpStatusCode.Forbidden);
        if (userLoginModel.ComputerInfo != encryptedUserLoginModel.UserLoginModel.ComputerInfo) Close("电脑信息不匹配", HttpStatusCode.Forbidden);
        await ReturnArgs.Args.ReplyAndClose(JsonSerializer.Serialize(LoginResultConvertFunction(user), JsonSerializerOptions));
    }
}

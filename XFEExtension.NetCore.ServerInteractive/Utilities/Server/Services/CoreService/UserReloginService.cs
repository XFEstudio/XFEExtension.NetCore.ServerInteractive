using System.Net;
using System.Text.RegularExpressions;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.StringExtension.Json;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 用户登录校验服务
/// </summary>
public class UserReloginService<T, F> : ServerCoreUserLoginServiceBase<T, F> where T : IUserInfo where F : class
{
    /// <inheritdoc/>
    public override async Task StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r)
    {
        Console.Write($"【{r.Args.ClientIP}】校验登录请求：");
        var session = Regex.Unescape(queryableJsonNode["session"].ToString());
        Console.WriteLine(session);
        var computerInfo = queryableJsonNode["computerInfo"].ToString();
        if (session.IsNullOrWhiteSpace()) r.Error("Session值不能为空", HttpStatusCode.BadRequest);
        if (computerInfo.IsNullOrWhiteSpace()) r.Error("电脑信息不能为空", HttpStatusCode.BadRequest);
        var split = session.Split('|');
        if (GetEncryptedUserLoginModelFunction().FirstOrDefault(user => user.UserLoginModel.UID == split[0]) is not EncryptedUserLoginModel encryptedUserLoginModel) throw r.GetError("Session值不正确或已过期", HttpStatusCode.Forbidden);
        if (encryptedUserLoginModel.UserLoginModel.ComputerInfo != computerInfo) r.Error("电脑信息不匹配", HttpStatusCode.Forbidden);
        var userLoginModel = UserHelper.Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]);
        if (userLoginModel.UID.IsNullOrWhiteSpace() || userLoginModel.UID != encryptedUserLoginModel.UserLoginModel.UID)
            r.Error("登录用户ID不匹配", HttpStatusCode.Forbidden);
        var user = UserHelper.GetUser(userLoginModel.UID, GetUserFunction().Cast<IUserFaceInfo>()) ?? throw r.GetError("用户ID未注册", HttpStatusCode.Forbidden);
        if (userLoginModel.LastIPAddress != r.Args.ClientIP) r.Error("IP地址不匹配", HttpStatusCode.Forbidden);
        if (userLoginModel.LastIPAddress != encryptedUserLoginModel.UserLoginModel.LastIPAddress) r.Error("IP地址不匹配", HttpStatusCode.Forbidden);
        if (userLoginModel.EndDateTime < DateTime.Now) r.Error("登录已过期", HttpStatusCode.Forbidden);
        await r.Args.ReplyAndClose(LoginResultConverter((T)user).ToJson());
    }
}

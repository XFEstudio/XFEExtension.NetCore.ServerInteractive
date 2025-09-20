using System.Net;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

public class UserLoginService : ServerCoreStandardRegisterServiceBase
{
    /// <inheritdoc/>
    public override async void StandardRequestReceived(object? sender, string execute, QueryableJsonNode queryableJsonNode, CyberCommRequestEventArgs e)
    {
        Console.Write($"【{e.ClientIP}】登录请求：");
        var account = queryableJsonNode["account"].ToString();
        var password = queryableJsonNode["password"].ToString();
        var computerInfo = queryableJsonNode["computerInfo"].ToString();
        if (account.IsNullOrWhiteSpace()) throw new StopAction(() => statusCode = HttpStatusCode.BadRequest, "账户名不能为空");
        if (password.IsNullOrWhiteSpace()) throw new StopAction(() => statusCode = HttpStatusCode.BadRequest, "登录密码不能为空");
        if (computerInfo.IsNullOrWhiteSpace()) throw new StopAction(() => statusCode = HttpStatusCode.BadRequest, "电脑信息不能为空");
        var user = UserHelper.GetUser(queryableJsonNode["account"], queryableJsonNode["password"], UserProfile.UserList, ref statusCode);
        Console.WriteLine($"{account}（{computerInfo}）");
        var userLogin = UserProfile.EncryptedUserLoginList.FirstOrDefault(userLogin => userLogin.UserLoginModel.UID == user.ID);
        if (userLogin is null)
        {
            userLogin = new EncryptedUserLoginModel
            {
                Key = AESHelper.GenerateRandomKey(),
                UserLoginModel = new UserLoginModel
                {
                    UID = user.ID,
                    ComputerInfo = computerInfo,
                    LastIPAddress = e.ClientIP,
                    EndDateTime = DateTime.Now.AddDays(ServerProfile.LoginKeepDays)
                }
            };
            UserProfile.EncryptedUserLoginList.Add(userLogin);
            Console.WriteLine($"[DEBUG]用户 {user.UserName} 登录成功，登录到期时间 {userLogin.UserLoginModel.EndDateTime}");
            await e.ReplyAndClose(new
            {
                session = UserHelper.Encrypt(userLogin.Key, userLogin.UserLoginModel),
                expireDate = userLogin.UserLoginModel.EndDateTime
            }.ToJson(), HttpStatusCode.OK);
        }
        else
        {
            userLogin.UserLoginModel.ComputerInfo = computerInfo;
            userLogin.UserLoginModel.LastIPAddress = e.ClientIP;
            userLogin.UserLoginModel.EndDateTime = DateTime.Now.AddDays(ServerProfile.LoginKeepDays);
            Console.WriteLine($"[DEBUG]用户 {user.UserName} 已登录，登录到期时间：{userLogin.UserLoginModel.EndDateTime}");
            await e.ReplyAndClose(new
            {
                session = UserHelper.Encrypt(userLogin.Key, userLogin.UserLoginModel),
                expireDate = userLogin.UserLoginModel.EndDateTime
            }.ToJson(), HttpStatusCode.OK);
        }
    }
}

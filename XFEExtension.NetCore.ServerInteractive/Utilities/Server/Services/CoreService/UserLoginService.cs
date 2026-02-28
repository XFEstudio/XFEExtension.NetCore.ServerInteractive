using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.StringExtension.Json;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 用户登录服务
/// </summary>
public class UserLoginService<T> : ServerCoreUserLoginServiceBase<T> where T : class
{
    /// <inheritdoc/>
    public override async Task StandardRequestReceived()
    {
        Console.Write($"登录请求：");
        var account = Json["account"].ToString();
        var password = Json["password"].ToString();
        var computerInfo = Json["computerInfo"].ToString();
        if (account.IsNullOrWhiteSpace()) Error("账户名不能为空");
        if (password.IsNullOrWhiteSpace()) Error("登录密码不能为空");
        if (computerInfo.IsNullOrWhiteSpace()) Error("电脑信息不能为空");
        var user = UserHelper.GetUser(Json["account"], Json["password"], GetUserFunction(), ReturnArgs!);
        Console.WriteLine($"{account}（{computerInfo}）");
        var userLoginList = GetEncryptedUserLoginModelFunction().Where(userLogin => userLogin.UserLoginModel.ComputerInfo == computerInfo);
        var userLogin = GetEncryptedUserLoginModelFunction().FirstOrDefault(userLogin => userLogin.UserLoginModel.UID == user.ID);
        //for (int i = 0; i < userLoginList.Count(); i++)
        //{
        //    var otherLogin = userLoginList.ElementAt(i);
        //    if (otherLogin.UserLoginModel.UID != user.ID)
        //    {
        //        RemoveEncryptedUserLoginModelFunction(otherLogin);
        //        i--;
        //        Console.WriteLine($"[DEBUG]删除用户 {otherLogin.UserLoginModel.UID} 在电脑 {computerInfo} 上的登录状态");
        //    }
        //}
        if (userLogin is null)
        {
            userLogin = new EncryptedUserLoginModel
            {
                Key = AESHelper.GenerateRandomKey(),
                UserLoginModel = new UserLoginModel
                {
                    UID = user.ID,
                    ComputerInfo = computerInfo,
                    LastIPAddress = ReturnArgs!.Args.ClientIP,
                    EndDateTime = DateTime.Now.AddDays(GetLoginKeepDays())
                }
            };
            AddEncryptedUserLoginModelFunction(userLogin);
            Console.WriteLine($"[DEBUG]用户 {user.UserName} 登录成功，登录到期时间 {userLogin.UserLoginModel.EndDateTime}");
            await ReturnArgs!.Args.ReplyAndClose(JsonSerializer.Serialize(new
            {
                session = $"{user.ID}|{UserHelper.Encrypt(userLogin.Key, userLogin.UserLoginModel)}",
                expireDate = userLogin.UserLoginModel.EndDateTime,
                userInfo = LoginResultConvertFunction(user)
            }, JsonSerializerOptions), HttpStatusCode.OK);
        }
        else
        {
            userLogin.UserLoginModel.ComputerInfo = computerInfo;
            userLogin.UserLoginModel.LastIPAddress = ReturnArgs!.Args.ClientIP;
            userLogin.UserLoginModel.EndDateTime = DateTime.Now.AddDays(GetLoginKeepDays());
            Console.WriteLine($"[DEBUG]用户 {user.UserName} 已登录，登录到期时间：{userLogin.UserLoginModel.EndDateTime}");
            await Close(JsonSerializer.Serialize(new
            {
                session = $"{user.ID}|{UserHelper.Encrypt(userLogin.Key, userLogin.UserLoginModel)}",
                expireDate = userLogin.UserLoginModel.EndDateTime,
                userInfo = LoginResultConvertFunction(user)
            }, JsonSerializerOptions));
        }
    }
}

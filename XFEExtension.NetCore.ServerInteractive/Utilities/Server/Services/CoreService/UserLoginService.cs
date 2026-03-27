using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 用户登录服务
/// </summary>
public class UserLoginService<T> : ServerCoreUserLoginServiceBase<T> where T : class
{
    /// <inheritdoc/>
    public override async Task RequestReceiveAsync()
    {
        Console.Write("登录请求");
        var account = Json?["account"]?.ToString();
        var password = Json?["password"]?.ToString();
        var deviceInfo = Json?["deviceInfo"]?.ToString();
        if (account.IsNullOrWhiteSpace()) throw Error("账户名不能为空");
        if (password.IsNullOrWhiteSpace()) throw Error("登录密码不能为空");
        if (deviceInfo.IsNullOrWhiteSpace()) throw Error("电脑信息不能为空");
        var user = UserHelper.GetUser(account, password, GetUserFunction(), ReturnArgs);
        Console.Write($"{account}（{deviceInfo[..10]}...）");
        var userLogin = GetEncryptedUserLoginModelFunction().FirstOrDefault(userLogin => userLogin.UserLoginModel.Uid == user.Id);
        //for (int i = 0; i < userLoginList.Count(); i++)
        //{
        //    var otherLogin = userLoginList.ElementAt(i);
        //    if (otherLogin.UserLoginModel.UID != user.ID)
        //    {
        //        RemoveEncryptedUserLoginModelFunction(otherLogin);
        //        i--;
        //        Console.WriteLine($"[DEBUG]删除用户 {otherLogin.UserLoginModel.UID} 在电脑 {deviceInfo} 上的登录状态");
        //    }
        //}
        if (userLogin is null)
        {
            userLogin = new EncryptedUserLoginModel
            {
                Key = AesHelper.GenerateRandomKey(),
                UserLoginModel = new UserLoginModel
                {
                    Uid = user.Id,
                    DeviceInfo = deviceInfo,
                    LastIPAddress = ReturnArgs.Args.ClientIP,
                    EndDateTime = DateTime.Now.AddDays(GetLoginKeepDays())
                }
            };
            AddEncryptedUserLoginModelFunction(userLogin);
            Console.Write($"到期时间 {userLogin.UserLoginModel.EndDateTime}");
        }
        else
        {
            userLogin.UserLoginModel.DeviceInfo = deviceInfo;
            userLogin.UserLoginModel.LastIPAddress = ReturnArgs.Args.ClientIP;
            userLogin.UserLoginModel.EndDateTime = DateTime.Now.AddDays(GetLoginKeepDays());
            Console.Write($"到期时间：{userLogin.UserLoginModel.EndDateTime}");
        }

        await Close(JsonSerializer.Serialize(new
        {
            session = $"{user.Id}|{UserHelper.Encrypt(userLogin.Key, userLogin.UserLoginModel)}",
            expireDate = userLogin.UserLoginModel.EndDateTime,
            userInfo = LoginResultConvertFunction(user)
        }, JsonSerializerOptions));
    }
}

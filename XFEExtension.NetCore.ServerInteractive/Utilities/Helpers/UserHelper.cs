using SCCApplication.Core.Models.UserModels;
using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

public static class UserHelper
{
    public static User? GetUser(string id, IEnumerable<User> userInfoList) => userInfoList.FirstOrDefault(user => user.ID == id);

    public static UserOperateResult GetUser(string sessionId, string computerInfo, string ipAddress, JsonSerializerOptions? jsonSerializerOptions, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<User> userInfoList, out User? user)
    {
        user = null;
        if (encryptedUserLoginModels.FirstOrDefault(user => user.UserLoginModel.ComputerInfo == computerInfo) is not EncryptedUserLoginModel encryptedUserLoginModel)
            return UserOperateResult.LoginExpired;
        if (Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, sessionId, jsonSerializerOptions) is not UserLoginModel targetUserLoginModel || encryptedUserLoginModel.UserLoginModel.UID != targetUserLoginModel.UID)
            return UserOperateResult.UserNotFound;
        if (encryptedUserLoginModel.UserLoginModel.EndDateTime < DateTime.Now || (encryptedUserLoginModel.UserLoginModel.LastIPAddress != ipAddress && !((encryptedUserLoginModel.UserLoginModel.LastIPAddress == "127.0.0.1" || encryptedUserLoginModel.UserLoginModel.LastIPAddress == "::1") && (ipAddress == "127.0.0.1" || ipAddress == "::1"))))
            return UserOperateResult.LoginExpired;
        if (GetUser(targetUserLoginModel.UID, userInfoList) is not User userInfo)
            return UserOperateResult.UserNotFound;
        if (!userInfo.Enable)
            return UserOperateResult.UserDisabled;
        user = userInfo;
        Console.WriteLine($"({user.UserName})");
        return UserOperateResult.Success;
    }

    public static UserOperateResult GetUser(string userName, string password, IEnumerable<User> userInfoList, out User? user)
    {
        user = null;
        if (userInfoList.FirstOrDefault(user => user.UserName == userName) is User userInfo)
        {
            if (userInfo.Password == password)
            {
                if (userInfo.Enable)
                {
                    user = userInfo;
                    return UserOperateResult.Success;
                }
                else
                {
                    return UserOperateResult.UserDisabled;
                }
            }
            else
            {
                return UserOperateResult.InvalidPassword;
            }
        }
        else
        {
            return UserOperateResult.UserNotFound;
        }
    }

    public static User GetUser(string userName, string password, IEnumerable<User> userInfoList, ref HttpStatusCode statusCode)
    {
        var result = GetUser(userName, password, userInfoList, out var user);
        if (result != UserOperateResult.Success)
        {
            statusCode = HttpStatusCode.Forbidden;
            throw new StopAction(() => { }, OutPutResult(result));
        }
        return user!;
    }

    public static UserOperateResult ValidateUserPermission(string userName, string password, UserRole requiredRole, IEnumerable<User> userInfoList)
    {
        var result = GetUser(userName, password, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            return result;
        if (user!.PermissionLevel < requiredRole)
            return UserOperateResult.PermissionDenied;
        return UserOperateResult.Success;
    }

    public static UserOperateResult ValidateUserPermission(string sessionId, string computerInfo, string ipAddress, UserRole requiredRole, JsonSerializerOptions? jsonSerializerOptions, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<User> userInfoList)
    {
        var result = GetUser(sessionId, computerInfo, ipAddress, jsonSerializerOptions, encryptedUserLoginModels, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            return result;
        if (user!.PermissionLevel < requiredRole)
            return UserOperateResult.PermissionDenied;
        return UserOperateResult.Success;
    }

    public static void ValidatePermission(string userName, string password, UserRole requiredRole, IEnumerable<User> userInfoList, ref HttpStatusCode statusCode)
    {
        var result = ValidateUserPermission(userName, password, requiredRole, userInfoList);
        if (result != UserOperateResult.Success)
        {
            statusCode = HttpStatusCode.Forbidden;
            throw new StopAction(() => { }, $"\n{OutPutResult(result)}");
        }
    }

    public static void ValidatePermission(string sessionId, string computerInfo, string ipAddress, UserRole requiredRole, JsonSerializerOptions? jsonSerializerOptions, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<User> userInfoList, ref HttpStatusCode statusCode)
    {
        var result = ValidateUserPermission(sessionId, computerInfo, ipAddress, requiredRole, jsonSerializerOptions, encryptedUserLoginModels, userInfoList);
        if (result != UserOperateResult.Success)
        {
            statusCode = HttpStatusCode.Forbidden;
            throw new StopAction(() => { }, OutPutResult(result));
        }
    }

    public static string OutPutResult(UserOperateResult userOperateResult) => userOperateResult switch
    {
        UserOperateResult.Success => "操作成功",
        UserOperateResult.UserNotFound => "用户不存在",
        UserOperateResult.InvalidPassword => "密码错误",
        UserOperateResult.UserDisabled => "用户被禁用",
        UserOperateResult.PermissionDenied => "权限不足",
        UserOperateResult.LoginExpired => "登录过期",
        _ => "未知错误"
    };
    /// <summary>
    /// 加密用户登录模型
    /// </summary>
    /// <param name="key"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public static string Encrypt<T>(string key, T model) where T : class => AESHelper.Encrypt(model.ToJson(), key);

    /// <summary>
    /// 解密用户登录模型
    /// </summary>
    /// <param name="key"></param>
    /// <param name="encryptedModel"></param>
    /// <returns></returns>
    public static T Decrypt<T>(string key, string encryptedModel, JsonSerializerOptions? jsonSerializerOptions = null) where T : class, new() => JsonSerializer.Deserialize<T>(AESHelper.Decrypt(encryptedModel, key), jsonSerializerOptions) ?? new();
}
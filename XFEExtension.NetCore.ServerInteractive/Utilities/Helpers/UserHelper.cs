using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// 用户帮助类
/// </summary>
public static class UserHelper
{
    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userInfoList"></param>
    /// <returns></returns>
    public static IUserFaceInfo? GetUser(string id, IEnumerable<IUserFaceInfo> userInfoList) => userInfoList.FirstOrDefault(user => user.ID == id);

    /// <summary>
    /// 获取用户（通过Session）
    /// </summary>
    /// <param name="session"></param>
    /// <param name="computerInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static UserOperateResult GetUser(string session, string computerInfo, string ipAddress, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserFaceInfo> userInfoList, out IUserInfo? user)
    {
        user = null;
        var split = session.Split('|');
        if (encryptedUserLoginModels.FirstOrDefault(user => user.UserLoginModel.UID == split[0]) is not EncryptedUserLoginModel encryptedUserLoginModel)
            return UserOperateResult.LoginExpired;
        if (encryptedUserLoginModel.UserLoginModel.ComputerInfo != computerInfo) return UserOperateResult.LoginExpired;
        if (Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]) is not UserLoginModel targetUserLoginModel || encryptedUserLoginModel.UserLoginModel.UID != targetUserLoginModel.UID)
            return UserOperateResult.UserNotFound;
        if (encryptedUserLoginModel.UserLoginModel.EndDateTime < DateTime.Now || (encryptedUserLoginModel.UserLoginModel.LastIPAddress != ipAddress && !((encryptedUserLoginModel.UserLoginModel.LastIPAddress == "127.0.0.1" || encryptedUserLoginModel.UserLoginModel.LastIPAddress == "::1") && (ipAddress == "127.0.0.1" || ipAddress == "::1"))))
            return UserOperateResult.LoginExpired;
        if (GetUser(targetUserLoginModel.UID, userInfoList) is not IUserInfo userInfo)
            return UserOperateResult.UserNotFound;
        if (!userInfo.Enable)
            return UserOperateResult.UserDisabled;
        user = userInfo;
        Console.WriteLine($"({user.UserName})");
        return UserOperateResult.Success;
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="userInfoList"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static UserOperateResult GetUser(string userName, string password, IEnumerable<IUserInfo> userInfoList, out IUserInfo? user)
    {
        user = null;
        if (userInfoList.FirstOrDefault(user => user.UserName == userName) is IUserInfo userInfo)
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

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="userInfoList"></param>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    /// <exception cref="StopAction"></exception>
    public static IUserInfo GetUser(string userName, string password, IEnumerable<IUserInfo> userInfoList, ref HttpStatusCode statusCode)
    {
        var result = GetUser(userName, password, userInfoList, out var user);
        if (result != UserOperateResult.Success)
        {
            statusCode = HttpStatusCode.Forbidden;
            throw new StopAction(() => { }, OutPutResult(result));
        }
        return user!;
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="userInfoList"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    /// <exception cref="StopAction"></exception>
    public static IUserInfo GetUser(string userName, string password, IEnumerable<IUserInfo> userInfoList, ServerCoreReturnArgs r)
    {
        var result = GetUser(userName, password, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            r.Error(OutPutResult(result), HttpStatusCode.Forbidden);
        return user!;
    }

    /// <summary>
    /// 校验用户权限
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="userInfoList"></param>
    /// <returns></returns>
    public static UserOperateResult ValidateUserPermission(string userName, string password, int requiredPermissionLevel, IEnumerable<IUserInfo> userInfoList)
    {
        var result = GetUser(userName, password, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            return result;
        if (user!.PermissionLevel < requiredPermissionLevel)
            return UserOperateResult.PermissionDenied;
        return UserOperateResult.Success;
    }

    /// <summary>
    /// 校验用户权限（使用Session）
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="computerInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <returns></returns>
    public static UserOperateResult ValidateUserPermission(string sessionId, string computerInfo, string ipAddress, int requiredPermissionLevel, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserInfo> userInfoList)
    {
        var result = GetUser(sessionId, computerInfo, ipAddress, encryptedUserLoginModels, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            return result;
        if (user!.PermissionLevel < requiredPermissionLevel)
            return UserOperateResult.PermissionDenied;
        return UserOperateResult.Success;
    }

    /// <summary>
    /// 校验权限
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="userInfoList"></param>
    /// <param name="statusCode"></param>
    /// <exception cref="StopAction"></exception>
    public static void ValidatePermission(string userName, string password, int requiredPermissionLevel, IEnumerable<IUserInfo> userInfoList, ref HttpStatusCode statusCode)
    {
        var result = ValidateUserPermission(userName, password, requiredPermissionLevel, userInfoList);
        if (result != UserOperateResult.Success)
        {
            statusCode = HttpStatusCode.Forbidden;
            throw new StopAction(() => { }, $"\n{OutPutResult(result)}");
        }
    }

    /// <summary>
    /// 校验权限
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="userInfoList"></param>
    /// <param name="r"></param>
    /// <exception cref="StopAction"></exception>
    public static void ValidatePermission(string userName, string password, int requiredPermissionLevel, IEnumerable<IUserInfo> userInfoList, ServerCoreReturnArgs r)
    {
        var result = ValidateUserPermission(userName, password, requiredPermissionLevel, userInfoList);
        if (result != UserOperateResult.Success)
        {
            r.StatusCode = HttpStatusCode.Forbidden;
            throw new StopAction(() => { }, $"\n{OutPutResult(result)}");
        }
    }

    /// <summary>
    /// 校验权限（使用Session）
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="computerInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <param name="r"></param>
    /// <exception cref="StopAction"></exception>
    public static void ValidatePermission(string sessionId, string computerInfo, string ipAddress, int requiredPermissionLevel, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserInfo> userInfoList, ServerCoreReturnArgs r)
    {
        var result = ValidateUserPermission(sessionId, computerInfo, ipAddress, requiredPermissionLevel, encryptedUserLoginModels, userInfoList);
        if (result != UserOperateResult.Success)
            r.Error(OutPutResult(result), HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// 输出结果
    /// </summary>
    /// <param name="userOperateResult"></param>
    /// <returns></returns>
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
    public static string Encrypt<T>(string key, T model) where T : class => AESHelper.Encrypt(JsonSerializer.Serialize(model), key);

    /// <summary>
    /// 解密用户登录模型
    /// </summary>
    /// <param name="key"></param>
    /// <param name="encryptedModel"></param>
    /// <returns></returns>
    public static T Decrypt<T>(string key, string encryptedModel) where T : class, new() => JsonSerializer.Deserialize<T>(AESHelper.Decrypt(encryptedModel, key)) ?? new();
}
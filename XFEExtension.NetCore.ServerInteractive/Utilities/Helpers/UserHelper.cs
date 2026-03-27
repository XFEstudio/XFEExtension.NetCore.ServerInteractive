using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using XFEExtension.NetCore.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.StringExtension;

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
    public static IUserFaceInfo? GetUser(string id, IEnumerable<IUserFaceInfo> userInfoList) => userInfoList.FirstOrDefault(user => user.Id == id);

    /// <summary>
    /// 获取用户（通过Session）
    /// </summary>
    /// <param name="session"></param>
    /// <param name="deviceInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public static UserOperateResult GetUser(string session, string deviceInfo, string ipAddress, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserFaceInfo> userInfoList, out IUserInfo? user)
    {
        user = null;
        session = Regex.Unescape(session);
        var split = session.Split('|');
        if (encryptedUserLoginModels.FirstOrDefault(user => user.UserLoginModel.Uid == split[0]) is not { } encryptedUserLoginModel || encryptedUserLoginModel.UserLoginModel.DeviceInfo != deviceInfo)
            return UserOperateResult.LoginExpired;
        if (Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]) is not { } targetUserLoginModel || encryptedUserLoginModel.UserLoginModel.Uid != targetUserLoginModel.Uid)
            return UserOperateResult.UserNotFound;
        if (encryptedUserLoginModel.UserLoginModel.EndDateTime < DateTime.Now || (encryptedUserLoginModel.UserLoginModel.LastIPAddress != ipAddress && !(encryptedUserLoginModel.UserLoginModel.LastIPAddress is "127.0.0.1" or "::1" && ipAddress is "127.0.0.1" or "::1")))
            return UserOperateResult.LoginExpired;
        if (GetUser(targetUserLoginModel.Uid, userInfoList) is not IUserInfo userInfo)
            return UserOperateResult.UserNotFound;
        if (!userInfo.Enable)
            return UserOperateResult.UserDisabled;
        user = userInfo;
        Console.Write($"({user.UserName})");
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
        switch (userInfoList.FirstOrDefault(user => user.UserName == userName))
        {
            case { } userInfo when userInfo.Password == password:
            {
                if (!userInfo.Enable) return UserOperateResult.UserDisabled;
                user = userInfo;
                return UserOperateResult.Success;
            }
            case not null:
                return UserOperateResult.InvalidPassword;
            default:
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
        if (result == UserOperateResult.Success) return user!;
        statusCode = HttpStatusCode.Forbidden;
        throw new StopAction(() => { }, OutPutResult(result));
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
        return result != UserOperateResult.Success ? throw r.Error(OutPutResult(result), HttpStatusCode.Forbidden) : user!;
    }

    /// <summary>
    /// 校验用户权限
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="userInfoList"></param>
    /// <returns></returns>
    public static UserOperateResult ValidateUserPermission(string? userName, string? password, int requiredPermissionLevel, IEnumerable<IUserInfo> userInfoList)
    {
        if (userName.IsNullOrWhiteSpace())
            return UserOperateResult.UserNotFound;
        if (password.IsNullOrWhiteSpace())
            return UserOperateResult.InvalidPassword;
        var result = GetUser(userName, password, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            return result;
        return user!.PermissionLevel < requiredPermissionLevel ? UserOperateResult.PermissionDenied : UserOperateResult.Success;
    }

    /// <summary>
    /// 校验用户权限（使用Session）
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="deviceInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <returns></returns>
    public static UserOperateResult ValidateUserPermission(string? sessionId, string? deviceInfo, string ipAddress, int requiredPermissionLevel, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserInfo> userInfoList)
    {
        if (sessionId.IsNullOrWhiteSpace() || deviceInfo.IsNullOrWhiteSpace())
            return UserOperateResult.UserNotFound;
        var result = GetUser(sessionId, deviceInfo, ipAddress, encryptedUserLoginModels, userInfoList, out var user);
        if (result != UserOperateResult.Success)
            return result;
        return user!.PermissionLevel < requiredPermissionLevel ? UserOperateResult.PermissionDenied : UserOperateResult.Success;
    }

    /// <summary>
    /// 校验用户权限（直接传入用户对象）
    /// </summary>
    /// <param name="userInfo">用户信息对象</param>
    /// <param name="requiredPermissionLevel">所需权限等级</param>
    /// <returns></returns>
    public static UserOperateResult ValidateUserPermission(IUserInfo userInfo, int requiredPermissionLevel)
    {
        if (!userInfo.Enable)
            return UserOperateResult.UserDisabled;
        return userInfo.PermissionLevel < requiredPermissionLevel ? UserOperateResult.PermissionDenied : UserOperateResult.Success;
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
    public static void ValidatePermission(string? userName, string? password, int requiredPermissionLevel, IEnumerable<IUserInfo> userInfoList, ref HttpStatusCode statusCode)
    {
        var result = ValidateUserPermission(userName, password, requiredPermissionLevel, userInfoList);
        if (result == UserOperateResult.Success) return;
        statusCode = HttpStatusCode.Forbidden;
        throw new StopAction(() => { }, $"\n{OutPutResult(result)}");
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
    public static void ValidatePermission(string? userName, string? password, int requiredPermissionLevel, IEnumerable<IUserInfo> userInfoList, ServerCoreReturnArgs r)
    {
        var result = ValidateUserPermission(userName, password, requiredPermissionLevel, userInfoList);
        if (result == UserOperateResult.Success) return;
        r.StatusCode = HttpStatusCode.Forbidden;
        throw new StopAction(() => { }, $"\n{OutPutResult(result)}");
    }

    /// <summary>
    /// 校验权限（使用Session）
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="deviceInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <param name="r"></param>
    /// <exception cref="StopAction"></exception>
    public static void ValidatePermission(string? sessionId, string? deviceInfo, string ipAddress, int requiredPermissionLevel, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserInfo> userInfoList, ServerCoreReturnArgs r)
    {
        var result = ValidateUserPermission(sessionId, deviceInfo, ipAddress, requiredPermissionLevel, encryptedUserLoginModels, userInfoList);
        if (result != UserOperateResult.Success)
            throw r.Error(OutPutResult(result), HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// 校验权限（直接传入用户对象）
    /// </summary>
    /// <param name="userInfo">用户信息对象</param>
    /// <param name="requiredPermissionLevel">所需权限等级</param>
    /// <param name="statusCode"></param>
    /// <exception cref="StopAction"></exception>
    public static void ValidatePermission(IUserInfo userInfo, int requiredPermissionLevel, ref HttpStatusCode statusCode)
    {
        var result = ValidateUserPermission(userInfo, requiredPermissionLevel);
        if (result == UserOperateResult.Success) return;
        statusCode = HttpStatusCode.Forbidden;
        throw new StopAction(() => { }, $"\n{OutPutResult(result)}");
    }

    /// <summary>
    /// 校验权限（直接传入用户对象）
    /// </summary>
    /// <param name="userInfo">用户信息对象</param>
    /// <param name="requiredPermissionLevel">所需权限等级</param>
    /// <param name="r"></param>
    public static void ValidatePermission(IUserInfo userInfo, int requiredPermissionLevel, ServerCoreReturnArgs r)
    {
        var result = ValidateUserPermission(userInfo, requiredPermissionLevel);
        if (result != UserOperateResult.Success)
            throw r.Error(OutPutResult(result), HttpStatusCode.Forbidden);
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
    public static string Encrypt<T>(string key, T model) where T : class => AesHelper.Encrypt(JsonSerializer.Serialize(model), key);

    /// <summary>
    /// 解密用户登录模型
    /// </summary>
    /// <param name="key"></param>
    /// <param name="encryptedModel"></param>
    /// <returns></returns>
    public static T Decrypt<T>(string key, string encryptedModel) where T : class, new() => JsonSerializer.Deserialize<T>(AesHelper.Decrypt(encryptedModel, key)) ?? new();
}
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
        if (split.Length != 2 || split[0].IsNullOrWhiteSpace() || split[1].IsNullOrWhiteSpace())
        {
            Console.Write($" Session格式无效：length={split.Length}, sessionLength={session.Length}");
            return UserOperateResult.LoginExpired;
        }
        if (encryptedUserLoginModels.FirstOrDefault(user => user.UserLoginModel.Uid == split[0]) is not { } encryptedUserLoginModel)
        {
            Console.Write($" Session未找到或已被清理：uid={split[0]}");
            return UserOperateResult.LoginExpired;
        }
        if (encryptedUserLoginModel.UserLoginModel.DeviceInfo != deviceInfo)
        {
            Console.Write($" 设备信息不匹配：uid={split[0]}, expected={FormatForLog(encryptedUserLoginModel.UserLoginModel.DeviceInfo)}, actual={FormatForLog(deviceInfo)}");
            return UserOperateResult.LoginExpired;
        }
        UserLoginModel targetUserLoginModel;
        try
        {
            targetUserLoginModel = Decrypt<UserLoginModel>(encryptedUserLoginModel.Key, split[1]);
        }
        catch (Exception ex)
        {
            Console.Write($" Session解密失败：uid={split[0]}, error={ex.Message}");
            return UserOperateResult.LoginExpired;
        }
        if (encryptedUserLoginModel.UserLoginModel.Uid != targetUserLoginModel.Uid)
        {
            Console.Write($" Session解密失败或用户ID不匹配：uid={split[0]}");
            return UserOperateResult.UserNotFound;
        }
        if (encryptedUserLoginModel.UserLoginModel.EndDateTime < DateTime.Now)
        {
            Console.Write($" Session到期：uid={split[0]}, expire={encryptedUserLoginModel.UserLoginModel.EndDateTime:O}, now={DateTime.Now:O}");
            return UserOperateResult.LoginExpired;
        }
        if (!IsSameIPAddress(encryptedUserLoginModel.UserLoginModel.LastIPAddress, ipAddress))
        {
            Console.Write($" IP地址不匹配：uid={split[0]}, expected={encryptedUserLoginModel.UserLoginModel.LastIPAddress}, actual={ipAddress}, expire={encryptedUserLoginModel.UserLoginModel.EndDateTime:O}");
            return UserOperateResult.LoginExpired;
        }
        if (GetUser(targetUserLoginModel.Uid, userInfoList) is not IUserInfo userInfo)
        {
            Console.Write($" 用户ID未注册：uid={targetUserLoginModel.Uid}");
            return UserOperateResult.UserNotFound;
        }
        if (!userInfo.Enable)
        {
            Console.Write($" 用户已禁用：uid={targetUserLoginModel.Uid}");
            return UserOperateResult.UserDisabled;
        }
        user = userInfo;
        Console.Write($"({user.UserName})");
        return UserOperateResult.Success;
    }

    internal static bool IsSameIPAddress(string loginIPAddress, string requestIPAddress) => loginIPAddress == requestIPAddress || (loginIPAddress is "127.0.0.1" or "::1" && requestIPAddress is "127.0.0.1" or "::1");

    private static string FormatForLog(string value)
    {
        if (value.IsNullOrEmpty())
            return "<empty>";
        return value.Length <= 16 ? value : $"{value[..16]}...";
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
    /// <param name="session"></param>
    /// <param name="deviceInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <returns></returns>
    public static UserOperateResult ValidateUserPermission(string? session, string? deviceInfo, string ipAddress, int requiredPermissionLevel, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserInfo> userInfoList)
    {
        if (session.IsNullOrWhiteSpace() || deviceInfo.IsNullOrWhiteSpace())
            return UserOperateResult.UserNotFound;
        var result = GetUser(session, deviceInfo, ipAddress, encryptedUserLoginModels, userInfoList, out var user);
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
    /// <param name="session"></param>
    /// <param name="deviceInfo"></param>
    /// <param name="ipAddress"></param>
    /// <param name="requiredPermissionLevel"></param>
    /// <param name="encryptedUserLoginModels"></param>
    /// <param name="userInfoList"></param>
    /// <param name="r"></param>
    /// <exception cref="StopAction"></exception>
    public static void ValidatePermission(string? session, string? deviceInfo, string ipAddress, int requiredPermissionLevel, IEnumerable<EncryptedUserLoginModel> encryptedUserLoginModels, IEnumerable<IUserInfo> userInfoList, ServerCoreReturnArgs r)
    {
        var result = ValidateUserPermission(session, deviceInfo, ipAddress, requiredPermissionLevel, encryptedUserLoginModels, userInfoList);
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

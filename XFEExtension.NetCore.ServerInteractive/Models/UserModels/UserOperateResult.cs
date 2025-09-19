namespace XFEExtension.NetCore.ServerInteractive.Models.UserModels;

/// <summary>
/// 用户操作结果
/// </summary>
public enum UserOperateResult
{
    /// <summary>
    /// 操作成功
    /// </summary>
    Success = 0,
    /// <summary>
    /// 用户未找到
    /// </summary>
    UserNotFound = 1,
    /// <summary>
    /// 密码错误
    /// </summary>
    InvalidPassword = 2,
    /// <summary>
    /// 用户被禁用
    /// </summary>
    UserDisabled = 3,
    /// <summary>
    /// 权限不足
    /// </summary>
    PermissionDenied = 4,
    /// <summary>
    /// 登录过期
    /// </summary>
    LoginExpired = 5,
}
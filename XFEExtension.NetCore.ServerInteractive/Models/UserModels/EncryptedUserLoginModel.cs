namespace XFEExtension.NetCore.ServerInteractive.Models.UserModels;

/// <summary>
/// 加密的用户登录模型
/// </summary>
public class EncryptedUserLoginModel
{
    /// <summary>
    /// 加密密钥
    /// </summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>
    /// 用户登录模型
    /// </summary>
    public required UserLoginModel UserLoginModel { get; set; }
}

namespace SCCApplication.Core.Models.UserModels;

/// <summary>
/// 用户登录模型
/// </summary>
public class UserLoginModel
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UID { get; set; } = string.Empty;
    /// <summary>
    /// 上一次登录IP
    /// </summary>
    public string LastIPAddress { get; set; } = string.Empty;
    /// <summary>
    /// 电脑配置信息
    /// </summary>
    public string ComputerInfo { get; set; } = string.Empty;
    /// <summary>
    /// 登录到期时间
    /// </summary>
    public DateTime EndDateTime { get; set; } = DateTime.MinValue;
}

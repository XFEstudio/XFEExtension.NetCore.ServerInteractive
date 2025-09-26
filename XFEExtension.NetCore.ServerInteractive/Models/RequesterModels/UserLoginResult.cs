using System.Text.Json.Serialization;
using XFEExtension.NetCore.ServerInteractive.Interfaces;

namespace XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;

/// <summary>
/// 用户登录结果
/// </summary>
/// <typeparam name="T">登录返回用户接口类型</typeparam>
public class UserLoginResult<T> where T : IUserFaceInfo
{
    /// <summary>
    /// 用户登录Session
    /// </summary>
    [JsonPropertyName("session")]
    public string Session { get; set; } = string.Empty;
    /// <summary>
    /// 用户登录到期日期
    /// </summary>
    [JsonPropertyName("expireDate")]
    public DateTime ExpireDate { get; set; }
    /// <summary>
    /// 用户信息
    /// </summary>
    [JsonPropertyName("userInfo")]
    public T UserInfo { get; set; } = default!;
}

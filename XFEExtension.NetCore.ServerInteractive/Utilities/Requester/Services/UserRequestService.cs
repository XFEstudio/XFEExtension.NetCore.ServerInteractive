using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 用户请求服务（包含登录与登录校验）
/// </summary>
public partial class UserRequestService<T> : StandardRequestServiceBase where T : IUserFaceInfo
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();

    /// <summary>
    /// 用户请求服务
    /// </summary>
    public UserRequestService() => _jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <summary>
    /// 解析登录响应
    /// </summary>
    [Response("user/login", Name = "login_user")]
    public object AnalyzeLoginResponse()
    {
        var result = JsonSerializer.Deserialize<UserLoginResult<T>>(Response, _jsonSerializerOptions);
        Session = result!.Session;
        return result;
    }

    /// <summary>
    /// 构造登录请求体
    /// </summary>
    [Request("user/login", Name = "login_user")]
    public object PostLoginRequest() => new
    {
        deviceInfo = DeviceInfo,
        account = Parameters[0],
        password = Parameters[1]
    };

    /// <summary>
    /// 解析登录校验响应
    /// </summary>
    [Response("user/relogin", Name = "relogin_user")]
    public object AnalyzeReloginResponse() => JsonSerializer.Deserialize<T>(Response, _jsonSerializerOptions)!;

    /// <summary>
    /// 构造登录校验请求体
    /// </summary>
    [Request("user/relogin", Name = "relogin_user")]
    public object PostReloginRequest() => new
    {
        session = Session,
        deviceInfo = DeviceInfo,
    };
}

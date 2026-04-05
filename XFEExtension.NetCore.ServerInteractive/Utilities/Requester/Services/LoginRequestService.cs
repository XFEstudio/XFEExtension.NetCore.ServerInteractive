using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 登录请求服务
/// </summary>
public partial class LoginRequestService<T> : StandardRequestServiceBase where T : IUserFaceInfo
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();

    /// <summary>
    /// 登录请求服务
    /// </summary>
    public LoginRequestService() => _jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <summary>
    /// 解析登录响应
    /// </summary>
    [Response("user/login")]
    public object AnalyzeLoginResponse()
    {
        var result = JsonSerializer.Deserialize<UserLoginResult<T>>(Response, _jsonSerializerOptions);
        Session = result!.Session;
        return result;
    }

    /// <summary>
    /// 构造登录请求体
    /// </summary>
    [Request("user/login")]
    public object PostLoginRequest() => new
    {
        deviceInfo = DeviceInfo,
        account = Parameters[0],
        password = Parameters[1]
    };
}

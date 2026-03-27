using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 登录请求服务
/// </summary>
public class LoginRequestService<T> : XFERequestServiceBase where T : IUserFaceInfo
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();
    /// <summary>
    /// 登录请求服务
    /// </summary>
    public LoginRequestService() => _jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <inheritdoc/>
    public override object AnalyzeResponse(string response)
    {
        var result = JsonSerializer.Deserialize<UserLoginResult<T>>(response, _jsonSerializerOptions);
        Session = result!.Session;
        return result;
    }

    /// <inheritdoc/>
    public override object PostRequest() => new
    {
        execute = Execute,
        deviceInfo = DeviceInfo,
        account = Parameters[0],
        password = Parameters[1]
    };
}

using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 登录校验请求服务
/// </summary>
public class ReloginRequestService<T> : XFERequestServiceBase where T : IUserFaceInfo
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();
    /// <summary>
    /// 登录校验请求服务
    /// </summary>
    public ReloginRequestService() => _jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response) => JsonSerializer.Deserialize<T>(response, _jsonSerializerOptions)!;

    /// <inheritdoc/>
    public override object PostRequest() => new
    {
        execute = Execute,
        session = Session,
        deviceInfo = DeviceInfo,
    };
}

using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Services;

/// <summary>
/// 登录校验请求服务
/// </summary>
public partial class ReloginRequestService<T> : StandardRequestServiceBase where T : IUserFaceInfo
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();

    /// <summary>
    /// 登录校验请求服务
    /// </summary>
    public ReloginRequestService() => _jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <summary>
    /// 解析登录校验响应
    /// </summary>
    [Response("user/relogin")]
    public object AnalyzeReloginResponse() => JsonSerializer.Deserialize<T>(Response, _jsonSerializerOptions)!;

    /// <summary>
    /// 构造登录校验请求体
    /// </summary>
    [Request("user/relogin")]
    public object PostReloginRequest() => new
    {
        session = Session,
        deviceInfo = DeviceInfo,
    };
}

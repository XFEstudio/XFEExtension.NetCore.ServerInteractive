using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

/// <summary>
/// 登录校验请求服务
/// </summary>
public class ReloginRequestService<T> : XFERequestServiceBase where T : IUserFaceInfo
{
    readonly JsonSerializerOptions jsonSerializerOptions = new();
    /// <summary>
    /// 登录校验请求服务
    /// </summary>
    public ReloginRequestService() => jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    /// <inheritdoc/>
    public override object AnalyzeResponse(string response) => JsonSerializer.Deserialize<T>(response, jsonSerializerOptions)!;

    /// <inheritdoc/>
    public override object PostRequest(string execute, params object[] parameters) => new
    {
        execute,
        session = XFEClientRequester.Session,
        computerInfo = XFEClientRequester.ComputerInfo,
    };
}

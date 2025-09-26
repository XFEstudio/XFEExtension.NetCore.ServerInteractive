using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester.Serviecs;

/// <summary>
/// 登录请求服务
/// </summary>
public class LoginRequestService<T> : XFERequestServiceBase where T : IUserFaceInfo
{
    readonly JsonSerializerOptions jsonSerializerOptions = new();
    /// <summary>
    /// 登录请求服务
    /// </summary>
    public LoginRequestService() => jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <inheritdoc/>
    public override object AnalyzeResponse(string response)
    {
        var result = JsonSerializer.Deserialize<UserLoginResult<T>>(response, jsonSerializerOptions);
        XFEClientRequester.Session = result!.Session;
        return result;
    }

    /// <inheritdoc/>
    public override object PostRequest(string execute, params object[] parameters) => new
    {
        execute,
        computerInfo = XFEClientRequester.ComputerInfo,
        account = parameters[0],
        password = parameters[1]
    };
}

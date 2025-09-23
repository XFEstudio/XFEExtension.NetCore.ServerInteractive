using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequester : IRequesterBase
{
    readonly JsonSerializerOptions jsonSerializerOptions = new();
    internal Dictionary<string, IRequestService> requestServiceDictionary = [];
    internal Dictionary<string, IXFERequestService> xFERequestServiceDictionary = [];
    /// <inheritdoc/>
    public string RequestAddress { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string Session { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string ComputerInfo { get; set; } = string.Empty;
    /// <inheritdoc/>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;

    /// <summary>
    /// XFE客户端请求器
    /// </summary>
    public XFEClientRequester()
    {
        jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    }

    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="XFERequesterException"></exception>
    public async Task<ClientRequestResult<T>> Request<T>(string serviceName, params object[] parameters) => (await Request(serviceName, parameters)).ConvertTo<T>();

    /// <summary>
    /// 请求
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="XFERequesterException"></exception>
    public async Task<ClientRequestResult<object>> Request(string serviceName, params object[] parameters)
    {
        try
        {
            if (requestServiceDictionary.TryGetValue(serviceName, out var service))
            {
                return await service.Request<object>(parameters);
            }
            else if (xFERequestServiceDictionary.TryGetValue(serviceName, out var xFEService))
            {
                var result = new ClientRequestResult<object>();
                var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress, xFEService.PostRequest(serviceName, parameters).ToJson());
                result.StatusCode = code;
                if (code == HttpStatusCode.OK)
                {
                    var requestResult = xFEService.AnalyzeResponse(response);
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                    result.Message = "Success";
                    result.Result = requestResult;
                }
                else
                {
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                    result.Message = response;
                }
                return result;
            }
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            return default!;
        }
        throw new XFERequesterException("请求的方法未注册");
    }
}

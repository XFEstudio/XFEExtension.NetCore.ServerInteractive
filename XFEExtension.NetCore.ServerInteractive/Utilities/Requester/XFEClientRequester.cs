using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.DelegateExtension;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequester : IRequesterBase
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new();
    internal Dictionary<string, Func<IRequestService>> RequestServiceDictionary = [];
    internal Dictionary<string, Func<IStandardRequestService>> StandardRequestServiceDictionary = [];
    internal Dictionary<string, StandardClientInstanceRequest> StandardClientInstanceRequestDictionary = [];
    /// <inheritdoc/>
    public string RequestAddress { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string Session { get; set; } = string.Empty;
    /// <inheritdoc/>
    public string DeviceInfo { get; set; } = string.Empty;
    /// <inheritdoc/>
    public event XFEEventHandler<object?, ServerInteractiveEventArgs>? MessageReceived;

    /// <summary>
    /// XFE客户端请求器
    /// </summary>
    protected XFEClientRequester()
    {
        _jsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
    }

    /// <summary>
    /// 请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serviceName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="XFERequesterException"></exception>
    public async Task<ClientRequestResult<T>> Request<T>(string serviceName, params object[] parameters) => (await Request(serviceName, parameters)).TryConvertTo<T>();

    /// <summary>
    /// 请求
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    /// <exception cref="XFERequesterException"></exception>
    public async Task<ClientRequestResult<object>> Request(string serviceName, params object[] parameters)
    {
        var result = new ClientRequestResult<object>();
        try
        {
            if (RequestServiceDictionary.TryGetValue(serviceName, out var serviceFactory))
            {
                var service = serviceFactory();
                service.XFEClientRequester = this;
                result = await service.Request<object>(parameters);
            }
            else if (StandardRequestServiceDictionary.TryGetValue(serviceName, out var xFEServiceFactory))
            {
                var xFEService = xFEServiceFactory();
                xFEService.XFEClientRequester = this;
                xFEService.DeviceInfo = DeviceInfo;
                xFEService.Parameters = parameters;
                // 通过RequestRouteMap解析实际路由路径
                var actualRoute = xFEService.RequestRouteMap.TryGetValue(serviceName, out var route) ? route : serviceName;
                xFEService.Route = actualRoute;
                if (!xFEService.RequestPoints.TryGetValue(serviceName, out var requestHandler))
                    throw new XFERequesterException($"未找到'{serviceName}'对应的请求处理方法（[Request]标记的方法）");
                var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress + $"/{actualRoute}", requestHandler(), _jsonSerializerOptions);
                result.StatusCode = code;
                if (code == HttpStatusCode.OK)
                {
                    xFEService.Response = response;
                    xFEService.UnescapedResponse = Regex.Unescape(response);
                    if (xFEService.ResponsePoints.TryGetValue(serviceName, out var responseHandler))
                    {
                        var requestResult = responseHandler();
                        result.Result = requestResult;
                    }
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                    result.Message = "Success";
                }
                else
                {
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                    result.Message = response;
                }
            }
            else if (StandardClientInstanceRequestDictionary.TryGetValue(serviceName, out var instance))
            {
                var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress + $"/{serviceName}", instance.ConstructBody(Session, DeviceInfo, parameters), _jsonSerializerOptions);
                result.StatusCode = code;
                if (code == HttpStatusCode.OK)
                {
                    var requestResult = instance.ProcessResponse?.Invoke(response);
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl("Success", code));
                    result.Message = "Success";
                    result.Result = requestResult ?? new();
                }
                else
                {
                    MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(response, code));
                    result.Message = response;
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            MessageReceived?.Invoke(this, new ServerInteractiveEventArgsImpl(ex.Message, HttpStatusCode.InternalServerError));
            result.Message = ex.Message;
            result.StatusCode = HttpStatusCode.InternalServerError;
            return result;
        }
    }
}

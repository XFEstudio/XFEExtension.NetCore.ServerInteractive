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
    internal List<(string Pattern, Func<IStandardRequestService> Factory)> WildcardStandardRequestServiceList = [];
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
                var actualRoute = xFEService.RequestRouteMap.GetValueOrDefault(serviceName, serviceName);
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
                        result.Result = responseHandler();
                    }
                    else
                    {
                        result.Result = response;
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
            else if (TryMatchWildcardStandardService(serviceName, out var wildcardFactory, out var matchedPattern))
            {
                var xFEService = wildcardFactory!();
                xFEService.XFEClientRequester = this;
                xFEService.DeviceInfo = DeviceInfo;
                xFEService.Parameters = parameters;
                // 通配符匹配：Route设为实际请求路径，以便服务方法中可通过Route属性查看具体路径
                xFEService.Route = serviceName;
                if (!xFEService.RequestPoints.TryGetValue(matchedPattern!, out var requestHandler))
                    throw new XFERequesterException($"未找到'{serviceName}'（通配符模式'{matchedPattern}'）对应的请求处理方法（[Request]标记的方法）");
                var (response, code) = await InteractiveHelper.GetServerResponse(RequestAddress + $"/{serviceName}", requestHandler(), _jsonSerializerOptions);
                result.StatusCode = code;
                if (code == HttpStatusCode.OK)
                {
                    xFEService.Response = response;
                    xFEService.UnescapedResponse = Regex.Unescape(response);
                    if (xFEService.ResponsePoints.TryGetValue(matchedPattern!, out var responseHandler))
                    {
                        result.Result = responseHandler();
                    }
                    else
                    {
                        result.Result = response;
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

    /// <summary>
    /// 尝试通过通配符模式匹配标准请求服务（选择最具体的匹配模式，与服务端行为一致）
    /// </summary>
    /// <param name="serviceName">请求路径</param>
    /// <param name="factory">匹配到的服务工厂</param>
    /// <param name="matchedPattern">匹配到的通配符模式</param>
    /// <returns>是否匹配成功</returns>
    private bool TryMatchWildcardStandardService(string serviceName, out Func<IStandardRequestService>? factory, out string? matchedPattern)
    {
        // 在所有命中的候选中选择最具体的模式（字面量段越多越优先），避免结果依赖注册顺序
        static int GetWildcardPatternPriority(string pattern)
        {
            var segments = pattern.Split('/');
            var literalSegmentCount = 0;
            var wildcardSegmentCount = 0;
            foreach (var segment in segments)
            {
                if (segment == "*")
                    wildcardSegmentCount++;
                else
                    literalSegmentCount++;
            }
            return (literalSegmentCount * 1000) - (wildcardSegmentCount * 10) + pattern.Length;
        }

        var bestPriority = int.MinValue;
        factory = null;
        matchedPattern = null;

        foreach (var (pattern, serviceFactory) in WildcardStandardRequestServiceList)
        {
            if (!RouteMatchHelper.MatchWildcardRoute(pattern, serviceName)) continue;
            var currentPriority = GetWildcardPatternPriority(pattern);
            if (currentPriority <= bestPriority) continue;
            bestPriority = currentPriority;
            factory = serviceFactory;
            matchedPattern = pattern;
        }

        return factory is not null;
    }
}

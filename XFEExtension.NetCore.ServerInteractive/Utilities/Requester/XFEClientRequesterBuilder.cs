using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Implements;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Options;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器构建器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequesterBuilder : XFEBuilderBase<XFEClientRequesterBuilder>
{
    private readonly XFEClientRequester _xFEClientRequester = new XFEClientRequesterImpl();
    private readonly Dictionary<string, Func<IRequestService>> _requestServiceDictionary = [];
    private readonly Dictionary<string, Func<IStandardRequestService>> _standardRequestServiceDictionary = [];
    private readonly List<(string Pattern, Func<IStandardRequestService> Factory)> _wildcardStandardRequestServiceList = [];
    private readonly Dictionary<string, StandardClientInstanceRequest> _standardClientInstanceRequestDictionary = [];
    /// <summary>
    /// 创建构建器
    /// </summary>
    /// <returns></returns>
    public static XFEClientRequesterBuilder CreateBuilder()
    {
        var builder = new XFEClientRequesterBuilderImpl();
        builder.AddParameter("XFEClientRequester", builder._xFEClientRequester);
        return builder;
    }

    /// <summary>
    /// 添加请求
    /// </summary>
    /// <typeparam name="T">请求服务泛型</typeparam>
    /// <param name="serviceName">请求名称</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddOriginRequest<T>(string serviceName) where T : IRequestService, new()
    {
        _requestServiceDictionary.Add(serviceName, () =>
        {
            var inst = new T();
            ApplyParameter(inst);
            return inst;
        });
        return this;
    }

    /// <summary>
    /// 注册标准请求服务（从RequestPoints/ResponsePoints/RequestRouteMap自动获取路由路径和名称）
    /// </summary>
    /// <typeparam name="T">请求服务泛型</typeparam>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest<T>() where T : IStandardRequestService, new()
    {
        var probeService = new T();
        ApplyParameter(probeService);

        var routeKeys = probeService.RequestPoints.Keys
            .Concat(probeService.ResponsePoints.Keys)
            .Concat(probeService.RequestRouteMap.Keys)
            .Distinct()
            .ToList();

        if (routeKeys.Count == 0)
            throw new InvalidOperationException($"类型 {typeof(T).Name} 的 RequestPoints/ResponsePoints/RequestRouteMap 为空，请确保已使用[Request]或[Response]标记方法");

        // 为每个路径/名称注册服务工厂（通配符路径注册到通配符列表，其余注册到标准字典）
        // 对于Name别名，需检查其目标路径是否为通配符：Name→通配符路径不支持，因为无法确定具体请求路径
        foreach (var key in routeKeys)
        {
            if (RouteMatchHelper.IsWildcardRoute(key))
            {
                if (_wildcardStandardRequestServiceList.Any(w => w.Pattern == key))
                    throw new InvalidOperationException($"通配符路由 '{key}' 重复注册");
                _wildcardStandardRequestServiceList.Add((key, () =>
                {
                    var inst = new T();
                    ApplyParameter(inst);
                    return inst;
                }));
            }
            else
            {
                // 检查Name别名是否映射到通配符路径
                if (probeService.RequestRouteMap.TryGetValue(key, out var targetPath) && targetPath != key && RouteMatchHelper.IsWildcardRoute(targetPath))
                    throw new InvalidOperationException($"名称别名 '{key}' 映射到通配符路径 '{targetPath}'，不支持通过名称调用通配符路由，请直接使用具体路径进行请求");
                _standardRequestServiceDictionary.Add(key, () =>
                {
                    var inst = new T();
                    ApplyParameter(inst);
                    return inst;
                });
            }
        }
        return this;
    }

    /// <summary>
    /// 添加实例请求
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="xFEClientInstanceRequest">请求实例</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest(string serviceName, StandardClientInstanceRequest xFEClientInstanceRequest)
    {
        _standardClientInstanceRequestDictionary.Add(serviceName, xFEClientInstanceRequest);
        return this;
    }

    /// <summary>
    /// 添加实例请求
    /// </summary>
    /// <param name="serviceName">请求服务名称</param>
    /// <param name="constructBody">构造请求体方法<seealso cref="object"/> (<seealso cref="string"/> session, <seealso cref="string"/> deviceInfo, <seealso cref="object"/>[] parameters)</param>
    /// <param name="processResponse">处理响应方法<seealso cref="object"/> (<seealso cref="string"/> response)</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest(string serviceName, Func<string, string, object[], object> constructBody, Func<string, object>? processResponse = null) => AddRequest(serviceName, new StandardClientInstanceRequest
    {
        ConstructBody = constructBody,
        ProcessResponse = processResponse
    });

    /// <summary>
    /// 构建XFE客户端请求器
    /// </summary>
    /// <returns>XFE客户端请求器</returns>
    public XFEClientRequester Build(XFEClientRequesterOptions options)
    {
        _xFEClientRequester.RequestAddress = options.RequestAddress;
        _xFEClientRequester.Session = options.Session;
        _xFEClientRequester.DeviceInfo = options.DeviceInfo;
        _xFEClientRequester.RequestServiceDictionary = _requestServiceDictionary;
        _xFEClientRequester.StandardRequestServiceDictionary = _standardRequestServiceDictionary;
        _xFEClientRequester.WildcardStandardRequestServiceList = _wildcardStandardRequestServiceList;
        _xFEClientRequester.StandardClientInstanceRequestDictionary = _standardClientInstanceRequestDictionary;
        return _xFEClientRequester;
    }

    /// <summary>
    /// 构建XFE客户端请求器
    /// </summary>
    /// <returns>XFE客户端请求器</returns>
    public XFEClientRequester Build(Action<XFEClientRequesterOptions> optionsBuilder)
    {
        var options = new XFEClientRequesterOptions();
        optionsBuilder(options);
        return Build(options);
    }
}

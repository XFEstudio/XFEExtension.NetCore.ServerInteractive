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
    private readonly Dictionary<string, IRequestService> _requestServiceDictionary = [];
    private readonly Dictionary<string, IXFERequestService> _xFERequestServiceDictionary = [];
    private readonly Dictionary<string, XFEClientInstanceRequest> _xFEClientInstanceRequestDictionary = [];
    /// <summary>
    /// 创建构建器
    /// </summary>
    /// <returns></returns>
    public static XFEClientRequesterBuilder CreateBuilder()
    {
        var builder = new XFEClientRequesterBuilderImpl
        {
            _xFEClientRequester =
            {
                RequestAddress = requestAddress,
                Session = session,
                ComputerInfo = computerInfo
            }
        };
        builder.AddParameter("XFEClientRequester", builder._xFEClientRequester);
        return builder;
    }

    /// <summary>
    /// 创建构建器
    /// </summary>
    /// <returns></returns>
    public static XFEClientRequesterBuilder CreateBuilder()
    {
        var builder = new XFEClientRequesterBuilderImpl
        {
            _xFEClientRequester =
            {
                RequestAddress = requestAddress,
                Session = string.Empty,
                ComputerInfo = DeviceHelper.GetUniqueHardwareId()
            }
        };
        builder.AddParameter("XFEClientRequester", builder._xFEClientRequester);
        return builder;
    }

    /// <summary>
    /// 添加请求
    /// </summary>
    /// <param name="serviceName">请求名称</param>
    /// <param name="requestService">请求服务对象</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest(string serviceName, IRequestService requestService)
    {
        ApplyParameter(requestService);
        _requestServiceDictionary.Add(serviceName, requestService);
        return this;
    }

    /// <summary>
    /// 添加请求
    /// </summary>
    /// <typeparam name="T">请求服务泛型</typeparam>
    /// <param name="serviceName">请求名称</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest<T>(string serviceName) where T : IRequestService, new() => AddRequest(serviceName, new T());

    /// <summary>
    /// 添加XFE请求器
    /// </summary>
    /// <param name="serviceName">请求服务名称</param>
    /// <param name="xFERequestService">请求服务对象</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddXFERequest(string serviceName, IXFERequestService xFERequestService)
    {
        ApplyParameter(xFERequestService);
        _xFERequestServiceDictionary.Add(serviceName, xFERequestService);
        return this;
    }

    /// <summary>
    /// 添加XFE请求器
    /// </summary>
    /// <typeparam name="T">请求服务泛型</typeparam>
    /// <param name="serviceName">请求服务名称</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddXFERequest<T>(string serviceName) where T : IXFERequestService, new() => AddXFERequest(serviceName, new T());

    /// <summary>
    /// 添加实例请求
    /// </summary>
    /// <param name="serviceName">服务名称</param>
    /// <param name="xFEClientInstanceRequest">请求实例</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest(string serviceName, XFEClientInstanceRequest xFEClientInstanceRequest)
    {
        _xFEClientInstanceRequestDictionary.Add(serviceName, xFEClientInstanceRequest);
        return this;
    }

    /// <summary>
    /// 添加实例请求
    /// </summary>
    /// <param name="serviceName">请求服务名称</param>
    /// <param name="constructBody">构造请求体方法<seealso cref="object"/> (<seealso cref="string"/> session, <seealso cref="string"/> computerInfo, <seealso cref="object"/>[] parameters)</param>
    /// <param name="processResponse">处理响应方法<seealso cref="object"/> (<seealso cref="string"/> response)</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest(string serviceName, Func<string, string, object[], object> constructBody, Func<string, object>? processResponse = null) => AddRequest(serviceName, new XFEClientInstanceRequest()
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
        _xFEClientRequester.ComputerInfo = options.ComputerInfo;
        _xFEClientRequester.AutoUnescapeResponse = options.AutoUnescapeResponse;
        _xFEClientRequester.RequestServiceDictionary = _requestServiceDictionary;
        _xFEClientRequester.XFERequestServiceDictionary = _xFERequestServiceDictionary;
        _xFEClientRequester.XFEClientInstanceRequestDictionary = _xFEClientInstanceRequestDictionary;
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
        _xFEClientRequester.RequestAddress = options.RequestAddress;
        _xFEClientRequester.Session = options.Session;
        _xFEClientRequester.ComputerInfo = options.ComputerInfo;
        _xFEClientRequester.AutoUnescapeResponse = options.AutoUnescapeResponse;
        _xFEClientRequester.RequestServiceDictionary = _requestServiceDictionary;
        _xFEClientRequester.XFERequestServiceDictionary = _xFERequestServiceDictionary;
        _xFEClientRequester.XFEClientInstanceRequestDictionary = _xFEClientInstanceRequestDictionary;
        return _xFEClientRequester;
    }
}

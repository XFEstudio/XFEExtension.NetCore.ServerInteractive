using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器构建器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequesterBuilder
{
    readonly Dictionary<string, object> parameterDictionary = [];
    readonly Dictionary<string, IRequestService> requestServiceDictionary = [];
    readonly Dictionary<string, IXFERequestService> xFERequestServiceDictionary = [];
    readonly Dictionary<string, XFEClientInstanceRequest> xFEClientInstanceRequestDictionary = [];
    /// <summary>
    /// 创建构建器
    /// </summary>
    /// <returns></returns>
    public static XFEClientRequesterBuilder CreateBuilder()
    {
        return new XFEClientRequesterBuilderImpl();
    }

    /// <summary>
    /// 添加请求
    /// </summary>
    /// <param name="serviceName">请求名称</param>
    /// <param name="requestService">请求服务对象</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddRequest(string serviceName, IRequestService requestService)
    {
        BuilderHelper.ApplyParameter(parameterDictionary, requestService);
        requestServiceDictionary.Add(serviceName, requestService);
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
        BuilderHelper.ApplyParameter(parameterDictionary, xFERequestService);
        xFERequestServiceDictionary.Add(serviceName, xFERequestService);
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
        xFEClientInstanceRequestDictionary.Add(serviceName, xFEClientInstanceRequest);
        return this;
    }

    /// <summary>
    /// 添加实例请求
    /// </summary>
    /// <param name="serviceName">请求服务名称</param>
    /// <param name="constructBody">构造请求体方法<seealso cref="object"/> (<seealso cref="string"/> session, <seealso cref="string"/> session, <seealso cref="object"/>[] parameters)</param>
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
    /// <param name="requestAddress">请求地址</param>
    /// <param name="session">Session</param>
    /// <param name="computerInfo">电脑信息</param>
    /// <returns>XFE客户端请求器</returns>
    public XFEClientRequester Build(string requestAddress = "http://localhost:8080/", string session = "", string computerInfo = "")
    {
        var requester = new XFEClientRequesterImpl
        {
            RequestAddress = requestAddress,
            Session = session,
            ComputerInfo = computerInfo,
            requestServiceDictionary = requestServiceDictionary,
            xFERequestServiceDictionary = xFERequestServiceDictionary,
            xFEClientInstanceRequestDictionary = xFEClientInstanceRequestDictionary
        };
        return requester;
    }
}

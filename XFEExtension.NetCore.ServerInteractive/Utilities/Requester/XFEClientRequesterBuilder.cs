using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.Requester;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

/// <summary>
/// XFE客户端请求器构建器
/// </summary>
[CreateImpl]
public abstract class XFEClientRequesterBuilder
{
    readonly Dictionary<string, object> parameterDictionary = [];
    /// <summary>
    /// 创建构建器
    /// </summary>
    /// <returns></returns>
    public static XFEClientRequesterBuilder CreateBuilder() => new XFEClientRequesterBuilderImpl();

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="requestService">请求服务对象</param>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddService(IRequestService requestService)
    {
        BuilderHelper.ApplyParameter(parameterDictionary, requestService);
        return this;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <returns></returns>
    public XFEClientRequesterBuilder AddService<T>() where T : IRequestService, new() => AddService(new T());
}

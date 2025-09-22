using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心构建器
/// </summary>
[CreateImpl]
public abstract class XFEServerCoreBuilder
{
    readonly XFEServerCore xFEServerCore = new XFEServerCoreImpl();
    readonly List<IServerCoreRegisterService> serverCoreServiceList = [];
    readonly List<IServerCoreVerifyService> serverCoreVerifyServiceList = [];
    readonly List<IServerCoreVerifyAsyncService> serverCoreVerifyAsyncServiceList = [];
    readonly Dictionary<string, IServerCoreStandardRegisterService> serverStandardCoreServiceDictionary = [];
    readonly Dictionary<string, IServerCoreStandardRegisterAsyncService> serverStandardCoreAsyncServiceDictionary = [];
    readonly Dictionary<List<string>, IServerCoreStandardRegisterService> serverMultiStandardCoreServiceDictionary = [];
    readonly Dictionary<List<string>, IServerCoreStandardRegisterAsyncService> serverMultiStandardCoreAsyncServiceDictionary = [];
    readonly Dictionary<string, object> serviceParameterCacheDictionary = [];

    /// <summary>
    /// 创建XFE服务器核心构建器
    /// </summary>
    /// <returns></returns>
    public static XFEServerCoreBuilder CreateBuilder()
    {
        var builder = new XFEServerCoreBuilderImpl();
        builder.serviceParameterCacheDictionary.Add("XFEServerCore", builder.xFEServerCore);
        return builder;
    }

    /// <summary>
    /// 添加参数列表
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="value">数值</param>
    /// <returns></returns>
    public XFEServerCoreBuilder AddParameter(string name, object value)
    {
        serviceParameterCacheDictionary.Add(name, value);
        return this;
    }

    /// <summary>
    /// 添加参数列表
    /// </summary>
    /// <param name="keyValuePairList"></param>
    /// <returns></returns>
    public XFEServerCoreBuilder AddParameterList(params KeyValuePair<string, object>[] keyValuePairList)
    {
        foreach (var keyValuePair in keyValuePairList)
            serviceParameterCacheDictionary.Add(keyValuePair.Key, keyValuePair.Value);
        return this;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="serverCoreRegisterService">服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddService(IServerCoreRegisterService serverCoreRegisterService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreRegisterService);
        serverCoreServiceList.Add(serverCoreRegisterService);
        return this;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddService<T>() where T : IServerCoreRegisterService, new() => AddService(new T());

    /// <summary>
    /// 添加校验服务
    /// </summary>
    /// <param name="serverCoreVerifyService">校验服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddVerifyService(IServerCoreVerifyService serverCoreVerifyService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreVerifyService);
        serverCoreVerifyServiceList.Add(serverCoreVerifyService);
        return this;
    }

    /// <summary>
    /// 添加校验服务
    /// </summary>
    /// <typeparam name="T">校验服务泛型</typeparam>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddVerifyService<T>() where T : IServerCoreVerifyService, new() => AddVerifyService(new T());

    /// <summary>
    /// 添加校验服务
    /// </summary>
    /// <param name="serverCoreVerifyAsyncService">校验服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddVerifyAsyncService(IServerCoreVerifyAsyncService serverCoreVerifyAsyncService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreVerifyAsyncService);
        serverCoreVerifyAsyncServiceList.Add(serverCoreVerifyAsyncService);
        return this;
    }

    /// <summary>
    /// 添加校验服务
    /// </summary>
    /// <typeparam name="T">校验服务泛型</typeparam>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddVerifyAsyncService<T>() where T : IServerCoreVerifyAsyncService, new() => AddVerifyAsyncService(new T());

    /// <summary>
    /// 注册标准服务
    /// </summary>
    /// <param name="execute">注册执行语句</param>
    /// <param name="serverCoreStandardRegisterService">标准服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardService(string execute, IServerCoreStandardRegisterService serverCoreStandardRegisterService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreStandardRegisterService);
        serverStandardCoreServiceDictionary.Add(execute, serverCoreStandardRegisterService);
        return this;
    }

    /// <summary>
    /// 注册标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="execute">注册执行语句</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardService<T>(string execute) where T : IServerCoreStandardRegisterService, new() => RegisterStandardService(execute, new T());

    /// <summary>
    /// 注册标准服务
    /// </summary>
    /// <param name="execute">注册执行语句</param>
    /// <param name="serverCoreStandardAsyncRegisterService">标准服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardAsyncService(string execute, IServerCoreStandardRegisterAsyncService serverCoreStandardAsyncRegisterService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreStandardAsyncRegisterService);
        serverStandardCoreAsyncServiceDictionary.Add(execute, serverCoreStandardAsyncRegisterService);
        return this;
    }

    /// <summary>
    /// 注册标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="execute">注册执行语句</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardAsyncService<T>(string execute) where T : IServerCoreStandardRegisterAsyncService, new() => RegisterStandardAsyncService(execute, new T());

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <param name="executeList">执行语句列表</param>
    /// <param name="serverCoreStandardRegisterService">标准服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardService(List<string> executeList, IServerCoreStandardRegisterService serverCoreStandardRegisterService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreStandardRegisterService);
        serverMultiStandardCoreServiceDictionary.Add(executeList, serverCoreStandardRegisterService);
        return this;
    }

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="executeList">执行语句列表</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardService<T>(List<string> executeList) where T : IServerCoreStandardRegisterService, new() => RegisterStandardService(executeList, new T());

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <param name="executeList">执行语句列表</param>
    /// <param name="serverCoreStandardRegisterAsyncService">标准服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardAsyncService(List<string> executeList, IServerCoreStandardRegisterAsyncService serverCoreStandardRegisterAsyncService)
    {
        BuilderHelper.ApplyParameter(serviceParameterCacheDictionary, serverCoreStandardRegisterAsyncService);
        serverMultiStandardCoreAsyncServiceDictionary.Add(executeList, serverCoreStandardRegisterAsyncService);
        return this;
    }

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="executeList">执行语句列表</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder RegisterStandardAsyncService<T>(List<string> executeList) where T : IServerCoreStandardRegisterAsyncService, new() => RegisterStandardAsyncService(executeList, new T());

    /// <summary>
    /// 构建XFE服务器核心
    /// </summary>
    /// <returns>XFE服务器核心</returns>
    public XFEServerCore Build(string? name = "")
    {
        foreach (var serverCoreService in serverCoreServiceList)
        {
            xFEServerCore.serverCoreServiceList.Add(serverCoreService);
            xFEServerCore.CyberCommServer.ServerStarted += serverCoreService.ServerStarted;
        }
        xFEServerCore.serverCoreVerifyServiceList = serverCoreVerifyServiceList;
        xFEServerCore.serverCoreVerifyAsyncServiceList = serverCoreVerifyAsyncServiceList;
        xFEServerCore.standardCoreServiceDictionary = serverStandardCoreServiceDictionary;
        xFEServerCore.standardCoreAsyncServiceDictionary = serverStandardCoreAsyncServiceDictionary;
        xFEServerCore.standardMultiCoreServiceDictionary = serverMultiStandardCoreServiceDictionary;
        xFEServerCore.standardMultiCoreAsyncServiceDictionary = serverMultiStandardCoreAsyncServiceDictionary;
        if (!name.IsNullOrEmpty())
            xFEServerCore.CoreServerName = name;
        return xFEServerCore;
    }
}

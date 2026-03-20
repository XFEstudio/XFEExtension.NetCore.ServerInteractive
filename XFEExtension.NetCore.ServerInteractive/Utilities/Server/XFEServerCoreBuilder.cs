using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Implements;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Options;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心构建器
/// </summary>
[CreateImpl]
public abstract class XFEServerCoreBuilder : XFEBuilderBase<XFEServerCoreBuilder>
{
    static int serverCount = 1;
    readonly XFEServerCore xFEServerCore = new XFEServerCoreImpl();
    readonly List<IServerCoreOriginalService> serverCoreServiceList = [];
    readonly List<IServerCoreVerifyService> serverCoreVerifyServiceList = [];
    readonly List<IServerCoreVerifyAsyncService> serverCoreVerifyAsyncServiceList = [];
    readonly Dictionary<string, Func<IServerCoreStandardService>> serverStandardCoreServiceDictionary = [];
    readonly Dictionary<List<string>, Func<IServerCoreStandardService>> serverMultiStandardCoreServiceDictionary = [];

    /// <summary>
    /// 创建XFE服务器核心构建器
    /// </summary>
    /// <returns></returns>
    public static XFEServerCoreBuilder CreateBuilder()
    {
        var builder = new XFEServerCoreBuilderImpl();
        builder.AddParameter("XFEServerCore", builder.xFEServerCore);
        return builder;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="serverCoreRegisterService">服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddService(IServerCoreOriginalService serverCoreRegisterService)
    {
        ApplyParameter(serverCoreRegisterService);
        serverCoreServiceList.Add(serverCoreRegisterService);
        return this;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddService<T>() where T : IServerCoreOriginalService, new() => AddService(new T());

    /// <summary>
    /// 添加校验服务
    /// </summary>
    /// <param name="serverCoreVerifyService">校验服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddVerifyService(IServerCoreVerifyService serverCoreVerifyService)
    {
        ApplyParameter(serverCoreVerifyService);
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
        ApplyParameter(serverCoreVerifyAsyncService);
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
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="execute">注册执行语句</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddStandardService<T>(string execute) where T : IServerCoreStandardService, new()
    {
        serverStandardCoreServiceDictionary.Add(execute, () =>
        {
            var inst = new T();
            ApplyParameter(inst);
            return inst;
        });
        return this;
    }

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <param name="executeList">执行语句列表</param>
    /// <param name="serverCoreStandardRegisterService">标准服务对象</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddStandardService(List<string> executeList, IServerCoreStandardService serverCoreStandardRegisterService)
    {
        var type = serverCoreStandardRegisterService.GetType();
        serverMultiStandardCoreServiceDictionary.Add(executeList, () =>
        {
            var inst = (IServerCoreStandardService)Activator.CreateInstance(type)!;
            ApplyParameter(inst);
            return inst;
        });
        return this;
    }

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="executeList">执行语句列表</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddStandardService<T>(List<string> executeList) where T : IServerCoreStandardService, new() => AddStandardService(executeList, new T());

    /// <summary>
    /// 构建XFE服务器核心
    /// </summary>
    /// <returns>XFE服务器核心</returns>
    public XFEServerCore Build(XFEServerCoreOptions? xFEServerCoreOptions = null)
    {
        foreach (var serverCoreService in serverCoreServiceList)
        {
            xFEServerCore.serverCoreServiceList.Add(serverCoreService);
            xFEServerCore.CyberCommServer.ServerStarted += serverCoreService.ServerStarted;
        }
        xFEServerCore.serverCoreVerifyServiceList = serverCoreVerifyServiceList;
        xFEServerCore.serverCoreVerifyAsyncServiceList = serverCoreVerifyAsyncServiceList;
        xFEServerCore.standardCoreServiceDictionary = serverStandardCoreServiceDictionary;
        xFEServerCore.standardMultiCoreServiceDictionary = serverMultiStandardCoreServiceDictionary;

        if (xFEServerCoreOptions is not null)
        {
            xFEServerCore.ServerCoreName = xFEServerCoreOptions.ServerCoreName;
            xFEServerCore.AutoUnescapeJson = xFEServerCoreOptions.AutoUnescapeJson;
            xFEServerCore.AcceptNonStandardJson = xFEServerCoreOptions.AcceptNonStandardJson;
            xFEServerCore.GetIpFunction = xFEServerCoreOptions.GetIpFunction;
            if(!xFEServerCoreOptions.BindingIPAddress.IsNullOrEmpty())
                xFEServerCore.BindingIPAddress = xFEServerCoreOptions.BindingIPAddress;
        }

        if (xFEServerCore.ServerCoreName.IsNullOrEmpty())
            xFEServerCore.ServerCoreName = $"XFE服务器-{serverCount++}";
        return xFEServerCore;
    }

    /// <summary>
    /// 构建XFE服务器核心
    /// </summary>
    /// <returns>XFE服务器核心</returns>
    public XFEServerCore Build(Action<XFEServerCoreOptions> optionsBuilder)
    {
        var xFEServerCoreOptions = new XFEServerCoreOptions();
        optionsBuilder(xFEServerCoreOptions);
        return Build(xFEServerCoreOptions);
    }
}

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
    private static int _serverCount = 1;
    private readonly XFEServerCore _xFEServerCore = new XFEServerCoreImpl();
    private readonly List<IServerCoreOriginalService> _serverCoreServiceList = [];
    private readonly List<IServerCoreVerifyService> _serverCoreVerifyServiceList = [];
    private readonly List<IServerCoreVerifyAsyncService> _serverCoreVerifyAsyncServiceList = [];
    private readonly Dictionary<string, Func<IServerCoreStandardService>> _serverStandardCoreServiceDictionary = [];
    private readonly Dictionary<List<string>, Func<IServerCoreStandardService>> _serverMultiStandardCoreServiceDictionary = [];

    /// <summary>
    /// 创建XFE服务器核心构建器
    /// </summary>
    /// <returns></returns>
    public static XFEServerCoreBuilder CreateBuilder()
    {
        var builder = new XFEServerCoreBuilderImpl();
        builder.AddParameter("XFEServerCore", builder._xFEServerCore);
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
        _serverCoreServiceList.Add(serverCoreRegisterService);
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
        _serverCoreVerifyServiceList.Add(serverCoreVerifyService);
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
        _serverCoreVerifyAsyncServiceList.Add(serverCoreVerifyAsyncService);
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
        _serverStandardCoreServiceDictionary.Add(execute, () =>
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
        _serverMultiStandardCoreServiceDictionary.Add(executeList, () =>
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
        foreach (var serverCoreService in _serverCoreServiceList)
        {
            _xFEServerCore.ServerCoreServiceList.Add(serverCoreService);
            _xFEServerCore.CyberCommServer.ServerStarted += serverCoreService.ServerStarted;
        }
        _xFEServerCore.ServerCoreVerifyServiceList = _serverCoreVerifyServiceList;
        _xFEServerCore.ServerCoreVerifyAsyncServiceList = _serverCoreVerifyAsyncServiceList;
        _xFEServerCore.StandardCoreServiceDictionary = _serverStandardCoreServiceDictionary;
        _xFEServerCore.StandardMultiCoreServiceDictionary = _serverMultiStandardCoreServiceDictionary;

        if (xFEServerCoreOptions is not null)
        {
            _xFEServerCore.ServerCoreName = xFEServerCoreOptions.ServerCoreName;
            _xFEServerCore.AutoUnescapeJson = xFEServerCoreOptions.AutoUnescapeJson;
            _xFEServerCore.AcceptNonStandardJson = xFEServerCoreOptions.AcceptNonStandardJson;
            _xFEServerCore.GetIpFunction = xFEServerCoreOptions.GetIpFunction;
            if(!xFEServerCoreOptions.BindingIPAddress.IsNullOrEmpty())
                _xFEServerCore.BindingIPAddress = xFEServerCoreOptions.BindingIPAddress;
        }

        if (_xFEServerCore.ServerCoreName.IsNullOrEmpty())
            _xFEServerCore.ServerCoreName = $"XFE服务器-{_serverCount++}";
        return _xFEServerCore;
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

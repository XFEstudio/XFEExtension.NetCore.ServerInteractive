using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Implements;
using XFEExtension.NetCore.ServerInteractive.Interfaces.CoreService;
using XFEExtension.NetCore.ServerInteractive.Options;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心构建器
/// </summary>
[CreateImpl]
public abstract class XFEServerCoreBuilder : XFEBuilderBase<XFEServerCoreBuilder>
{
    private static int s_serverCount = 1;
    private readonly XFEServerCore _xFEServerCore = new XFEServerCoreImpl();
    private readonly List<IServerCoreOriginalService> _serverCoreServiceList = [];
    private readonly List<Func<IServerCoreVerifyService>> _serverCoreVerifyServiceList = [];
    private readonly Dictionary<string, Func<IServerCoreStandardService>> _serverStandardCoreServiceDictionary = [];
    private readonly List<(string Pattern, Func<IServerCoreStandardService> Factory)> _serverWildcardCoreServiceList = [];

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
    public XFEServerCoreBuilder AddOriginalService(IServerCoreOriginalService serverCoreRegisterService)
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
    public XFEServerCoreBuilder AddOriginalService<T>() where T : IServerCoreOriginalService, new() => AddOriginalService(new T());

    /// <summary>
    /// 添加校验服务
    /// </summary>
    /// <typeparam name="T">校验服务泛型</typeparam>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddVerifyService<T>() where T : IServerCoreVerifyService, new()
    {
        _serverCoreVerifyServiceList.Add(() =>
        {
            var inst = new T();
            ApplyParameter(inst);
            return inst;
        });
        return this;
    }

    /// <summary>
    /// 注册标准服务（手动指定路由路径，适用于动态路由的服务）
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="route">路由路径</param>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddServiceWithRoute<T>(string route) where T : IServerCoreStandardService, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(route, nameof(route));
        route = route.Trim('/');

        _serverStandardCoreServiceDictionary.Add(route, () =>
        {
            var inst = new T();
            ApplyParameter(inst);
            return inst;
        });
        return this;
    }

    /// <summary>
    /// 注册标准服务（从EntryPointList自动获取路由路径）
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <returns>XFE服务器核心构建器</returns>
    public XFEServerCoreBuilder AddService<T>() where T : IServerCoreStandardService, new()
    {
        var probeService = new T();
        ApplyParameter(probeService);

        var entryPointList = probeService.SyncEntryPoints.Keys
            .Concat(probeService.AsyncEntryPoints.Keys)
            .Distinct()
            .ToList();

        if (entryPointList is null || entryPointList.Count == 0)
            throw new InvalidOperationException($"类型 {typeof(T).Name} 的 EntryPointList 为空");

        // 为每个入口点注册服务工厂（区分精确路由和通配符路由）
        foreach (var route in entryPointList)
        {
            Func<IServerCoreStandardService> factory = () =>
            {
                var inst = new T();
                ApplyParameter(inst);
                return inst;
            };

            if (RouteMatchHelper.IsWildcardRoute(route))
            {
                _serverWildcardCoreServiceList.Add((route, factory));
            }
            else
            {
                _serverStandardCoreServiceDictionary.Add(route, factory);
            }
        }
        return this;
    }

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
        _xFEServerCore.StandardCoreServiceDictionary = _serverStandardCoreServiceDictionary;
        _xFEServerCore.WildcardCoreServiceList = _serverWildcardCoreServiceList;

        if (xFEServerCoreOptions is not null)
        {
            _xFEServerCore.ServerCoreName = xFEServerCoreOptions.ServerCoreName;
            _xFEServerCore.AutoUnescapeJson = xFEServerCoreOptions.AutoUnescapeJson;
            _xFEServerCore.AcceptNonStandardJson = xFEServerCoreOptions.AcceptNonStandardJson;
            _xFEServerCore.GetIPFunction = xFEServerCoreOptions.GetIPFunction;
            _xFEServerCore.MainEntryPoint = xFEServerCoreOptions.MainEntryPoint;
            if (xFEServerCoreOptions.BindingIPAddress.Count > 0)
                _xFEServerCore.BindingIPAddressList = xFEServerCoreOptions.BindingIPAddress;
        }

        if (_xFEServerCore.ServerCoreName.IsNullOrEmpty())
            _xFEServerCore.ServerCoreName = $"XFE服务器-{s_serverCount++}";
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

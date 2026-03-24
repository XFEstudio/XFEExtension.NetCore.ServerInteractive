using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Implements;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器构建器
/// </summary>
[CreateImpl]
public abstract class XFEServerBuilder : XFEBuilderBase<XFEServerBuilder>
{
    private bool _addedProcessServiced = false;
    private IServerCoreProcessService? _serverCoreProcess;
    private readonly List<IServerCoreService> _serverCoreServiceList = [];

    /// <summary>
    /// 服务器实例
    /// </summary>
    public XFEServer XFEServer { get; init; } = new XFEServerImpl();

    /// <summary>
    /// 创建构建器
    /// </summary>
    /// <returns>XFE服务器构建器</returns>
    public static XFEServerBuilder CreateBuilder() => new XFEServerBuilderImpl();

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <param name="serverService">服务对象</param>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddService(IServerService serverService)
    {
        serverService.XFEServer = XFEServer;
        XFEServer.ServerServiceList.Add(serverService);
        return this;
    }

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddService<T>() where T : IServerService, new() => AddService(new T());

    /// <summary>
    /// 添加异步服务
    /// </summary>
    /// <param name="asyncServerService">异步服务对象</param>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddAsyncService(IAsyncServerService asyncServerService)
    {
        asyncServerService.XFEServer = XFEServer;
        XFEServer.AsyncServerServiceList.Add(asyncServerService);
        return this;
    }

    /// <summary>
    /// 添加异步服务
    /// </summary>
    /// <typeparam name="T">异步服务泛型</typeparam>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddAsyncService<T>() where T : IAsyncServerService, new() => AddAsyncService(new T());

    /// <summary>
    /// 添加初始化服务
    /// </summary>
    /// <param name="serverInitializerService">初始化服务对象</param>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddInitializer(IServerInitializerService serverInitializerService)
    {
        serverInitializerService.XFEServer = XFEServer;
        XFEServer.ServerInitializerServiceList.Add(serverInitializerService);
        return this;
    }

    /// <summary>
    /// 添加初始化服务
    /// </summary>
    /// <typeparam name="T">初始化服务泛型</typeparam>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddInitializer<T>() where T : IServerInitializerService, new() => AddInitializer(new T());

    /// <summary>
    /// 添加核心服务
    /// </summary>
    /// <param name="serverCoreService">核心服务对象</param>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddServerCore(IServerCoreService serverCoreService)
    {
        serverCoreService.XFEServer = XFEServer;
        _serverCoreServiceList.Add(serverCoreService);
        return this;
    }

    /// <summary>
    /// 添加核心服务
    /// </summary>
    /// <typeparam name="T">核心服务泛型</typeparam>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddServerCore<T>() where T : IServerCoreService, new() => AddServerCore(new T());

    /// <summary>
    /// 添加核心服务处理程序
    /// </summary>
    /// <param name="serverCoreProcessService">核心服务处理程序对象</param>
    /// <returns>XFE服务器构建器</returns>
    /// <exception cref="XFEServerBuilderException">XFE服务器构建器异常</exception>
    public XFEServerBuilder AddCoreProcessor(IServerCoreProcessService serverCoreProcessService)
    {
        if (_addedProcessServiced)
            throw new XFEServerBuilderException("已经添加过处理器，处理器仅能添加一个！");
        if (serverCoreProcessService is null)
            throw new XFEServerBuilderException("处理器为空", new ArgumentNullException(nameof(serverCoreProcessService)));
        _addedProcessServiced = true;
        _serverCoreProcess = serverCoreProcessService;
        return this;
    }

    /// <summary>
    /// 添加核心服务处理程序
    /// </summary>
    /// <typeparam name="T">核心服务处理程序泛型</typeparam>
    /// <returns>XFE服务器构建器</returns>
    public XFEServerBuilder AddCoreProcessor<T>() where T : IServerCoreProcessService, new() => AddCoreProcessor(new T());

    /// <summary>
    /// 构建XFE服务器
    /// </summary>
    /// <returns>XFE服务器</returns>
    /// <exception cref="XFEServerBuilderException">未添加核心处理器异常</exception>
    public XFEServer Build()
    {
        if (!_addedProcessServiced || _serverCoreProcess is null)
            throw new XFEServerBuilderException("未添加核心处理器！");
        _serverCoreProcess.ServerCoreServiceList = _serverCoreServiceList;
        XFEServer.ServerCoreProcessService = _serverCoreProcess;
        return XFEServer;
    }
}

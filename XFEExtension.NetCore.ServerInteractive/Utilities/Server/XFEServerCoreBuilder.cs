using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces.ServerCoreService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server;

/// <summary>
/// XFE服务器核心构建器
/// </summary>
[CreateImpl]
public abstract class XFEServerCoreBuilder
{
    readonly XFEServerCore xFEServerCore = new XFEServerCoreImpl();
    readonly List<IServerCoreService> serverCoreServiceList = [];
    readonly Dictionary<string, IServerStandardCoreService> serverStandardCoreServiceDictionary = [];
    readonly Dictionary<List<string>, IServerStandardCoreService> serverMultiStandardCoreServiceDictionary = [];

    /// <summary>
    /// 创建XFE服务器核心构建器
    /// </summary>
    /// <returns></returns>
    public static XFEServerCoreBuilder CreateBuilder() => new XFEServerCoreBuilderImpl();

    /// <summary>
    /// 添加服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <returns>构建器</returns>
    public XFEServerCoreBuilder AddService<T>() where T : IServerCoreService, new()
    {
        serverCoreServiceList.Add(new T()
        {
            XFEServerCore = xFEServerCore
        });
        return this;
    }

    /// <summary>
    /// 注册标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="execute">注册执行语句</param>
    /// <returns></returns>
    public XFEServerCoreBuilder RegisterStandardService<T>(string execute) where T : IServerStandardCoreService, new()
    {
        serverStandardCoreServiceDictionary.Add(execute, new T()
        {
            XFEServerCore = xFEServerCore
        });
        return this;
    }

    /// <summary>
    /// 注册多个执行语句的标准服务
    /// </summary>
    /// <typeparam name="T">服务泛型</typeparam>
    /// <param name="executeList">执行语句列表</param>
    /// <returns></returns>
    public XFEServerCoreBuilder RegisterStandardService<T>(List<string> executeList) where T : IServerStandardCoreService, new()
    {
        serverMultiStandardCoreServiceDictionary.Add(executeList, new T()
        {
            XFEServerCore = xFEServerCore
        });
        return this;
    }

    /// <summary>
    /// 构建XFE服务器核心
    /// </summary>
    /// <returns></returns>
    public XFEServerCore Build()
    {
        foreach (var serverCoreService in serverCoreServiceList)
        {
            xFEServerCore.ServerCoreServiceList.Add(serverCoreService);
            xFEServerCore.CyberCommServer.ServerStarted += serverCoreService.ServerStarted;
        }
        xFEServerCore.StandardCoreServiceDictionary = serverStandardCoreServiceDictionary;
        xFEServerCore.StandardMultiCoreServiceDictionary = serverMultiStandardCoreServiceDictionary;
        return xFEServerCore;
    }
}

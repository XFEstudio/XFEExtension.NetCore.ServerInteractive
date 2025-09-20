using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.ServerInteractive.Utilities.Services.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities;

/// <summary>
/// XFE服务器构建器扩展
/// </summary>
public static class XFEServerBuilderExtensions
{
    /// <summary>
    /// 使用XFE日志记录
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder UseXFELog(this XFEServerBuilder xFEServerBuilder)
    {
        xFEServerBuilder.AddInitializer<ServerLogInitializer>();
        return xFEServerBuilder;
    }

    /// <summary>
    /// 使用XFE异常处理程序
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder UseXFEExceptionProcess(this XFEServerBuilder xFEServerBuilder)
    {
        xFEServerBuilder.AddService<ServerExceptionProcessService>();
        return xFEServerBuilder;
    }

    /// <summary>
    /// 使用XFE核心处理程序
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder UseXFECoreProcessor(this XFEServerBuilder xFEServerBuilder)
    {
        xFEServerBuilder.AddCoreProcessor<XFECoreServerProcessService>();
        return xFEServerBuilder;
    }

    /// <summary>
    /// 使用XFEIP初始化器
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder UseXFEIpInitializer(this XFEServerBuilder xFEServerBuilder)
    {
        xFEServerBuilder.AddInitializer<ServerIpInitializer>();
        return xFEServerBuilder;
    }
}

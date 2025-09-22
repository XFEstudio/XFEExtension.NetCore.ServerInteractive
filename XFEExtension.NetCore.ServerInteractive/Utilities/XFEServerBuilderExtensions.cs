using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

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
    public static XFEServerBuilder UseXFEServer(this XFEServerBuilder xFEServerBuilder) => xFEServerBuilder.UseXFELog()
            .AddXFEExceptionProcess()
            .AddXFEIpInitializer()
            .AddXFECoreProcessor();
    /// <summary>
    /// 使用XFE日志记录
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder UseXFELog(this XFEServerBuilder xFEServerBuilder) => xFEServerBuilder.AddInitializer<ServerLogInitializer>();

    /// <summary>
    /// 使用XFE异常处理程序
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder AddXFEExceptionProcess(this XFEServerBuilder xFEServerBuilder) => xFEServerBuilder.AddService<ServerExceptionProcessService>();

    /// <summary>
    /// 使用XFE核心处理程序
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder AddXFECoreProcessor(this XFEServerBuilder xFEServerBuilder) => xFEServerBuilder.AddCoreProcessor<XFECoreServerProcessService>();

    /// <summary>
    /// 使用XFEIP初始化器
    /// </summary>
    /// <param name="xFEServerBuilder"></param>
    /// <returns></returns>
    public static XFEServerBuilder AddXFEIpInitializer(this XFEServerBuilder xFEServerBuilder) => xFEServerBuilder.AddInitializer<ServerIpInitializer>();
}

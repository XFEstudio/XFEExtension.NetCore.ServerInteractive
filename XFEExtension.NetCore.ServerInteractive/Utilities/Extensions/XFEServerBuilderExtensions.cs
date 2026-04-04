using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.ServerService;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;

/// <summary>
/// XFE服务器构建器扩展
/// </summary>
public static class XFEServerBuilderExtensions
{
    /// <param name="xFEServerBuilder"></param>
    extension(XFEServerBuilder xFEServerBuilder)
    {
        /// <summary>
        /// 使用XFE标准服务器
        /// </summary>
        /// <returns></returns>
        public XFEServerBuilder UseXFEServer() => xFEServerBuilder.UseXFELog()
            .AddXFEExceptionProcess()
            .AddXFECoreProcessor();

        /// <summary>
        /// 使用XFE日志记录
        /// </summary>
        /// <returns></returns>
        public XFEServerBuilder UseXFELog() => xFEServerBuilder.AddInitializer<ServerLogInitializer>();

        /// <summary>
        /// 使用XFE异常处理程序
        /// </summary>
        /// <returns></returns>
        public XFEServerBuilder AddXFEExceptionProcess() => xFEServerBuilder.AddService<ServerExceptionProcessService>();

        /// <summary>
        /// 使用XFE核心处理程序
        /// </summary>
        /// <returns></returns>
        public XFEServerBuilder AddXFECoreProcessor() => xFEServerBuilder.AddCoreProcessor<XFEServerCoreProcessService>();

        /// <summary>
        /// 使用XFEIP初始化器
        /// </summary>
        /// <returns></returns>
        [Obsolete("核心服务器现已不再使用单一IP，故IP初始化器不再提供", DiagnosticId = "XFW0002", UrlFormat = "https://docs.xfegzs.com/View/Errors%2FServerInteractive%2FXFW0002")]
        public XFEServerBuilder AddXFEIPInitializer() => xFEServerBuilder.AddInitializer<ServerIPInitializer>();
    }
}

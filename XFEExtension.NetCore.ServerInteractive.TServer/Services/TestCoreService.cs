using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Services;

public partial class TestCoreService : ServerCoreUserServiceBase
{
    [EntryPoint("v1/test/*")]
    public async Task TestEntryPoint() => await Close($"""
                                                       Hello from {XFEServerCore.ServerCoreName}：
                                                           Your route is {Route};
                                                           Your IP is {ClientIP};
                                                           Your headers are {string.Join(',', Args.Request.Headers)}
                                                       """);
}

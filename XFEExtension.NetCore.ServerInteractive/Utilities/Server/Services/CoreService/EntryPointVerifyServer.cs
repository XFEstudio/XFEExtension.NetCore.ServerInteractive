using System.Net;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Profiles;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 服务器入口点校验
/// </summary>
public class EntryPointVerifyServer : ServerCoreVerifyServiceBase
{
    /// <inheritdoc/>
    public override bool VerifyRequest(object? sender, CyberCommRequestEventArgs e, ServerCoreReturnArgs r)
    {
        Console.Write($"[DEBUG]【{e.ClientIP}】接收到请求");
        if (ServerBaseProfile.BannedIPAddressList.Contains(e.ClientIP)) throw new StopAction(() =>
        {
            Console.WriteLine("-校验失败");
            r.Close(HttpStatusCode.Forbidden);
        }, "您的已被封禁");
        if (e.RequestURL?.Segments.Length > 2 && $"{e.RequestURL?.Segments[0]}{e.RequestURL?.Segments[1]}" != $"/{ServerBaseProfile.EntryPoint}" && e.RequestMethod != "POST") throw new StopAction(() =>
        {
            Console.WriteLine("-校验失败");
            r.Close(HttpStatusCode.BadGateway);
        }, "请求的API接口不正确");
        Console.WriteLine("-校验通过");
        return true;
    }
}

using System.Net;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Profiles;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// 服务器主入口点校验
/// </summary>
public class EntryPointVerifyService : ServerCoreVerifyServiceBase
{
    /// <inheritdoc/>
    public override bool VerifyRequest()
    {
        Console.Write($"[DEBUG]({XFEServerCore.ServerCoreName})【{ClientIP}】接收到请求");
        if (ServerBaseProfile.BannedIPAddressList.Contains(ClientIP))
        {
            Console.WriteLine("-校验失败");
            throw Error("您的IP已被封禁", HttpStatusCode.Forbidden);
        }

        if (!XFEServerCore.AcceptPost && Request.HttpMethod == "POST")
        {
            Console.WriteLine("-校验失败");
            throw Error("不接受为POST的请求方法", HttpStatusCode.MethodNotAllowed);
        }

        if (!XFEServerCore.AcceptGet && Request.HttpMethod == "GET")
        {
            Console.WriteLine("-校验失败");
            throw Error("不接受为GET的请求方法", HttpStatusCode.MethodNotAllowed);
        }

        Console.WriteLine("-校验通过");
        return true;
    }
}

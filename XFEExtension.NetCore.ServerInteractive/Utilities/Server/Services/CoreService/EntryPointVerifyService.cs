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
        Console.Write($"[INFO]({XFEServerCore.ServerCoreName})【{ClientIP}】接收到请求");
        if (ServerBaseProfile.BannedIPAddressList.Contains(ClientIP))
        {
            Console.WriteLine("-校验失败");
            throw Error("您的IP已被封禁", HttpStatusCode.Forbidden);
        }

        var method = Args.RequestMethod;
        var isAllowed = (XFEServerCore.AcceptGet && method == "GET") || (XFEServerCore.AcceptPost && method == "POST");
        if (!isAllowed)
        {
            Console.WriteLine("-校验失败");
            var allowedMethods = new List<string>();
            if (XFEServerCore.AcceptGet) allowedMethods.Add("GET");
            if (XFEServerCore.AcceptPost) allowedMethods.Add("POST");
            var allowedStr = allowedMethods.Count > 0 ? string.Join(", ", allowedMethods) : "无";
            throw Error($"不接受的请求方法 {method}，当前允许的方法：{allowedStr}", HttpStatusCode.MethodNotAllowed);
        }

        Console.WriteLine("-校验通过");
        return true;
    }
}

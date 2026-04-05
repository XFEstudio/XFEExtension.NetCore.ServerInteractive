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

        // 使用XFEServerCore的MainEntryPoint属性进行主入口点校验（如果配置了）
        if (!string.IsNullOrEmpty(XFEServerCore.MainEntryPoint))
        {
            if (Request.Url?.Segments.Length < 2 || $"{Request.Url?.Segments[0]}{Request.Url?.Segments[1]}".TrimEnd('/') != $"/{XFEServerCore.MainEntryPoint}" || Request.HttpMethod != "POST")
            {
                Console.WriteLine("-校验失败");
                throw Error("请求的API接口不正确", HttpStatusCode.BadGateway);
            }
        }
        else if (Request.HttpMethod != "POST")
        {
            Console.WriteLine("-校验失败");
            throw Error("请求方法必须为POST", HttpStatusCode.MethodNotAllowed);
        }

        Console.WriteLine("-校验通过");
        return true;
    }
}

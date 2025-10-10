using System.Net;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension.Json;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// IP封禁服务
/// </summary>
public class IpBannerService<T> : ServerCoreUserServiceBase<T> where T : IUserInfo
{
    /// <summary>
    /// 获取权限
    /// </summary>
    public int GetPermission { get; set; } = 0;
    /// <summary>
    /// 添加权限
    /// </summary>
    public int AddPermission { get; set; } = 0;
    /// <summary>
    /// 移除权限
    /// </summary>
    public int RemovePermission { get; set; } = 0;
    /// <inheritdoc/>
    public override async Task StandardRequestReceived(string execute, QueryableJsonNode queryableJsonNode, ServerCoreReturnArgs r)
    {
        switch (execute)
        {
            case "get_bannedIpList":
                Console.Write($"【{r.Args.ClientIP}】获取禁止的IP地址列表请求");
                UserHelper.ValidatePermission(queryableJsonNode["session"], queryableJsonNode["computerInfo"], r.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction().Cast<IUserInfo>(), r);
                await r.Args.ReplyAndClose(ServerBaseProfile.BannedIPAddressList.ToJson());
                break;
            case "add_bannedIp":
                Console.WriteLine($"【{r.Args.ClientIP}】添加禁止的IP地址请求 添加：{queryableJsonNode["bannedIp"]}");
                UserHelper.ValidatePermission(queryableJsonNode["session"], queryableJsonNode["computerInfo"], r.Args.ClientIP, AddPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction().Cast<IUserInfo>(), r);
                if (queryableJsonNode["bannedIp"] is null) r.Error("无IP地址传入", HttpStatusCode.BadRequest);
                ServerBaseProfile.BannedIPAddressList.Add(new()
                {
                    IPAddress = queryableJsonNode["bannedIp"],
                    Notes = queryableJsonNode["notes"]
                });
                r.Args.Close();
                break;
            case "remove_bannedIp":
                Console.WriteLine($"【{r.Args.ClientIP}】删除禁止的IP地址请求 移除：{queryableJsonNode["bannedIp"]}");
                UserHelper.ValidatePermission(queryableJsonNode["session"], queryableJsonNode["computerInfo"], r.Args.ClientIP, RemovePermission, GetEncryptedUserLoginModelFunction(), GetUserFunction().Cast<IUserInfo>(), r);
                var targetIp = ServerBaseProfile.BannedIPAddressList.FirstOrDefault(ip => ip.IPAddress == queryableJsonNode["bannedIp"].ToString()) ?? throw r.GetError("无IP地址传入", HttpStatusCode.BadRequest);
                await r.Args.ReplyAndClose(ServerBaseProfile.BannedIPAddressList.Remove(targetIp).ToString());
                break;
            default:
                break;
        }
    }
}

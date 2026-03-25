using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// IP封禁服务
/// </summary>
public class IpBannerService : ServerCoreUserServiceBase
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
    public override async Task RequestReceiveAsync()
    {
        switch (Execute)
        {
            case "get_bannedIpList":
                Console.Write("获取禁止的IP地址列表请求");
                UserHelper.ValidatePermission(Json?["session"]?.ToString(), Json?["computerInfo"]?.ToString(), ReturnArgs.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                await Close(ServerBaseProfile.BannedIpAddressList.ToJson());
                break;
            case "add_bannedIp":
                Console.Write($"添加禁止的IP地址请求 添加：{Json?["bannedIp"]}");
                UserHelper.ValidatePermission(Json?["session"]?.ToString(), Json?["computerInfo"]?.ToString(), ReturnArgs.Args.ClientIP, AddPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                if (Json?["bannedIp"] is null) throw Error("无IP地址传入");
                ServerBaseProfile.BannedIpAddressList.Add(new()
                {
                    IpAddress = Json?["bannedIp"]?.ToString() ?? string.Empty,
                    Notes = Json?["notes"]?.ToString()
                });
                OK();
                break;
            case "remove_bannedIp":
                Console.Write($"删除禁止的IP地址请求 移除：{Json?["bannedIp"]}");
                UserHelper.ValidatePermission(Json?["session"]?.ToString(), Json?["computerInfo"]?.ToString(), ReturnArgs.Args.ClientIP, RemovePermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
                var targetIp = ServerBaseProfile.BannedIpAddressList.FirstOrDefault(ip => ip.IpAddress == Json?["bannedIp"]?.ToString()) ?? throw Error("无IP地址传入");
                await Close(ServerBaseProfile.BannedIpAddressList.Remove(targetIp).ToString());
                break;
        }
    }
}

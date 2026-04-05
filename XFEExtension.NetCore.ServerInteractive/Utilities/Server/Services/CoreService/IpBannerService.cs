using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension.Json;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <summary>
/// IP封禁服务
/// </summary>
public partial class IPBannerService : ServerCoreUserServiceBase
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

    /// <summary>
    /// 获取禁止的IP地址列表
    /// </summary>
    [EntryPoint("ip/banned/get")]
    public async Task GetBannedIPList()
    {
        Console.Write("获取禁止的IP地址列表请求");
        UserHelper.ValidatePermission(Json?["session"]?.ToString(), Json?["deviceInfo"]?.ToString(), ReturnArgs.Args.ClientIP, GetPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
        await Close(ServerBaseProfile.BannedIPAddressList.ToJson());
    }

    /// <summary>
    /// 添加禁止的IP地址
    /// </summary>
    [EntryPoint("ip/banned/add")]
    public void AddBannedIP()
    {
        Console.Write($"添加禁止的IP地址请求 添加：{Json?["bannedIP"]}");
        UserHelper.ValidatePermission(Json?["session"]?.ToString(), Json?["deviceInfo"]?.ToString(), ReturnArgs.Args.ClientIP, AddPermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
        if (Json?["bannedIP"] is null) throw Error("无IP地址传入");
        ServerBaseProfile.BannedIPAddressList.Add(new()
        {
            IPAddress = Json?["bannedIP"]?.ToString() ?? string.Empty,
            Notes = Json?["notes"]?.ToString()
        });
        OK();
    }

    /// <summary>
    /// 移除禁止的IP地址
    /// </summary>
    [EntryPoint("ip/banned/remove")]
    public async Task RemoveBannedIP()
    {
        Console.Write($"删除禁止的IP地址请求 移除：{Json?["bannedIP"]}");
        UserHelper.ValidatePermission(Json?["session"]?.ToString(), Json?["deviceInfo"]?.ToString(), ReturnArgs.Args.ClientIP, RemovePermission, GetEncryptedUserLoginModelFunction(), GetUserFunction(), ReturnArgs);
        var targetIP = ServerBaseProfile.BannedIPAddressList.FirstOrDefault(ip => ip.IPAddress == Json?["bannedIP"]?.ToString()) ?? throw Error("无IP地址传入");
        await Close(ServerBaseProfile.BannedIPAddressList.Remove(targetIP).ToString());
    }
}

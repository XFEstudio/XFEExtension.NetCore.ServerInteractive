using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;
using XFEExtension.NetCore.StringExtension;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <inheritdoc cref="ServerCoreStandardServiceBase" />
public abstract class ServerCoreUserServiceBase : ServerCoreStandardServiceBase, IUserServiceBase
{
    /// <summary>
    /// 会话（自动校验）
    /// </summary>
    public string Session
    {
        get
        {
            field ??= Json > "session" ?? throw Error("会话无效");
            return field;
        }
    }
    /// <summary>
    /// 设备信息（自动校验）
    /// </summary>
    public string DeviceInfo
    {
        get
        {
            field ??= Json > "deviceInfo" ?? throw Error("缺少设备信息");
            return field;
        }
    }
    /// <summary>
    /// 本次请求用户（自动校验）
    /// </summary>
    public IUserInfo User
    {
        get
        {
            field ??= VerifyUserInfo();
            return field;
        }
    }
    /// <inheritdoc/>
    public Func<IEnumerable<IUserInfo>> GetUserFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Action<EncryptedUserLoginModel> AddEncryptedUserLoginModelFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Action<IUserInfo> AddUserFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Func<int> GetLoginKeepDays { get; set; } = () => 7;
    /// <inheritdoc/>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();
    /// <inheritdoc/>
    public Action<EncryptedUserLoginModel> RemoveEncryptedUserLoginModelFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    protected ServerCoreUserServiceBase() => JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());

    /// <summary>
    /// 校验用户信息（仅限标准请求）
    /// </summary>
    /// <returns></returns>
    protected IUserInfo VerifyUserInfo()
    {
        if (Session.NullOrWhiteSpace) throw Error("会话无效");
        if (DeviceInfo.NullOrWhiteSpace) throw Error("设备信息无效");
        var result = UserHelper.GetUser(Session, DeviceInfo, ClientIP, GetEncryptedUserLoginModelFunction(), GetUserFunction(), out var user);
        return user is null ? throw Error(UserHelper.OutPutResult(result)) : user;
    }
}

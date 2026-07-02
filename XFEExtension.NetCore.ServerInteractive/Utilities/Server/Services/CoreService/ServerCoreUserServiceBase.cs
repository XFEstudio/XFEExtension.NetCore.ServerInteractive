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
    IUserInfo? _user;

    /// <summary>
    /// 会话
    /// </summary>
    public string? Session { get => Json > "session"; }
    /// <summary>
    /// 设备信息
    /// </summary>
    public string? DeviceInfo { get => Json > "deviceInfo"; }
    /// <summary>
    /// 本次请求用户（自动校验）
    /// </summary>
    public IUserInfo? User
    {
        get
        {

            if (_user is null)
            {
                var (_, _, user) = VerifyUserInfo();
                _user = user;
            }
            return _user;
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
    protected (string session, string deviceInfo, IUserInfo user) VerifyUserInfo()
    {
        string session = Json > "session" ?? throw Error("用户未登录");
        string deviceInfo = Json > "deviceInfo" ?? throw Error("缺少电脑信息");
        if (session.NullOrWhiteSpace) throw Error("用户未登录");
        if (deviceInfo.NullOrWhiteSpace) throw Error("电脑信息不能为空");
        var result = UserHelper.GetUser(session, deviceInfo, ClientIP, GetEncryptedUserLoginModelFunction(), GetUserFunction(), out var user);
        return user is null ? throw Error(UserHelper.OutPutResult(result)) : (session, deviceInfo, user);
    }
}

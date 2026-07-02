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

    /// <inheritdoc/>
    public string? Session { get => Json > "session"; }
    /// <inheritdoc/>
    public string? DeviceInfo { get => Json > "deviceInfo"; }
    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected (string session, string deviceInfo, IUserInfo user) VerifyUserInfo()
    {
        string session = Json > "session" ?? throw Error("用户未登录");
        string deviceInfo = Json > "deviceInfo" ?? throw Error("缺少电脑信息");
        if (session.NullOrWhiteSpace) throw Error("用户未登录");
        if (deviceInfo.NullOrWhiteSpace) throw Error("电脑信息不能为空");
        var result = UserHelper.GetUser(session, deviceInfo, ClientIP, GetEncryptedUserLoginModelFunction(), GetUserFunction(), out var user);
        return user is null ? throw Error(UserHelper.OutPutResult(result)) : (session, deviceInfo, user);
    }

    (string session, string deviceInfo, IUserInfo user) IUserServiceBase.VerifyUserInfo()
    {
        return VerifyUserInfo();
    }
}

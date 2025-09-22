using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Server.Services.CoreService;

/// <inheritdoc/>
public abstract class ServerCoreUserServiceBase : ServerCoreStandardRegisterAsyncServiceBase, IUserServiceBase
{
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
    public ServerCoreUserServiceBase() => JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
}

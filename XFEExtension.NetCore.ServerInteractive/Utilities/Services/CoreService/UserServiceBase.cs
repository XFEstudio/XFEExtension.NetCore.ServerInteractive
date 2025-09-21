using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Services.CoreService;

/// <inheritdoc/>
public abstract class UserServiceBase : IUserServiceBase
{
    /// <inheritdoc/>
    public Func<IEnumerable<User>> GetUserFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Action<EncryptedUserLoginModel> AddEncryptedUserLoginModelFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Action<User> AddUserFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public Func<int> GetLoginKeepDays { get; set; } = () => 7;
    /// <inheritdoc/>
    public JsonSerializerOptions JsonSerializerOptions { get; set; } = new();

    /// <inheritdoc/>
    public UserServiceBase() => JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
}

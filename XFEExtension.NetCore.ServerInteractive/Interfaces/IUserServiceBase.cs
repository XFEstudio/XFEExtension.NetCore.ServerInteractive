using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;

namespace XFEExtension.NetCore.ServerInteractive.Interfaces;

/// <summary>
/// 用户服务基类
/// </summary>
public interface IUserServiceBase
{
    /// <summary>
    /// Json序列化设置
    /// </summary>
    JsonSerializerOptions JsonSerializerOptions { get; set; }
    /// <summary>
    /// 添加加密用户模型方法
    /// </summary>
    Action<EncryptedUserLoginModel> AddEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 移除加密用户模型方法
    /// </summary>
    Action<EncryptedUserLoginModel> RemoveEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 添加用户方法
    /// </summary>
    Action<IUserInfo> AddUserFunction { get; set; }
    /// <summary>
    /// 获取加密用户模型方法
    /// </summary>
    Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 获取登录持续天数
    /// </summary>
    Func<int> GetLoginKeepDays { get; set; }
    /// <summary>
    /// 获取用户方法
    /// </summary>
    Func<IEnumerable<IUserInfo>> GetUserFunction { get; set; }
}
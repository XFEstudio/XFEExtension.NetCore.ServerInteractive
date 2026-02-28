using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

namespace XFEExtension.NetCore.ServerInteractive.Utilities;

/// <summary>
/// XFE服务器核心服务选项
/// </summary>
public class XFEStandardServerCoreOptions<T> where T : class
{
    /// <summary>
    /// 获取用户函数
    /// </summary>
    public Func<IEnumerable<User>>? GetUserFunction { get; set; }
    /// <summary>
    /// 获取加密用户登录模型函数
    /// </summary>
    public Func<IEnumerable<EncryptedUserLoginModel>>? GetEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 添加加密用户登录模型函数
    /// </summary>
    public Action<EncryptedUserLoginModel>? AddEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 删除加密用户登录模型函数
    /// </summary>
    public Action<EncryptedUserLoginModel>? RemoveEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 获取登录保持天数函数
    /// </summary>
    public Func<int> GetLoginKeepDays { get; set; } = () => 7;
    /// <summary>
    /// 登录结果转换函数（将用户模型转换为要返回给客户端的登录结果模型）
    /// </summary>
    public Func<object, T>? LoginResultConvertFunction { get; set; }
    /// <summary>
    /// 数据表管理器构建器（用于构建数据表管理器，数据表管理器用于处理与数据表相关的请求）
    /// </summary>
    public XFEDataTableManagerBuilder? DataTableManagerBuilder { get; set; }
}

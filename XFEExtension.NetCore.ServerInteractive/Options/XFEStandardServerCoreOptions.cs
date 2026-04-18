using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

namespace XFEExtension.NetCore.ServerInteractive.Options;

/// <summary>
/// XFE服务器核心服务选项
/// </summary>
public class XFEStandardServerCoreOptions<T> where T : class
{
    /// <summary>
    /// 使用服务器入口点校验服务（默认为true，启用服务器入口点校验服务，服务器入口点校验服务用于校验请求是否来自合法的入口点）
    /// </summary>
    public bool UseEntryPointVerifyService { get; set; } = true;
    /// <summary>
    /// 使用日期统计服务（默认为true，启用日期统计服务，日期统计服务用于统计每天的请求数量等信息，可以用于监控服务器的使用情况）
    /// </summary>
    public bool UseDailyCounterService { get; set; } = true;
    /// <summary>
    /// 使用XFE异常处理服务（默认为true，启用XFE异常处理服务，XFE异常处理服务用于处理服务器核心服务中的异常，并将异常信息返回给客户端）
    /// </summary>
    public bool UseXFEErrorProcessService { get; set; } = true;
    /// <summary>
    /// 使用连接检查服务（默认为true，启用连接检查服务，连接检查服务用于检查服务器与客户端之间的连接是否正常，可以用于监控服务器的连接状态）
    /// </summary>
    public bool UseConnectService { get; set; } = true;
    /// <summary>
    /// 获取用户函数
    /// </summary>
    public Func<IEnumerable<User>>? GetUserFunction { get; set; }
    /// <summary>
    /// 添加用户函数
    /// </summary>
    public Action<IUserInfo>? AddUserFunction { get; set; }
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

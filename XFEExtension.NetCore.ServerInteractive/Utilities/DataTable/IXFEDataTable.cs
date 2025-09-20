using System.Net;
using System.Text.Json;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace SCCApplication.Core.Utilities.DataTable;

/// <summary>
/// XFE数据表格接口
/// </summary>
public interface IXFEDataTable
{
    /// <summary>
    /// 添加数据所需的最小权限
    /// </summary>
    int AddPermissionLevel { get; set; }
    /// <summary>
    /// 移除数据所需的最小权限
    /// </summary>
    int RemovePermissionLevel { get; set; }
    /// <summary>
    /// 更改数据所需的最小权限
    /// </summary>
    int ChangePermissionLevel { get; set; }
    /// <summary>
    /// 获取数据所需的最小权限
    /// </summary>
    int GetPermissionLevel { get; set; }
    /// <summary>
    /// 获取加密用户登录列表的方法
    /// </summary>
    Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; }
    /// <summary>
    /// 获取用户列表的方法
    /// </summary>
    Func<IEnumerable<User>> GetUsersFunction { get; set; }
    /// <summary>
    /// Json序列化选项
    /// </summary>
    JsonSerializerOptions? JsonSerializerOptions { get; set; }
    /// <summary>
    /// 表名称，用其中元素的名称命名即可（如：Order（订单））
    /// </summary>
    string TableName { get; set; }
    /// <summary>
    /// 请求时使用的表名称，开头小写（如：order）
    /// </summary>
    string TableNameInRequest { get; set; }
    /// <summary>
    /// 表的显示名称，命名规则参照<seealso cref="TableName"/>
    /// </summary>
    string TableShowName { get; set; }
    /// <summary>
    /// 执行一条语句
    /// </summary>
    /// <param name="execute">执行的操作</param>
    /// <param name="requestJsonNode">Json节点</param>
    /// <param name="e">服务器参数</param>
    /// <returns>状态码</returns>
    Task<HttpStatusCode> Execute(string execute, QueryableJsonNode requestJsonNode, CyberCommRequestEventArgs e);
}
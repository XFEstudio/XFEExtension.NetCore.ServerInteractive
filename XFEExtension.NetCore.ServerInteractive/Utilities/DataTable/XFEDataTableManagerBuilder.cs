using System.Text.Json;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;

namespace SCCApplication.Core.Utilities.DataTable;

[CreateImpl]
public abstract class XFEDataTableManagerBuilder
{
    readonly List<IXFEDataTable> dataTableList = [];

    /// <summary>
    /// 创建构造器
    /// </summary>
    /// <returns></returns>
    public static XFEDataTableManagerBuilder CreateBuilder() => new XFEDataTableManagerBuilderImpl();

    /// <summary>
    /// 添加数据表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="xFEDataTable">数据表</param>
    /// <returns></returns>
    public XFEDataTableManagerBuilder AddTable<T>(XFEDataTable<T> xFEDataTable) where T : IIDModel
    {
        dataTableList.Add(xFEDataTable);
        return this;
    }

    /// <summary>
    /// 添加数据表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="tabelShowName">表格数据的显示名称（如：订单、用户等）</param>
    /// <param name="addRole">增加数据所需的最小权限</param>
    /// <param name="removeRole">删除数据所需的最小权限</param>
    /// <param name="changeRole">更改数据所需的最小权限</param>
    /// <param name="getRole">获取数据所需的最小权限</param>
    /// <param name="profileType">配置文件类型</param>
    /// <param name="jsonSerializerOptions">JSON转换器</param>
    /// <returns></returns>
    public XFEDataTableManagerBuilder AddTable<T>(string tabelShowName, UserRole addRole, UserRole removeRole, UserRole changeRole, UserRole getRole, Type profileType, JsonSerializerOptions? jsonSerializerOptions = null) where T : IIDModel
    {
        dataTableList.Add(new XFEDataTable<T>(profileType)
        {
            TableShowName = tabelShowName,
            AddRole = addRole,
            RemoveRole = removeRole,
            ChangeRole = changeRole,
            GetRole = getRole,
            JsonSerializerOptions = jsonSerializerOptions
        });
        return this;
    }

    /// <summary>
    /// 构建数据表管理器
    /// </summary>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="getUsersFunction"></param>
    /// <returns></returns>
    public XFEDataTableManager Build(Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction, Func<IEnumerable<User>> getUsersFunction)
    {
        var xFEDataTableManager = new XFEDataTableManagerImpl();
        foreach (var table in dataTableList)
        {
            table.GetEncryptedUserLoginModelFunction = getEncryptedUserLoginModelFunction;
            table.GetUsersFunction = getUsersFunction;
            xFEDataTableManager.TableList.Add(table);
            xFEDataTableManager.TableDictionary.Add(table.TableNameInRequest, table);
        }
        return xFEDataTableManager;
    }
}

using System.Text.Json;
using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

/// <summary>
/// XFE数据表格管理器构造器
/// </summary>
[CreateImpl]
public abstract class XFEDataTableManagerBuilder
{
    readonly List<IXFEDataTable> dataTableList = [];
    readonly JsonSerializerOptions userJsonSerializerOptions = new();
    /// <summary>
    /// 执行语句列表
    /// </summary>
    public List<string> ExecuteList { get; set; } = [];

    /// <summary>
    /// 创建构造器
    /// </summary>
    /// <returns></returns>
    public static XFEDataTableManagerBuilder CreateBuilder()
    {
        var builder = new XFEDataTableManagerBuilderImpl();
        builder.userJsonSerializerOptions.Converters.Add(new JsonDateTimeConverter());
        return builder;
    }

    /// <summary>
    /// 添加数据表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="xFEDataTable">数据表</param>
    /// <returns></returns>
    public XFEDataTableManagerBuilder AddTable<T>(XFEDataTable<T> xFEDataTable) where T : IIDModel
    {
        dataTableList.Add(xFEDataTable);
        ExecuteList.AddRange([$"get_{xFEDataTable.TableNameInRequest}", $"add_{xFEDataTable.TableNameInRequest}", $"change_{xFEDataTable.TableNameInRequest}", $"remove_{xFEDataTable.TableNameInRequest}"]);
        return this;
    }

    /// <summary>
    /// 添加数据表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <typeparam name="P">配置文件类型</typeparam>
    /// <param name="tabelShowName">表格数据的显示名称（如：订单、用户等）</param>
    /// <param name="addPermissionLevel">增加数据所需的最小权限</param>
    /// <param name="removePermissionLevel">删除数据所需的最小权限</param>
    /// <param name="changePermissionLevel">更改数据所需的最小权限</param>
    /// <param name="getPermissionLevel">获取数据所需的最小权限</param>
    /// <param name="jsonSerializerOptions">JSON转换器</param>
    /// <returns></returns>
    public XFEDataTableManagerBuilder AddTable<T, P>(string tabelShowName, int addPermissionLevel, int removePermissionLevel, int changePermissionLevel, int getPermissionLevel, JsonSerializerOptions? jsonSerializerOptions = null) where T : IIDModel where P : XFEProfile => AddTable<T>(new XFEDataTable<T>(typeof(P))
    {
        TableShowName = tabelShowName,
        AddPermissionLevel = addPermissionLevel,
        RemovePermissionLevel = removePermissionLevel,
        ChangePermissionLevel = changePermissionLevel,
        GetPermissionLevel = getPermissionLevel,
        UserJsonSerializerOptions = userJsonSerializerOptions,
        JsonSerializerOptions = jsonSerializerOptions
    });

    /// <summary>
    /// 构建数据表管理器
    /// </summary>
    /// <param name="getEncryptedUserLoginModelFunction"></param>
    /// <param name="getUsersFunction"></param>
    /// <returns></returns>
    public XFEDataTableManager Build(Func<IEnumerable<IUserInfo>> getUsersFunction, Func<IEnumerable<EncryptedUserLoginModel>> getEncryptedUserLoginModelFunction)
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

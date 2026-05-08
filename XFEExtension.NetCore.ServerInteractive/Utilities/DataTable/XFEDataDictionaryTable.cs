using System.Net;
using System.Reflection;
using System.Text.Json;
using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.Exceptions;
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.ServerModels;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

/// <summary>
/// XFE数据字典表格
/// </summary>
/// <typeparam name="TValue">字典中值的类型</typeparam>
public class XFEDataDictionaryTable<TValue> : IXFEDataTable where TValue : IIdModel
{
    /// <inheritdoc/>
    public Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Func<IEnumerable<IUserInfo>> GetUsersFunction { get; set; } = () => [];
    /// <summary>
    /// 获取字典的方法
    /// </summary>
    public Func<ProfileDictionary<string, TValue>> GetTableFunction { get; set; } = () => [];
    /// <summary>
    /// 向字典中添加元素的方法
    /// </summary>
    public Action<string, TValue> AddToTableFunction { get; set; } = (_, _) => { };
    /// <summary>
    /// 从字典中移除元素的方法
    /// </summary>
    public Action<string> RemoveFromTableFunction { get; set; } = _ => { };
    /// <summary>
    /// 更改字典中元素的方法
    /// </summary>
    public Action<TValue> ChangeItemTableFunction { get; set; } = _ => { };
    /// <inheritdoc/>
    public string TableShowName { get; set; } = typeof(TValue).Name;
    /// <inheritdoc/>
    public string TableName { get; set; } = typeof(TValue).Name;
    /// <inheritdoc/>
    public string TableNameInRequest { get; set; }
    /// <inheritdoc/>
    public int GetPermissionLevel { get; set; }
    /// <inheritdoc/>
    public int RemovePermissionLevel { get; set; }
    /// <inheritdoc/>
    public int ChangePermissionLevel { get; set; }
    /// <inheritdoc/>
    public int AddPermissionLevel { get; set; }
    /// <inheritdoc/>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }
    /// <inheritdoc/>
    public JsonSerializerOptions? UserJsonSerializerOptions { get; set; }

    /// <summary>
    /// 创建字典表格并手动设置Get Set方法等
    /// </summary>
    public XFEDataDictionaryTable() => TableNameInRequest = $"{TableName[0]}".ToLower() + TableName[1..];

    /// <summary>
    /// 根据指定type自动设置Get Set方法
    /// </summary>
    /// <param name="type">继承于XFEProfile的配置文件类型</param>
    /// <remarks>
    /// 对于自动设置Get Set方法的Table，请确保配置文件使用XFEProfile创建ProfileDictionary，属性名称为XXXTable
    /// </remarks>
    public XFEDataDictionaryTable(Type type)
    {
        TableNameInRequest = $"{TableName[0]}".ToLower() + TableName[1..];
        var property = type.GetProperty($"{typeof(TValue).Name}Table", BindingFlags.Public | BindingFlags.Static);
        var profileDictionaryType = typeof(ProfileDictionary<string, TValue>);
        var addMethod = profileDictionaryType.GetMethod("Add", [typeof(string), typeof(TValue)]);
        var removeMethod = profileDictionaryType.GetMethod("Remove", [typeof(string)]);
        var saveMethod = type.GetMethod("SaveProfile", BindingFlags.Public | BindingFlags.Static);
        if (property is null || addMethod is null || removeMethod is null || saveMethod is null) return;
        GetTableFunction = () => property.GetValue(null) as ProfileDictionary<string, TValue> ?? [];
        AddToTableFunction = (key, value) => addMethod.Invoke(GetTableFunction(), [key, value]);
        RemoveFromTableFunction = key => removeMethod.Invoke(GetTableFunction(), [key]);
        ChangeItemTableFunction = item =>
        {
            var table = GetTableFunction();
            if (!table.ContainsKey(item.Id)) return;
            table[item.Id] = item;
            saveMethod.Invoke(null, []);
        };
    }

    /// <summary>
    /// 获取字典中所有值
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TValue> GetValues() => GetTableFunction().Values;

    /// <summary>
    /// 添加一个元素
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(string key, TValue value) => AddToTableFunction(key, value);

    /// <summary>
    /// 移除一个元素
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key) => RemoveFromTableFunction(key);

    /// <summary>
    /// 更改一个元素
    /// </summary>
    /// <param name="value"></param>
    public void Change(TValue value) => ChangeItemTableFunction(value);

    /// <inheritdoc/>
    public async Task<HttpStatusCode> Execute(string execute, QueryableJsonNode requestJsonNode, ServerCoreReturnArgs r)
    {
        var statusCode = HttpStatusCode.OK;
        try
        {
            switch (execute)
            {
                case "get":
                    Console.Write($"获取{TableShowName}列表请求");
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["deviceInfo"], r.Args.ClientIP, GetPermissionLevel, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), r);
                    List<TValue> valueList = [.. GetTableFunction().Values];
                    var pageCount = requestJsonNode["pageCount"]?.GetValue<int>() ?? -1;
                    if (pageCount == -1)
                    {
                        await r.Args.ReplyAndClose(JsonSerializer.Serialize(new
                        {
                            totalCount = valueList.Count,
                            lastPage = -1,
                            dataList = valueList
                        }, JsonSerializerOptions));
                    }
                    else
                    {
                        var page = requestJsonNode["page"]?.GetValue<int>() ?? -1;
                        await r.Args.ReplyAndClose(JsonSerializer.Serialize(new
                        {
                            totalCount = valueList.Count,
                            lastPage = (int)Math.Ceiling((double)valueList.Count / pageCount),
                            dataList = valueList[(page * pageCount)..((page + 1) * pageCount)]
                        }, JsonSerializerOptions));
                    }
                    break;
                case "add":
                    Console.Write($"添加{TableShowName}请求");
                    var item = JsonSerializer.Deserialize<TValue>(Convert.FromBase64String(requestJsonNode["data"]?.ToString() ?? string.Empty), JsonSerializerOptions);
                    if (item is null)
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n无法使用Json转换目标{TableShowName}信息");
                    }
                    Console.Write($"：{item.Id}");
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["deviceInfo"], r.Args.ClientIP, AddPermissionLevel, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), r);
                    if (item.Id.IsNullOrWhiteSpace())
                        item.Id = Guid.NewGuid().ToString();
                    var addTable = GetTableFunction();
                    while (addTable.ContainsKey(item.Id))
                        item.Id = Guid.NewGuid().ToString();
                    Add(item.Id, item);
                    r.Args.Close();
                    break;
                case "remove":
                    Console.Write($"删除{TableShowName}请求");
                    var id = requestJsonNode["id"]?.ToString();
                    Console.Write($"：{id}");
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["deviceInfo"], r.Args.ClientIP, RemovePermissionLevel, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), r);
                    if (id.IsNullOrWhiteSpace())
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"{TableShowName}ID不能为空");
                    }
                    Remove(id);
                    r.Args.Close();
                    break;
                case "change":
                    Console.Write($"更改{TableShowName}请求");
                    item = JsonSerializer.Deserialize<TValue>(Convert.FromBase64String(requestJsonNode["data"]?.ToString() ?? string.Empty), JsonSerializerOptions);
                    if (item is null)
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n无法使用Json转换目标{TableShowName}信息");
                    }
                    Console.Write($"：{item.Id}");
                    if (item.Id.IsNullOrWhiteSpace())
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n{TableShowName}ID不能为空");
                    }
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["deviceInfo"], r.Args.ClientIP, ChangePermissionLevel, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), r);
                    Change(item);
                    r.Args.Close();
                    break;
                default:
                    Console.WriteLine($"[ERROR] 意料之外的方法：{execute}");
                    await r.Args.ReplyAndClose($"意料之外的方法：{execute}", HttpStatusCode.BadRequest);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN]【{r.Args.ClientIP}】{ex.Message}");
            Console.WriteLine($"[TRACE]{ex.StackTrace}");
            await r.Args.ReplyAndClose(ex.Message, statusCode);
        }
        return statusCode;
    }
}

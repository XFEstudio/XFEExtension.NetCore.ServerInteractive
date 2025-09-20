using System.Net;
using System.Reflection;
using System.Text.Json;
using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.StringExtension;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

/// <summary>
/// XFE数据表格
/// </summary>
/// <typeparam name="T"></typeparam>
public class XFEDataTable<T> : IXFEDataTable where T : IIDModel
{
    /// <inheritdoc/>
    public Func<IEnumerable<EncryptedUserLoginModel>> GetEncryptedUserLoginModelFunction { get; set; } = () => [];
    /// <inheritdoc/>
    public Func<IEnumerable<User>> GetUsersFunction { get; set; } = () => [];
    /// <summary>
    /// 获取列表的方法
    /// </summary>
    public Func<IEnumerable<T>> GetTableFunction { get; set; } = () => [];
    /// <summary>
    /// 添加元素至列表的方法
    /// </summary>
    public Action<T> AddToTableFunction { get; set; } = item => { };
    /// <summary>
    /// 从列表中移除元素的方法
    /// </summary>
    public Action<string> RemoveFromTableFunction { get; set; } = id => { };
    /// <summary>
    /// 更改列表中元素的方法
    /// </summary>
    public Action<T> ChangeItemTableFunction { get; set; } = item => { };
    /// <inheritdoc/>
    public string TableShowName { get; set; } = typeof(T).Name;
    /// <inheritdoc/>
    public string TableName { get; set; } = typeof(T).Name;
    /// <inheritdoc/>
    public string TableNameInRequest { get; set; } = typeof(T).Name;
    /// <inheritdoc/>
    public int GetPermissionLevel { get; set; } = 0;
    /// <inheritdoc/>
    public int RemovePermissionLevel { get; set; } = 0;
    /// <inheritdoc/>
    public int ChangePermissionLevel { get; set; } = 0;
    /// <inheritdoc/>
    public int AddPermissionLevel { get; set; } = 0;
    /// <inheritdoc/>
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    /// <summary>
    /// 创建列表并手动设置Get Set方法等
    /// </summary>
    public XFEDataTable() => TableNameInRequest = $"{TableName[0]}".ToLower() + TableName[1..];

    /// <summary>
    /// 根据指定type自动设置Get Set方法
    /// </summary>
    /// <param name="type">继承于XFEProfile的配置文件类型</param>
    /// <remarks>
    /// 对于自动设置Get Set方法的Table，请确保配置文件使用XFEProfile创建ProfileList，属性名称为XXXTable
    /// </remarks>
    public XFEDataTable(Type type)
    {
        TableNameInRequest = $"{TableName[0]}".ToLower() + TableName[1..];
        var property = type.GetProperty($"{typeof(T).Name}Table", BindingFlags.Public | BindingFlags.Static);
        var profileListType = typeof(ProfileList<T>);
        var addMethod = profileListType.GetMethod("Add", [typeof(T)]);
        var removeMethod = profileListType.GetMethod("RemoveAt");
        var saveMethod = type.GetMethod("SaveProfile", BindingFlags.Public | BindingFlags.Static);
        if (property is not null && addMethod is not null && removeMethod is not null && saveMethod is not null)
        {
            GetTableFunction = () => property.GetValue(null) as ProfileList<T> ?? [];
            AddToTableFunction = item => addMethod.Invoke(GetTableFunction(), [item]);
            RemoveFromTableFunction = id =>
            {
                var table = GetTableFunction();
                removeMethod.Invoke(table, [table.FirstOrDefault(item => item.ID == id)]);
            };
            ChangeItemTableFunction = item =>
            {
                var table = GetTableFunction();
                var targetItem = table.FirstOrDefault(originItem => originItem.ID == item.ID);
                if (targetItem is not null)
                {
                    foreach (var itemProperty in targetItem.GetType().GetProperties())
                    {
                        itemProperty.SetValue(targetItem, itemProperty.GetValue(item));
                    }
                    saveMethod.Invoke(null, []);
                }
            };
        }
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetList() => GetTableFunction();

    /// <summary>
    /// 添加一个元素
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item) => AddToTableFunction(item);

    /// <summary>
    /// 移除一个元素
    /// </summary>
    /// <param name="id"></param>
    public void Remove(string id) => RemoveFromTableFunction(id);

    /// <summary>
    /// 更改一个元素
    /// </summary>
    /// <param name="item"></param>
    public void Change(T item) => ChangeItemTableFunction(item);

    /// <inheritdoc/>
    public async Task<HttpStatusCode> Execute(string execute, QueryableJsonNode requestJsonNode, CyberCommRequestEventArgs e)
    {
        var statusCode = HttpStatusCode.OK;
        try
        {
            switch (execute)
            {
                case "get":
                    Console.Write($"【{e.ClientIP}】获取{TableShowName}列表请求");
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["computerInfo"], e.ClientIP, GetPermissionLevel, JsonSerializerOptions, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), ref statusCode);
                    await e.ReplyAndClose(JsonSerializer.Serialize(GetTableFunction(), JsonSerializerOptions), HttpStatusCode.OK);
                    break;
                case "add":
                    Console.Write($"【{e.ClientIP}】添加{TableShowName}请求");
                    var item = JsonSerializer.Deserialize<T>(Convert.FromBase64String(requestJsonNode["data"]), JsonSerializerOptions);
                    if (item is null)
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n无法使用Json转换目标{TableShowName}信息");
                    }
                    Console.Write($"：{item.ID}");
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["computerInfo"], e.ClientIP, AddPermissionLevel, JsonSerializerOptions, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), ref statusCode);
                    if (item.ID.IsNullOrWhiteSpace())
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"{TableShowName}ID不能为空");
                    }
                    while (IDModelHelper.HasSameID(item.ID, GetTableFunction().Cast<IIDModel>()))
                        item.ID = Guid.NewGuid().ToString();
                    Add(item);
                    e.Close();
                    break;
                case "remove":
                    Console.Write($"【{e.ClientIP}】删除{TableShowName}请求");
                    var id = requestJsonNode["id"].ToString();
                    Console.Write($"：{id}");
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["computerInfo"], e.ClientIP, RemovePermissionLevel, JsonSerializerOptions, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), ref statusCode);
                    if (id.IsNullOrWhiteSpace())
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"{TableShowName}ID不能为空");
                    }
                    Remove(id);
                    e.Close();
                    break;
                case "change":
                    Console.Write($"【{e.ClientIP}】更改{TableShowName}请求");
                    item = JsonSerializer.Deserialize<T>(Convert.FromBase64String(requestJsonNode["data"]), JsonSerializerOptions);
                    if (item is null)
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n无法使用Json转换目标{TableShowName}信息");
                    }
                    Console.Write($"：{item.ID}");
                    if (item.ID.IsNullOrWhiteSpace())
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n{TableShowName}ID不能为空");
                    }
                    if (item.ID.IsNullOrWhiteSpace())
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        throw new StopAction(() => { }, $"\n{TableShowName}ID不能为空");
                    }
                    UserHelper.ValidatePermission(requestJsonNode["session"], requestJsonNode["computerInfo"], e.ClientIP, ChangePermissionLevel, JsonSerializerOptions, GetEncryptedUserLoginModelFunction(), GetUsersFunction(), ref statusCode);
                    Change(item);
                    e.Close();
                    break;
                default:
                    Console.WriteLine($"[ERROR]【{e.ClientIP}】意料之外的方法：{execute}");
                    await e.ReplyAndClose($"意料之外的方法：{execute}", HttpStatusCode.BadRequest);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN]【{e.ClientIP}】{ex.Message}");
            Console.WriteLine($"[TRACE]{ex.StackTrace}");
            await e.ReplyAndClose(ex.Message, statusCode);
        }
        return statusCode;
    }
}

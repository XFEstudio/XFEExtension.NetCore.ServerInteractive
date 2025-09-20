using System.Net;
using XFEExtension.NetCore.AutoImplement;
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.XFETransform.JsonConverter;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;

/// <summary>
/// XFE数据表格管理器
/// </summary>
[CreateImpl]
public abstract class XFEDataTableManager
{
    /// <summary>
    /// 数据表格列表
    /// </summary>
    public List<IXFEDataTable> TableList { get; set; } = [];
    /// <summary>
    /// 数据表格字典
    /// </summary>
    public Dictionary<string, IXFEDataTable> TableDictionary { get; set; } = [];

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="execute">请求语句</param>
    /// <param name="requestJsonNode">json节点</param>
    /// <param name="e">服务器参数</param>
    /// <returns></returns>
    public async Task Execute(string execute, QueryableJsonNode requestJsonNode, CyberCommRequestEventArgs e)
    {
        var split = execute.Split('_');
        if (TableDictionary.TryGetValue(split[1], out var table))
        {
            _ = await table.Execute(split[0], requestJsonNode, e);
        }
        else
        {
            Console.WriteLine($"[ERROR]【{e.ClientIP}】意料之外的表格：{split[1]}");
            await e.ReplyAndClose($"意料之外的方法：{execute}", HttpStatusCode.BadRequest);
        }
    }
}

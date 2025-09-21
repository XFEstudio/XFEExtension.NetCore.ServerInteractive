using System.Text.Json.Serialization;
using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.Utilities;

/// <summary>
/// 表格请求结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class TableRequestResult<T> where T : IIDModel
{
    /// <summary>
    /// 数据列表
    /// </summary>
    [JsonPropertyName("dataList")]
    public List<T> DataList { get; set; } = [];
    /// <summary>
    /// 最后一页
    /// </summary>
    [JsonPropertyName("lastPage")]
    public int LastPage { get; set; }
    /// <summary>
    /// 总计数量
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}

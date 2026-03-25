using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// ID模型帮助类
/// </summary>
public static class IdModelHelper
{
    /// <summary>
    /// 是否有相同的ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemList"></param>
    /// <returns></returns>
    public static bool HasSameId(string id, IEnumerable<IIdModel> itemList) => itemList.Any(item => item.Id == id);
}

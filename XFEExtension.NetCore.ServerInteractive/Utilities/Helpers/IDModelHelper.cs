using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

public static class IDModelHelper
{
    public static bool HasSameID(string id, IEnumerable<IIDModel> itemList) => itemList.Any(item => item.ID == id);
}

using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Profiles;

public partial class DataProfile : XFEProfile
{
    [ProfileProperty]
    [ProfilePropertyAddGet("Current._personTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._personTable")]
    private ProfileList<Person> _personTable = [];

    [ProfileProperty]
    [ProfilePropertyAddGet("Current._orderTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current._orderTable")]
    private ProfileList<Order> _orderTable = [];
}

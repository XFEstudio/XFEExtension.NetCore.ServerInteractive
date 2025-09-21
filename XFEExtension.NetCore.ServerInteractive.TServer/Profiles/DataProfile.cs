using XFEExtension.NetCore.AutoConfig;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Profiles;

public partial class DataProfile : XFEProfile
{
    [ProfileProperty]
    [ProfilePropertyAddGet("Current.personTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.personTable")]
    private ProfileList<Person> personTable = [];

    [ProfileProperty]
    [ProfilePropertyAddGet("Current.orderTable.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.orderTable")]
    private ProfileList<Order> orderTable = [];
}

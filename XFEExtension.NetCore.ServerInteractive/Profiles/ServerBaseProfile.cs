using XFEExtension.NetCore.AutoConfig;

namespace XFEExtension.NetCore.ServerInteractive.Profiles;

public partial class ServerBaseProfile : XFEProfile
{
    [ProfileProperty]
    [ProfilePropertyAddGet("Current.serverLastBindingAddressDictionary.CurrentProfile = Current")]
    [ProfilePropertyAddGet("return Current.serverLastBindingAddressDictionary")]
    private ProfileDictionary<string, string> serverLastBindingAddressDictionary = [];
}

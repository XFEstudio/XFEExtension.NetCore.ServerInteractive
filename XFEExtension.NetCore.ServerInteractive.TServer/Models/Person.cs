using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Models;

public class Person : IIDModel
{
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

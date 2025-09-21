using XFEExtension.NetCore.ServerInteractive.Models;

namespace XFEExtension.NetCore.ServerInteractive.TServer.Models;

public class Order : IIDModel
{
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Description { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.XFETransform.JsonConverter;

internal class Program
{
    static readonly ServerInteractive serverInteractive = new("http://localhost:8080/api", DeviceHelper.GetUniqueHardwareId(), string.Empty);
    //static readonly TableRequester tableRequester = new()
    //{
    //    RequestAddress = "http://localhost:8080/api",
    //    ComputerInfo = DeviceHelper.GetUniqueHardwareId()!,
    //    Session = string.Empty
    //};

    [SMTest("Admin", "123456")]
    public static async Task Login(string account, string password)
    {
        var session = await serverInteractive.Login(account, password);
        Console.WriteLine(session);
        QueryableJsonNode jsonNode = session;
        serverInteractive.TableRequester.Session = jsonNode["session"];
    }

    //[SMTest]
    public static string ConvertOrder()
    {
        var order = new Order()
        {
            Description = "测试订单的描述",
            Name = "测试订单"
        };
        return JsonSerializer.Serialize(order);
    }

    //[SMTest]
    public static async Task<bool> AddOrder() => await serverInteractive.TableRequester.Add<Order>(new()
    {
        Description = "测试订单的描述",
        Name = "测试订单"
    });

    [SMTest]
    public static async Task GetOrder()
    {
        var result = await serverInteractive.TableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.ID}\tName:{order.Name}\tDS:{order.Description}");
        }
    }

    [SMTest]
    public static async Task ChangeOrder()
    {
        var result = await serverInteractive.TableRequester.Get<Order>();
        var order = result.DataList[0];
        Console.WriteLine($"ID:{order.ID}\tName:{order.Name}\tDS:{order.Description}");
        order.Description = "这是一条修改后的测试订单";
        order.Name = "Test001";
        await serverInteractive.TableRequester.Change(order);
    }
    [SMTest]
    public static async Task GetOrder2()
    {
        var result = await serverInteractive.TableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.ID}\tName:{order.Name}\tDS:{order.Description}");
        }
    }
}
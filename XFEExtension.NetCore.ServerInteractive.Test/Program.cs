using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities;
using XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

internal class Program
{
    static readonly XFEClientRequester xFEClientRequester = XFEClientRequesterBuilder.CreateBuilder("http://localhost:8080/api", string.Empty, DeviceHelper.GetUniqueHardwareId())
        .UseXFEStandardRequest<UserFaceInfo>()
        .Build();

    static readonly TableRequester tableRequester = new();

    static Program() => xFEClientRequester.MessageReceived += XFEClientRequester_MessageReceived;

    private static void XFEClientRequester_MessageReceived(object? sender, ServerInteractiveEventArgs e)
    {
        Console.WriteLine($"请求完成：{e.StatusCode}\t{e.Message}");
    }

    [SMTest("Admin", "123456")]
    public static async Task Login(string account, string password)
    {
        var result = await xFEClientRequester.Request<UserLoginResult<UserFaceInfo>>("login", account, password);
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result.Session);
            Console.WriteLine(result.Result.ExpireDate);
            Console.WriteLine(result.Result.UserInfo.ID);
            Console.WriteLine(result.Result.UserInfo.NickName);
            Console.WriteLine(result.Result.UserInfo.PermissionLevel);
            tableRequester.RequestAddress = xFEClientRequester.RequestAddress;
            tableRequester.Session = result.Result.Session;
            tableRequester.ComputerInfo = xFEClientRequester.ComputerInfo;
        }
        else
        {
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Message);
        }
    }

    [SMTest]
    public static async Task ReLogin()
    {
        var result = await xFEClientRequester.Request<UserFaceInfo>("relogin");
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result.ID);
            Console.WriteLine(result.Result.NickName);
            Console.WriteLine(result.Result.PermissionLevel);
        }
        else
        {
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Message);
        }
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
    public static async Task<bool> AddOrder() => await tableRequester.Add<Order>(new()
    {
        Description = "测试订单的描述",
        Name = "测试订单"
    });

    //[SMTest]
    public static async Task GetOrder()
    {
        var result = await tableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.ID}\tName:{order.Name}\tDS:{order.Description}");
        }
    }

    //[SMTest]
    public static async Task ChangeOrder()
    {
        var result = await tableRequester.Get<Order>();
        var order = result.DataList[0];
        Console.WriteLine($"ID:{order.ID}\tName:{order.Name}\tDS:{order.Description}");
        order.Description = "这是一条修改后的测试订单";
        order.Name = "Test001";
        await tableRequester.Change(order);
    }

    //[SMTest]
    public static async Task GetOrder2()
    {
        var result = await tableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.ID}\tName:{order.Name}\tDS:{order.Description}");
        }
        Console.ReadLine();
    }
}
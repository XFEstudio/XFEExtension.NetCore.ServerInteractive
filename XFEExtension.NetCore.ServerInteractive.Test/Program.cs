using System.Text.Json;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.RequesterModels;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;
using XFEExtension.NetCore.XUnit.Attributes;

namespace XFEExtension.NetCore.ServerInteractive.Test;

internal class Program
{
    private static readonly XFEClientRequester XFEClientRequester = XFEClientRequesterBuilder.CreateBuilder()
        .UseXFEStandardRequest<UserFaceInfo>()
        .AddRequest("test", (_, _, _) => new
        {
            execute = "test"
        }, response => response)
        .Build(options =>
        {
            options.RequestAddress = "http://localhost:8080/management";
        });

    private static readonly TableRequester TableRequester = new();

    static Program() => XFEClientRequester.MessageReceived += XFEClientRequester_MessageReceived;

    private static void XFEClientRequester_MessageReceived(object? sender, ServerInteractiveEventArgs e)
    {
        Console.WriteLine($"请求完成：{e.StatusCode}\t{e.Message}");
    }

    [SMTest("Admin", "12345641")]
    public static async Task Login(string account, string password)
    {
        var result = await XFEClientRequester.Request<UserLoginResult<UserFaceInfo>>("login", account, password);
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result.Session);
            Console.WriteLine(result.Result.ExpireDate);
            Console.WriteLine(result.Result.UserInfo.Id);
            Console.WriteLine(result.Result.UserInfo.NickName);
            Console.WriteLine(result.Result.UserInfo.PermissionLevel);
            TableRequester.RequestAddress = XFEClientRequester.RequestAddress;
            TableRequester.Session = result.Result.Session;
            TableRequester.DeviceInfo = XFEClientRequester.DeviceInfo;
        }
        else
        {
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Message);
        }
    }

    //[SMTest]
    public static async Task Test()
    {
        await Parallel.ForEachAsync(Enumerable.Range(0, 100000), async (_, _) =>
        {
            var result = await XFEClientRequester.Request<string>("test");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(result.Result);
            }
            else
            {
                Console.WriteLine(result.StatusCode);
                Console.WriteLine(result.Message);
            }
        });
    }

    //[SMTest]
    public static async Task ReLogin()
    {
        var result = await XFEClientRequester.Request<UserFaceInfo>("relogin");
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result.Id);
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
    public static async Task Check()
    {
        try
        {
            var result = await XFEClientRequester.Request<DateTime>("check_connect");
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(result.Result);
            }
            else
            {
                Console.WriteLine(result.StatusCode);
                Console.WriteLine(result.Message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    //[SMTest]
    public static async Task GetLog()
    {
        var result = await XFEClientRequester.Request<string>("get_log", DateTime.MinValue, DateTime.MaxValue);
        if (result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result);
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
    public static async Task<bool> AddOrder() => await TableRequester.Add<Order>(new()
    {
        Description = "测试订单的描述",
        Name = "测试订单"
    });

    //[SMTest]
    public static async Task GetOrder()
    {
        var result = await TableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.Id}\tName:{order.Name}\tDS:{order.Description}");
        }
    }

    //[SMTest]
    public static async Task ChangeOrder()
    {
        var result = await TableRequester.Get<Order>();
        var order = result.DataList[0];
        Console.WriteLine($"ID:{order.Id}\tName:{order.Name}\tDS:{order.Description}");
        order.Description = "这是一条修改后的测试订单";
        order.Name = "Test001";
        await TableRequester.Change(order);
    }

    //[SMTest]
    public static async Task GetOrder2()
    {
        var result = await TableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.Id}\tName:{order.Name}\tDS:{order.Description}");
        }
        Console.ReadLine();
    }
}
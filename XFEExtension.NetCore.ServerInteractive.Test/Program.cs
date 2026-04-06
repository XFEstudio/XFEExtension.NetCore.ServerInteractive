using System.Net;
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
    private static readonly XFEClientRequester s_xFEClientRequester = XFEClientRequesterBuilder.CreateBuilder()
        .UseXFEStandardRequest<UserFaceInfo>()
        .AddRequest("test", (_, _, _) => new
        {
            execute = "test"
        }, response => response)
        .AddRequest("echo", (_, _, parameters) => new
        {
            execute = "echo",
            message = parameters.Length > 0 ? parameters[0] : string.Empty
        }, response => response)
        .AddRequest("math/add", (_, _, parameters) => new
        {
            execute = "math/add",
            a = parameters.Length > 0 ? parameters[0] : 0,
            b = parameters.Length > 1 ? parameters[1] : 0
        }, response => response)
        .AddRequest("math/multiply", (_, _, parameters) => new
        {
            execute = "math/multiply",
            a = parameters.Length > 0 ? parameters[0] : 0,
            b = parameters.Length > 1 ? parameters[1] : 0
        }, response => response)
        .AddRequest("status", (_, _, _) => new
        {
            execute = "status"
        }, response => response)
        .AddRequest("greet", (_, _, parameters) => new
        {
            execute = "greet",
            name = parameters.Length > 0 ? parameters[0] : string.Empty
        }, response => response)
        .AddRequest("time", (_, _, _) => new
        {
            execute = "time"
        }, response => response)
        .Build(options =>
        {
            options.RequestAddress = "http://localhost:3305/api";
        });

    private static readonly TableRequester TableRequester = new();

    static Program() => s_xFEClientRequester.MessageReceived += XFEClientRequester_MessageReceived;

    private static void XFEClientRequester_MessageReceived(object? sender, ServerInteractiveEventArgs e)
    {
        Console.WriteLine($"请求完成：{e.StatusCode}\t{e.Message}");
    }

    #region 基础测试

    [SMTest]
    public static async Task TestGET()
    {
        using var client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:3305/api/")
        };
        var response = await client.GetAsync("test");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"请求失败：{response.StatusCode}，响应内容：{errorContent}");
        }
    }

    /// <summary>
    /// 测试基本的test端点请求
    /// </summary>
   [SMTest]
    public static async Task Test()
    {
        var result = await s_xFEClientRequester.Request<string>("test");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result);
        }
        else
        {
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Message);
        }
    }

    /// <summary>
    /// 测试连接检查端点
    /// </summary>
   [SMTest]
    public static async Task Check()
    {
        try
        {
            var result = await s_xFEClientRequester.Request<DateTime>("check_connect");
            if (result.StatusCode == HttpStatusCode.OK)
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

    #endregion

    #region 用户登录测试

    /// <summary>
    /// 测试用户登录功能
    /// </summary>
    [SMTest("Admin", "123456")]
    public static async Task Login(string account, string password)
    {
        var result = await s_xFEClientRequester.Request<UserLoginResult<UserFaceInfo>>("login", account, password);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result.Session);
            Console.WriteLine(result.Result.ExpireDate);
            Console.WriteLine(result.Result.UserInfo.Id);
            Console.WriteLine(result.Result.UserInfo.NickName);
            Console.WriteLine(result.Result.UserInfo.PermissionLevel);
            TableRequester.RequestAddress = s_xFEClientRequester.RequestAddress;
            TableRequester.Session = result.Result.Session;
            TableRequester.DeviceInfo = s_xFEClientRequester.DeviceInfo;
        }
        else
        {
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Message);
        }
    }

    /// <summary>
    /// 测试无效用户登录（错误密码）
    /// </summary>
    [SMTest("Admin", "wrong_password")]
    public static async Task LoginWithWrongPassword(string account, string password)
    {
        var result = await s_xFEClientRequester.Request<UserLoginResult<UserFaceInfo>>("login", account, password);
        Console.WriteLine($"状态码：{result.StatusCode}");
        Console.WriteLine($"消息：{result.Message}");
    }

    /// <summary>
    /// 测试无效用户登录（不存在的用户）
    /// </summary>
    [SMTest("NonExistentUser", "123456")]
    public static async Task LoginWithNonExistentUser(string account, string password)
    {
        var result = await s_xFEClientRequester.Request<UserLoginResult<UserFaceInfo>>("login", account, password);
        Console.WriteLine($"状态码：{result.StatusCode}");
        Console.WriteLine($"消息：{result.Message}");
    }

    /// <summary>
    /// 测试重新登录功能
    /// </summary>
    [SMTest]
    public static async Task ReLogin()
    {
        var result = await s_xFEClientRequester.Request<UserFaceInfo>("relogin");
        if (result.StatusCode == HttpStatusCode.OK)
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

    #endregion

    #region Echo服务测试

    /// <summary>
    /// 测试Echo服务：发送消息并验证原样返回
    /// </summary>
   [SMTest]
    public static async Task EchoTest()
    {
        var result = await s_xFEClientRequester.Request<string>("echo", "Hello, XFE!");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"Echo响应：{result.Result}");
        }
        else
        {
            Console.WriteLine($"Echo失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试Echo服务：发送空消息
    /// </summary>
   [SMTest]
    public static async Task EchoEmptyTest()
    {
        var result = await s_xFEClientRequester.Request<string>("echo", string.Empty);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"空Echo响应：{result.Result}");
        }
        else
        {
            Console.WriteLine($"空Echo失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试Echo服务：发送中文消息
    /// </summary>
   [SMTest]
    public static async Task EchoChineseTest()
    {
        var result = await s_xFEClientRequester.Request<string>("echo", "你好，世界！");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"中文Echo响应：{result.Result}");
        }
        else
        {
            Console.WriteLine($"中文Echo失败：{result.StatusCode} {result.Message}");
        }
    }

    #endregion

    #region 数学运算服务测试

    /// <summary>
    /// 测试加法运算
    /// </summary>
   [SMTest]
    public static async Task MathAddTest()
    {
        var result = await s_xFEClientRequester.Request<string>("math/add", 3.0, 5.0);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"3 + 5 = {result.Result}");
        }
        else
        {
            Console.WriteLine($"加法失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试乘法运算
    /// </summary>
   [SMTest]
    public static async Task MathMultiplyTest()
    {
        var result = await s_xFEClientRequester.Request<string>("math/multiply", 4.0, 7.0);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"4 × 7 = {result.Result}");
        }
        else
        {
            Console.WriteLine($"乘法失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试加法运算：负数
    /// </summary>
   [SMTest]
    public static async Task MathAddNegativeTest()
    {
        var result = await s_xFEClientRequester.Request<string>("math/add", -10.0, 3.0);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"-10 + 3 = {result.Result}");
        }
        else
        {
            Console.WriteLine($"负数加法失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试乘法运算：零
    /// </summary>
   [SMTest]
    public static async Task MathMultiplyZeroTest()
    {
        var result = await s_xFEClientRequester.Request<string>("math/multiply", 42.0, 0.0);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"42 × 0 = {result.Result}");
        }
        else
        {
            Console.WriteLine($"零乘法失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试加法运算：小数
    /// </summary>
   [SMTest]
    public static async Task MathAddDecimalTest()
    {
        var result = await s_xFEClientRequester.Request<string>("math/add", 1.5, 2.3);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"1.5 + 2.3 = {result.Result}");
        }
        else
        {
            Console.WriteLine($"小数加法失败：{result.StatusCode} {result.Message}");
        }
    }

    #endregion

    #region 状态服务测试

    /// <summary>
    /// 测试服务器状态端点
    /// </summary>
   [SMTest]
    public static async Task StatusTest()
    {
        var result = await s_xFEClientRequester.Request<string>("status");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"服务器状态：{result.Result}");
        }
        else
        {
            Console.WriteLine($"状态查询失败：{result.StatusCode} {result.Message}");
        }
    }

    #endregion

    #region 问候服务测试

    /// <summary>
    /// 测试问候服务：正常名称
    /// </summary>
   [SMTest]
    public static async Task GreetTest()
    {
        var result = await s_xFEClientRequester.Request<string>("greet", "XFEstudio");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"问候响应：{result.Result}");
        }
        else
        {
            Console.WriteLine($"问候失败：{result.StatusCode} {result.Message}");
        }
    }

    /// <summary>
    /// 测试问候服务：空名称（应返回错误）
    /// </summary>
   [SMTest]
    public static async Task GreetEmptyNameTest()
    {
        var result = await s_xFEClientRequester.Request<string>("greet");
        Console.WriteLine($"空名称状态码：{result.StatusCode}");
        Console.WriteLine($"空名称消息：{result.Message}");
    }

    #endregion

    #region 时间服务测试

    /// <summary>
    /// 测试服务器时间端点
    /// </summary>
    [SMTest]
    public static async Task TimeTest()
    {
        var result = await s_xFEClientRequester.Request<string>("time");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine($"服务器时间：{result.Result}");
        }
        else
        {
            Console.WriteLine($"时间查询失败：{result.StatusCode} {result.Message}");
        }
    }

    #endregion

    #region 压力测试

    /// <summary>
    /// 压力测试：并行发送10000个请求
    /// </summary>
    [SMTest]
    public static async Task TestBench()
    {
        await Parallel.ForEachAsync(Enumerable.Range(0, 10000), async (_, _) =>
        {
            var result = await s_xFEClientRequester.Request<string>("test");
            if (result.StatusCode == HttpStatusCode.OK)
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

    #endregion

    #region 日志服务测试

    /// <summary>
    /// 测试获取服务器日志
    /// </summary>
    [SMTest]
    public static async Task GetLog()
    {
        var result = await s_xFEClientRequester.Request<string>("get_log", DateTime.MinValue, DateTime.MaxValue);
        if (result.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine(result.Result);
        }
        else
        {
            Console.WriteLine(result.StatusCode);
            Console.WriteLine(result.Message);
        }
    }

    #endregion

    #region 数据表操作测试

    /// <summary>
    /// 测试Order模型序列化
    /// </summary>
    [SMTest]
    public static string ConvertOrder()
    {
        var order = new Order
        {
            Description = "测试订单的描述",
            Name = "测试订单"
        };
        return JsonSerializer.Serialize(order);
    }

    /// <summary>
    /// 测试添加订单
    /// </summary>
   [SMTest]
    public static async Task<bool> AddOrder() => await TableRequester.Add<Order>(new()
    {
        Description = "测试订单的描述",
        Name = "测试订单"
    });

    /// <summary>
    /// 测试获取订单列表
    /// </summary>
    [SMTest]
    public static async Task GetOrder()
    {
        var result = await TableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.Id}\tName:{order.Name}\tDS:{order.Description}");
        }
    }

    /// <summary>
    /// 测试修改订单
    /// </summary>
    [SMTest]
    public static async Task ChangeOrder()
    {
        var result = await TableRequester.Get<Order>();
        var order = result.DataList[0];
        Console.WriteLine($"ID:{order.Id}\tName:{order.Name}\tDS:{order.Description}");
        order.Description = "这是一条修改后的测试订单";
        order.Name = "Test001";
        await TableRequester.Change(order);
    }

    /// <summary>
    /// 测试获取修改后的订单列表
    /// </summary>
    [SMTest]
    public static async Task GetOrder2()
    {
        var result = await TableRequester.Get<Order>();
        foreach (var order in result.DataList)
        {
            Console.WriteLine($"ID:{order.Id}\tName:{order.Name}\tDS:{order.Description}");
        }
        Console.ReadLine();
    }

    #endregion
}
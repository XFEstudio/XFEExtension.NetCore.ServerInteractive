# XFEExtension.NetCore.ServerInteractive

## 描述

ServerInteractive 是一个 C# 的 DLL 库，基于 `XFEExtension.NetCore` 提供的 CyberComm 网络架构，让使用者可以快速构建服务器端和客户端网络框架。支持基于路由的请求分发、增量代码生成（Source Generator）、用户登录体系、数据表管理、IP 封禁等开箱即用的标准功能。

---

## 目录

- [快速开始](#快速开始)
- [服务器端](#服务器端)
  - [搭建服务器](#搭建服务器)
  - [定义标准核心服务（路由处理）](#定义标准核心服务路由处理)
  - [通配符路由](#通配符路由)
  - [使用XFE标准服务器核心](#使用xfe标准服务器核心)
  - [手动组合服务](#手动组合服务)
  - [原始服务与校验服务](#原始服务与校验服务)
  - [服务器核心配置选项](#服务器核心配置选项)
  - [服务方法中响应客户端](#服务方法中响应客户端)
- [客户端](#客户端)
  - [搭建请求器](#搭建请求器)
  - [内联请求注册](#内联请求注册)
  - [定义标准请求服务（Source Generator）](#定义标准请求服务source-generator)
  - [使用XFE标准请求](#使用xfe标准请求)
  - [TableRequester（数据表请求器）](#tablerequester数据表请求器)
- [增量生成器（Source Generator）](#增量生成器source-generator)
  - [服务端：EntryPoint 生成器](#服务端entrypoint-生成器)
  - [客户端：ClientRequest 生成器](#客户端clientrequest-生成器)
  - [诊断规则](#诊断规则)
- [常见错误与解决方案](#常见错误与解决方案)

---

## 快速开始

> 使用前请在项目中引用 `XFEExtension.NetCore.ServerInteractive` NuGet 包。

**最简单的服务器示例**（一个 echo 服务，绑定本地 3300 端口）：

**服务定义（EchoService.cs）**

```csharp
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

// 类必须为 partial，以便 Source Generator 自动生成路由字典
public partial class EchoService : ServerCoreStandardServiceBase
{
    [EntryPoint("echo")]         // 路由路径：/echo
    public async Task Echo()
    {
        var message = Json?["message"]?.GetValue<string>() ?? string.Empty;
        await Close(message);    // 回复客户端并结束请求
    }
}
```

**程序入口（Program.cs）**

```csharp
using XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()                               // 注册日志、异常处理、核心处理器
    .AddServerCore(
        XFEServerCoreBuilder.CreateBuilder()
            .AddService<EchoService>()            // 注册 EchoService（路由从 [EntryPoint] 自动获取）
            .Build(options =>
            {
                options.BindIP("http://localhost:3300/");
            }))
    .Build();

await server.Start();
```

---

## 服务器端

### 搭建服务器

使用 `XFEServerBuilder` 来组装服务器。

```csharp
var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()                   // 使用XFE标准服务器（包含日志初始化、异常处理、核心处理器）
    .AddServerCore(serverCore)        // 添加一个 XFEServerCore 核心服务器实例
    .Build();                         // 构建 XFEServer 实例

await server.Start();                 // 启动所有初始化服务、同步/异步服务及核心服务
```

`UseXFEServer()` 等价于：

```csharp
.AddInitializer<ServerLogInitializer>()      // 日志初始化器
.AddService<ServerExceptionProcessService>() // 全局异常处理
.AddCoreProcessor<XFEServerCoreProcessService>() // 核心请求处理器
```

你也可以只注册自定义服务而不使用标准套件：

```csharp
var server = XFEServerBuilder.CreateBuilder()
    .AddInitializer<MyInitializerService>()  // 自定义初始化服务（服务器启动前执行一次）
    .AddService<MyService>()                 // 自定义同步服务（服务器启动时执行）
    .AddAsyncService<MyAsyncService>()       // 自定义异步服务（服务器启动时异步执行）
    .AddCoreProcessor<MyCoreProcessor>()     // 必须添加一个核心处理器
    .Build();
```

---

### 定义标准核心服务（路由处理）

继承 `ServerCoreStandardServiceBase` 并使用 `[EntryPoint("路由路径")]` 标记方法。  
类必须声明为 `partial`，Source Generator 才能自动生成路由字典。

```csharp
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

public partial class MathService : ServerCoreStandardServiceBase
{
    [EntryPoint("math/add")]
    public async Task Add()
    {
        var a = Json?["a"]?.GetValue<double>() ?? 0;
        var b = Json?["b"]?.GetValue<double>() ?? 0;
        await Close(a + b);     // 返回结果并关闭请求
    }

    [EntryPoint("math/subtract")]
    public void Subtract()
    {
        var a = Json?["a"]?.GetValue<double>() ?? 0;
        var b = Json?["b"]?.GetValue<double>() ?? 0;
        OK();                   // 标记已处理
        // 同步方法同样支持，返回类型为 void
    }
}
```

在 `XFEServerCoreBuilder` 中注册：

```csharp
XFEServerCoreBuilder.CreateBuilder()
    .AddService<MathService>()   // 从 [EntryPoint] 自动获取路由路径并注册
    .Build(options => { options.BindIP("http://localhost:3300/"); });
```

**可在服务方法中访问的常用属性：**

| 属性 | 类型 | 说明 |
|------|------|------|
| `Route` | `string` | 当前请求的路由路径 |
| `ClientIP` | `string` | 客户端 IP 地址 |
| `Json` | `QueryableJsonNode?` | 解析后的请求体 JSON 节点 |
| `Args` | `CyberCommRequestEventArgs` | 原始请求事件参数（含 Headers、Request 等） |
| `XFEServerCore` | `XFEServerCore` | 所属的服务器核心实例 |
| `Handled` | `bool` | 是否已处理请求 |
| `IsStandardError` | `bool` | 是否为标准错误响应 |

---

### 通配符路由

路由段中使用独立的 `*` 可以匹配任意单段路径，支持多级通配符。  
框架会优先匹配字面量段更多的模式（最具体优先原则），与注册顺序无关。

```csharp
public partial class DynamicService : ServerCoreStandardServiceBase
{
    // 匹配 v1/任意值/info，例如 v1/users/info、v1/orders/info
    [EntryPoint("v1/*/info")]
    public async Task DynamicInfo()
    {
        // Route 属性包含实际请求路径，如 v1/users/info
        await Close($"您请求的路由是：{Route}，您的IP是：{ClientIP}");
    }

    // 匹配 v1/test/* 下的路径（* 仅匹配单个路径段），例如 v1/test/a, v1/test/hello
    [EntryPoint("v1/test/*")]
    public async Task TestAny()
    {
        await Close($"Server: {XFEServerCore.ServerCoreName}, Route: {Route}");
    }
}
```

> **注意：** 通配符 `*` 必须是完整的路径段，不能与其他字符混合（如 `a*b` 是非法的）。

---

### 使用XFE标准服务器核心

`UseXFEStandardServerCore<T>()` 扩展方法提供了一套完整的标准化服务器核心，内置：
- 用户登录（服务端路由 `user/login`，客户端可通过别名 `login` 调用）、自动重登（路由 `user/relogin`，别名 `relogin`）
- 数据表 CRUD 管理（`table/get/{表名}`、`table/add/{表名}`、`table/change/{表名}`、`table/remove/{表名}`）
- IP 封禁（`ip/banned/get`、`ip/banned/add`、`ip/banned/remove`）、每日计数
- 连接检查（`check_connect`）、服务器日志（路由 `log/get`，别名 `get_log`；路由 `log/clear`，别名 `clear_log`）

```csharp
var serverCore = XFEServerCoreBuilder.CreateBuilder()
    .UseXFEStandardServerCore<IUserFaceInfo>(options =>
    {
        // 提供用户数据源
        options.GetUserFunction = () => UserProfile.UserTable;
        options.AddUserFunction = user => UserProfile.UserTable.Add((User)user);
        options.GetEncryptedUserLoginModelFunction = () => UserProfile.EncryptedUserLoginModelTable;
        options.AddEncryptedUserLoginModelFunction = UserProfile.EncryptedUserLoginModelTable.Add;
        options.RemoveEncryptedUserLoginModelFunction = model => UserProfile.EncryptedUserLoginModelTable.Remove(model);
        options.GetLoginKeepDays = () => 7;  // 登录 Session 保持天数

        // 登录成功后将 User 对象转换为返回给客户端的接口类型
        options.LoginResultConvertFunction = user => (IUserFaceInfo)user;

        // 配置数据表管理器
        options.DataTableManagerBuilder = XFEDataTableManagerBuilder.CreateBuilder()
            // 参数：显示名称, 添加权限, 删除权限, 修改权限, 查询权限
            .AddTable<Order, DataProfile>("订单", addPermissionLevel: 1, removePermissionLevel: 2, changePermissionLevel: 1, getPermissionLevel: 1)
            .AddTable<User, UserProfile>("用户", addPermissionLevel: 2, removePermissionLevel: 2, changePermissionLevel: 2, getPermissionLevel: 1);
    })
    .AddService<MyCustomService>()   // 追加自定义业务服务
    .Build(options =>
    {
        options.MainEntryPoint = "api";     // 所有路由前缀，例如登录请求的完整路径为 /api/user/login
        options.AcceptPost = true;
        options.AcceptGet = true;
        options.BindIP("http://localhost:3300/")
               .BindIP("https://localhost:3301/");
    });
```

完整服务器示例（配合 TServer 参考实现）：

```csharp
var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()
    .AddServerCore(
        XFEServerCoreBuilder.CreateBuilder()
            .UseXFEStandardServerCore<IUserFaceInfo>(options =>
            {
                options.GetUserFunction = () => UserProfile.UserTable;
                options.AddUserFunction = user => UserProfile.UserTable.Add((User)user);
                options.GetEncryptedUserLoginModelFunction = () => UserProfile.EncryptedUserLoginModelTable;
                options.AddEncryptedUserLoginModelFunction = UserProfile.EncryptedUserLoginModelTable.Add;
                options.RemoveEncryptedUserLoginModelFunction = model => UserProfile.EncryptedUserLoginModelTable.Remove(model);
                options.GetLoginKeepDays = () => 7;
                options.LoginResultConvertFunction = user => (IUserFaceInfo)user;
                options.DataTableManagerBuilder = XFEDataTableManagerBuilder.CreateBuilder()
                    .AddTable<Order, DataProfile>("订单", 1, 2, 1, 1)
                    .AddTable<Person, DataProfile>("人物", 1, 2, 1, 1)
                    .AddTable<User, UserProfile>("用户", 2, 2, 2, 1);
            })
            .AddService<TestCoreService>()
            .AddService<EchoCoreService>()
            .Build(options =>
            {
                options.MainEntryPoint = "api";
                options.AcceptPost = true;
                options.AcceptGet = true;
                options.BindIP("http://localhost:3305/")
                       .BindIP("https://localhost:3306/");
            }))
    .Build();

await server.Start();
```

---

### 手动组合服务

如果不需要标准套件，可以单独按需添加各个服务：

```csharp
XFEServerCoreBuilder.CreateBuilder()
    // 注册标准服务（路由从 [EntryPoint] 自动获取）
    .AddService<MyRouteService>()
    // 显式指定路由注册（适用于动态路由或不使用 Source Generator 的场景）
    .AddServiceWithRoute<MyDynamicService>("dynamic/*/process")
    // 原始服务（监听服务器启动事件和所有原始请求）
    .AddOriginalService<MyRawRequestHandler>()
    // 校验服务（在路由分发前对每个请求进行验证，返回 false 可拦截请求）
    .AddVerifyService<MyAuthVerifyService>()
    // 添加用户基础参数（供登录相关服务使用）
    .AddUserParameterBase(getUserFn, addUserFn, getEncryptedModelFn, addEncryptedModelFn, removeEncryptedModelFn, getKeepDaysFn, convertFn)
    // 添加数据表管理器
    .AddDataTableManager(tableManagerBuilder, getUserFn, getEncryptedModelFn)
    // 添加各内置服务
    .AddEntryPointVerify()          // 入口点校验服务
    .AddDailyCounterService()       // 每日请求统计
    .AddXFEErrorProcessService()    // 异常处理服务
    .AddConnectService()            // 连接检查服务（check_connect 路由）
    .AddStandardLoginService<IUserFaceInfo>()  // 标准登录服务（login、relogin 路由）
    .AddServerLogService()          // 服务器日志查询（get_log 路由）
    .AddIPBannerService()           // IP 封禁服务（get_bannedIPList、add_bannedIP、remove_bannedIP 路由）
    .Build(options =>
    {
        options.BindIP("http://localhost:3300/");
    });
```

---

### 原始服务与校验服务

**原始服务（`IServerCoreOriginalService`）**：继承 `ServerCoreOriginalServiceBase`，可以监听服务器启动事件和每一个原始请求（在路由分发之前）。

```csharp
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

public class MyRawService : ServerCoreOriginalServiceBase
{
    public override void ServerStarted(object? sender, EventArgs e)
    {
        Console.WriteLine("服务器已启动！");
    }

    public override void RequestReceived(object? sender, CyberCommRequestEventArgs e)
    {
        Console.WriteLine($"收到原始请求：{e.Request.Url}");
    }
}
```

**校验服务（`IServerCoreVerifyService`）**：继承 `ServerCoreVerifyServiceBase`，每个请求在路由分发前都会执行校验。返回 `false` 或 `Task<false>` 会中断后续处理。

```csharp
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

public class MyAuthService : ServerCoreVerifyServiceBase
{
    public override bool VerifyRequest()
    {
        // 同步校验，返回 false 则拦截请求
        var token = Args.Request.Headers["X-Token"];
        return !string.IsNullOrEmpty(token);
    }

    public override async Task<bool> VerifyRequestAsync()
    {
        // 异步校验，默认返回 true（不拦截）
        return true;
    }
}
```

---

### 服务器核心配置选项

`XFEServerCoreBuilder.Build(options => { ... })` 接受 `XFEServerCoreOptions`：

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `BindingIPAddress` | `List<string>` | `[]` | 服务器绑定的 URL 列表 |
| `ServerCoreName` | `string` | 自动生成 | 服务器核心名称（日志显示用） |
| `MainEntryPoint` | `string` | `""` | 主入口前缀（空则直接从次级路由匹配） |
| `AcceptGet` | `bool` | `false` | 是否接受 GET 请求 |
| `AcceptPost` | `bool` | `true` | 是否接受 POST 请求 |
| `AutoUnescapeJson` | `bool` | `true` | 是否自动对 JSON 进行反转义 |
| `AcceptNonStandardJson` | `bool` | `true` | 是否接受非标准 JSON 请求体 |
| `GetIPFunction` | `Func<CyberCommRequestEventArgs, string>` | 从 `ClientIP` 获取 | 自定义 IP 获取函数（如代理场景） |

```csharp
.Build(options =>
{
    options.ServerCoreName = "主服务器";
    options.MainEntryPoint = "api";    // 请求路径格式：/api/{子路由}
    options.AcceptGet = true;
    options.AcceptPost = true;
    options.GetIPFunction = args => args.Request.Headers["X-Forwarded-For"] ?? args.ClientIP;
    options.BindIP("http://localhost:3300/")
           .BindIP("https://localhost:3301/");
});
```

---

### 服务方法中响应客户端

在 `ServerCoreStandardServiceBase` 子类中，可以使用以下方法响应：

```csharp
// 发送数据（不关闭连接）
await Send("消息文本");
await Send(new { code = 0, data = "ok" }); // 自动序列化为 JSON

// 发送数据并关闭请求（最常用）
await Close("响应内容");
await Close(new { result = 42 });

// 标记请求已处理（适用于同步方法，不发送响应体）
OK();

// 返回错误信息
Error("参数缺失", HttpStatusCode.BadRequest);
await CloseWithError("未授权", HttpStatusCode.Unauthorized);
```

---

## 客户端

### 搭建请求器

使用 `XFEClientRequesterBuilder` 构建 `XFEClientRequester`：

```csharp
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

var requester = XFEClientRequesterBuilder.CreateBuilder()
    .AddRequest("echo", (session, deviceInfo, parameters) => new
    {
        execute = "echo",
        message = parameters.Length > 0 ? parameters[0] : string.Empty
    }, response => response)
    .Build(options =>
    {
        options.RequestAddress = "http://localhost:3300";  // 服务器地址
        options.Session = string.Empty;                    // 用户 Session（登录后更新）
        options.DeviceInfo = DeviceHelper.GetUniqueHardwareId(); // 设备唯一标识
    });

// 发起请求
var result = await requester.Request<string>("echo", "Hello, World!");
if (result.StatusCode == HttpStatusCode.OK)
    Console.WriteLine(result.Result);  // 输出：Hello, World!
else
    Console.WriteLine($"失败：{result.StatusCode} {result.Message}");
```

`XFEClientRequesterOptions` 属性：

| 属性 | 类型 | 默认值 | 说明 |
|------|------|--------|------|
| `RequestAddress` | `string` | `"http://localhost:3300/"` | 服务器请求地址 |
| `Session` | `string` | `""` | 当前用户 Session |
| `DeviceInfo` | `string` | 硬件唯一 ID | 设备信息 |

---

### 内联请求注册

`AddRequest(route, constructBody, processResponse)` 适合简单的一次性请求：

```csharp
var requester = XFEClientRequesterBuilder.CreateBuilder()
    // 无参请求
    .AddRequest("status", (_, _, _) => new { execute = "status" }, response => response)
    // 带参数请求
    .AddRequest("math/add", (_, _, parameters) => new
    {
        execute = "math/add",
        a = parameters.Length > 0 ? parameters[0] : 0,
        b = parameters.Length > 1 ? parameters[1] : 0
    }, response => response)
    // 带 session 和 deviceInfo 的登录请求示例
    .AddRequest("login", (session, deviceInfo, parameters) => new
    {
        execute = "login",
        account = parameters[0],
        password = parameters[1],
        deviceInfo
    }, response => JsonSerializer.Deserialize<UserLoginResult<UserFaceInfo>>(response)!)
    .Build(options =>
    {
        options.RequestAddress = "http://localhost:3300";
    });
```

- `constructBody`：`(string session, string deviceInfo, object[] parameters) => object`，返回请求体对象（自动 JSON 序列化）。
- `processResponse`：`(string response) => object`，解析响应字符串，可为 `null`（不处理）。

---

### 定义标准请求服务（Source Generator）

继承 `StandardRequestServiceBase` 并使用 `[Request]`/`[Response]` 特性。  
类必须声明为 `partial`。

```csharp
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

public partial class MathRequestService : StandardRequestServiceBase
{
    // [Request] 标记构造请求体的方法，Path 为路由路径，Name 为可选的调用别名
    [Request("math/add", Name = "add")]
    public object BuildAddRequest() => new
    {
        execute = "math/add",
        a = Parameters.Length > 0 ? Parameters[0] : 0,
        b = Parameters.Length > 1 ? Parameters[1] : 0
    };

    // [Response] 标记解析响应的方法，Path 和 Name 与 [Request] 对应
    [Response("math/add", Name = "add")]
    public object ParseAddResponse() => double.Parse(UnescapedResponse);

    [Request("math/multiply")]
    public object BuildMultiplyRequest() => new
    {
        execute = "math/multiply",
        a = Parameters.Length > 0 ? Parameters[0] : 0,
        b = Parameters.Length > 1 ? Parameters[1] : 0
    };

    [Response("math/multiply")]
    public object ParseMultiplyResponse() => double.Parse(UnescapedResponse);
}
```

在 `[Request]`/`[Response]` 方法中可访问的属性：

| 属性 | 说明 |
|------|------|
| `Parameters` | 调用 `Request(name, params object[] parameters)` 时传入的参数数组 |
| `Session` | 当前用户 Session |
| `DeviceInfo` | 设备信息 |
| `Response` | 原始响应字符串 |
| `UnescapedResponse` | 反转义后的响应字符串 |
| `Route` | 实际请求路由路径 |

注册到请求器：

```csharp
var requester = XFEClientRequesterBuilder.CreateBuilder()
    .AddRequest<MathRequestService>()   // 从 [Request]/[Response] 自动获取路由并注册
    .Build(options =>
    {
        options.RequestAddress = "http://localhost:3300";
    });

// 通过路由路径调用
var result1 = await requester.Request<double>("math/add", 3.0, 5.0);
// 通过 Name 别名调用（等价）
var result2 = await requester.Request<double>("add", 3.0, 5.0);
```

---

### 使用XFE标准请求

`UseXFEStandardRequest<T>()` 扩展方法一键注册所有标准服务请求（登录、重登、IP 封禁、日志、连接检查）：

```csharp
using XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

var requester = XFEClientRequesterBuilder.CreateBuilder()
    .UseXFEStandardRequest<UserFaceInfo>()   // T 为登录返回的用户信息接口实现类
    .Build(options =>
    {
        options.RequestAddress = "http://localhost:3300";
    });

// 登录
var loginResult = await requester.Request<UserLoginResult<UserFaceInfo>>("login", "Admin", "123456");
if (loginResult.StatusCode == HttpStatusCode.OK)
{
    Console.WriteLine($"Session: {loginResult.Result.Session}");
    Console.WriteLine($"过期时间: {loginResult.Result.ExpireDate}");
    Console.WriteLine($"昵称: {loginResult.Result.UserInfo.NickName}");
    requester.Session = loginResult.Result.Session;  // 保存 Session 供后续请求使用
}

// 重新登录（使用 Session + 设备信息自动验证）
var reloginResult = await requester.Request<UserFaceInfo>("relogin");

// 检查连接
var checkResult = await requester.Request<DateTime>("check_connect");

// 获取日志
var logResult = await requester.Request<string>("get_log", DateTime.MinValue, DateTime.MaxValue);

// IP 封禁管理
// 说明：AddBannedIPRequest() 注册的调用名称为 get_bannedIPList / add_bannedIP / remove_bannedIP
// 对应服务端实际路由为 ip/banned/get / ip/banned/add / ip/banned/remove
var bannedIPs = await requester.Request<List<IPAddressInfo>>("get_bannedIPList");
await requester.Request<string>("add_bannedIP", "192.168.1.100", "恶意请求");
await requester.Request<bool>("remove_bannedIP", "192.168.1.100");
```

`UseXFEStandardRequest<T>()` 等价于：

```csharp
.AddLoginRequest<T>()        // 注册 login（→ user/login）、relogin（→ user/relogin）请求
.AddBannedIPRequest()        // 注册 get_bannedIPList、add_bannedIP、remove_bannedIP 请求
                             //   对应服务端路由：ip/banned/get、ip/banned/add、ip/banned/remove
.AddLogRequest()             // 注册 get_log（→ log/get）、clear_log（→ log/clear）请求
.AddCheckConnectRequest()    // 注册 check_connect 请求
```

---

### TableRequester（数据表请求器）

`TableRequester` 专门用于与服务端数据表管理器进行 CRUD 交互：

```csharp
var tableRequester = new TableRequester
{
    RequestAddress = "http://localhost:3300",
    Session = "your-session-here",
    DeviceInfo = DeviceHelper.GetUniqueHardwareId()
};

// 获取所有订单（无分页）
var result = await tableRequester.Get<Order>();
foreach (var order in result.DataList)
    Console.WriteLine($"ID:{order.Id}\tName:{order.Name}");

// 分页获取（每页 10 条，第 1 页）
var paged = await tableRequester.Get<Order>(pageCount: 10, page: 1);

// 添加数据
bool success = await tableRequester.Add(new Order { Name = "新订单", Description = "描述" });

// 修改数据（通过 Id 匹配）
var order = result.DataList[0];
order.Name = "修改后的名称";
await tableRequester.Change(order);

// 删除数据
await tableRequester.Remove<Order>(order.Id);
```

表名默认从类型名自动推断：类型名首字母小写即为请求表名（`TableNameInRequest`），例如 `Order` → `order`。  
这是 `TableRequester` 与服务端通信时实际使用的名称，与 `AddTable` 时指定的 `tableShowName`（显示名称，如 `"订单"`）不同。

```csharp
// 默认：Order 类型 → 请求表名自动推断为 "order"（类型名首字母小写）
await tableRequester.Get<Order>();

// 手动指定请求表名（需与服务端 TableNameInRequest 一致，即类型名首字母小写）
// 注意：这里填写的是请求路径中使用的表名，不是 AddTable 时的 tableShowName
await tableRequester.Get<Order>("order", pageCount: 10, page: 1);
```

数据模型需实现 `IIdModel` 接口（包含 `string Id` 属性）：

```csharp
public class Order : IIdModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

---

## 增量生成器（Source Generator）

本库提供了两个增量生成器，在编译期自动为服务类和请求类生成路由字典，无需手动维护。

### 服务端：EntryPoint 生成器

自动为继承 `ServerCoreStandardServiceBase` 的 `partial` 类生成 `SyncEntryPoints` 和 `AsyncEntryPoints` 字典。

**使用要求：**
- 类必须声明为 `partial`
- 方法返回类型必须为 `void`（同步）或 `Task`/`Task<T>`（异步）
- 方法不能有参数
- 路由路径不能包含引号（`"`）或反斜杠（`\`）
- 通配符 `*` 必须是独立的路径段（不能与其他字符混合）
- 同一个类中同一个路径只能有一个处理方法

**示例：**

```csharp
public partial class ApiService : ServerCoreStandardServiceBase
{
    [EntryPoint("user/profile")]
    public async Task GetProfile()
    {
        // ...
    }

    [EntryPoint("user/update")]
    public void UpdateUser()
    {
        // ...
    }

    // 通配符路由：匹配 resource/任意/details
    [EntryPoint("resource/*/details")]
    public async Task ResourceDetails()
    {
        // Route 包含实际匹配的路径
        await Close($"Resource detail for {Route}");
    }
}
```

生成器自动生成（无需手写）：

```csharp
// ApiService.EntryPoints.g.cs（自动生成，仅展示）
public override Dictionary<string, Action> SyncEntryPoints => new()
{
    { "user/update", UpdateUser },
};

public override Dictionary<string, Func<Task>> AsyncEntryPoints => new()
{
    { "user/profile", GetProfile },
    { "resource/*/details", ResourceDetails },
};
```

---

### 客户端：ClientRequest 生成器

自动为继承 `StandardRequestServiceBase` 的 `partial` 类生成 `RequestPoints`、`ResponsePoints` 和 `RequestRouteMap` 字典。

**使用要求：**
- 类必须声明为 `partial`
- 方法返回类型必须为 `object`
- 方法不能有参数
- 同一个类中同一路径或名称（在 Request 或 Response 中）不能重复

**示例：**

```csharp
public partial class UserRequestService<T> : StandardRequestServiceBase where T : IUserFaceInfo
{
    // Path 为路由路径，Name 为可选的请求别名（调用时可用 Path 或 Name）
    [Request("login", Name = "login")]
    public object LoginRequest() => new
    {
        execute = "login",
        account = Parameters[0],
        password = Parameters[1],
        deviceInfo = DeviceInfo
    };

    [Response("login", Name = "login")]
    public object LoginResponse() => JsonSerializer.Deserialize<UserLoginResult<T>>(UnescapedResponse)!;

    [Request("relogin", Name = "relogin")]
    public object ReloginRequest() => new
    {
        execute = "relogin",
        session = Session,
        deviceInfo = DeviceInfo
    };

    [Response("relogin", Name = "relogin")]
    public object ReloginResponse() => JsonSerializer.Deserialize<T>(UnescapedResponse)!;
}
```

---

### 诊断规则

增量生成器会在编译期检查代码并报告诊断信息：

| 代码 | 说明 | 适用范围 |
|------|------|----------|
| `XFE0003` | 包含 `[EntryPoint]` 方法的类必须为 `partial` | 服务端 |
| `XFE0004` | `[EntryPoint]` 方法不能有参数 | 服务端 |
| `XFE0005` | `[EntryPoint]` 方法返回类型必须为 `void` 或 `Task` | 服务端 |
| `XFE0006` | `[EntryPoint]` 路径包含无效字符（引号或反斜杠） | 服务端 |
| `XFE0007` | 包含 `[Request]`/`[Response]` 方法的类必须为 `partial` | 客户端 |
| `XFE0008` | `[Request]`/`[Response]` 方法不能有参数 | 客户端 |
| `XFE0009` | `[Request]`/`[Response]` 方法返回类型必须为 `object` | 客户端 |
| `XFE0010` | `[Request]`/`[Response]` 路径包含无效字符 | 客户端 |
| `XFE0011` | `[Request]`/`[Response]` 路径或名称重复注册 | 客户端 |
| `XFE0012` | `[EntryPoint]` 路径在同一类中重复注册 | 服务端 |
| `XFE0013` | `[EntryPoint]` 通配符使用无效（`*` 必须是完整路径段） | 服务端 |

所有诊断均有对应的在线文档：`https://docs.xfegzs.com/View/Errors/ServerInteractive/XFE{代码}`

---

## 常见错误与解决方案

**Q：服务没有收到请求，控制台没有任何输出**
- 确认 `XFEServerCoreBuilder.Build(options => { options.BindIP("..."); })` 中绑定了正确的地址
- 如果设置了 `MainEntryPoint`，请求路径需要包含该前缀，例如设置 `api` 后请求 `/api/echo`

**Q：`InvalidOperationException: 类型 MyService 的 EntryPointList 为空`**
- 确认 `MyService` 继承了 `ServerCoreStandardServiceBase`
- 确认类声明了 `partial` 关键字
- 确认方法上有 `[EntryPoint("路径")]` 特性且路径不为空

**Q：`XFEServerBuilderException: 未添加核心处理器`**
- 在 `XFEServerBuilder.Build()` 前必须调用 `AddCoreProcessor<T>()`，或使用 `UseXFEServer()` 扩展方法

**Q：路由匹配到了不期望的服务**
- 框架按"最具体优先"原则：字面量段越多的路由模式优先级越高，与注册顺序无关
- 精确路由（无通配符）总是优先于通配符路由

**Q：`[Request]`/`[Response]` 方法的返回类型报错**
- 方法必须声明返回类型为 `object`（不能是 `string`、`int` 等具体类型）

**Q：客户端请求总是返回 500**
- 检查 `options.RequestAddress` 末尾是否有多余的斜杠（框架会自动拼接路由：`RequestAddress + "/" + route`）
- 确认服务端路由与请求名称完全一致（区分大小写）
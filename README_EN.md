# XFEExtension.NetCore.ServerInteractive

## Description

ServerInteractive is a C# DLL library built on the CyberComm network architecture provided by `XFEExtension.NetCore`. It enables developers to rapidly build server-side and client-side network frameworks. It supports route-based request dispatch, incremental code generation (Source Generator), user login system, data table management, IP banning, and other out-of-the-box standard features.

---

## Table of Contents

- [Quick Start](#quick-start)
- [Server Side](#server-side)
  - [Setting Up the Server](#setting-up-the-server)
  - [Defining Standard Core Services (Route Handling)](#defining-standard-core-services-route-handling)
  - [Wildcard Routes](#wildcard-routes)
  - [Using the XFE Standard Server Core](#using-the-xfe-standard-server-core)
  - [Manual Service Composition](#manual-service-composition)
  - [Original Services and Verify Services](#original-services-and-verify-services)
  - [Server Core Configuration Options](#server-core-configuration-options)
  - [Responding to Clients in Service Methods](#responding-to-clients-in-service-methods)
- [Client Side](#client-side)
  - [Setting Up a Requester](#setting-up-a-requester)
  - [Inline Request Registration](#inline-request-registration)
  - [Defining Standard Request Services (Source Generator)](#defining-standard-request-services-source-generator)
  - [Using XFE Standard Requests](#using-xfe-standard-requests)
  - [TableRequester (Data Table Requester)](#tablerequester-data-table-requester)
- [Source Generator](#source-generator)
  - [Server Side: EntryPoint Generator](#server-side-entrypoint-generator)
  - [Client Side: ClientRequest Generator](#client-side-clientrequest-generator)
  - [Diagnostic Rules](#diagnostic-rules)
- [Common Errors and Solutions](#common-errors-and-solutions)

---

## Quick Start

> Before using, add the `XFEExtension.NetCore.ServerInteractive` NuGet package to your project.

**Simplest server example** (an echo service bound to local port 3300):

**Service definition (EchoService.cs)**

```csharp
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

// The class must be partial so the Source Generator can automatically generate the route dictionary
public partial class EchoService : ServerCoreStandardServiceBase
{
    [EntryPoint("echo")]         // Route path: /echo
    public async Task Echo()
    {
        var message = Json?["message"]?.GetValue<string>() ?? string.Empty;
        await Close(message);    // Reply to the client and end the request
    }
}
```

**Program entry point (Program.cs)**

```csharp
using XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()                               // Register logger, exception handler, and core processor
    .AddServerCore(
        XFEServerCoreBuilder.CreateBuilder()
            .AddService<EchoService>()            // Register EchoService (routes are automatically obtained from [EntryPoint])
            .Build(options =>
            {
                options.BindIP("http://localhost:3300/");
            }))
    .Build();

await server.Start();
```

---

## Server Side

### Setting Up the Server

Use `XFEServerBuilder` to assemble the server.

```csharp
var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()                   // Use XFE standard server (includes log initialization, exception handling, core processor)
    .AddServerCore(serverCore)        // Add an XFEServerCore instance
    .Build();                         // Build the XFEServer instance

await server.Start();                 // Start all initializers, sync/async services, and core services
```

`UseXFEServer()` is equivalent to:

```csharp
.AddInitializer<ServerLogInitializer>()      // Log initializer
.AddService<ServerExceptionProcessService>() // Global exception handler
.AddCoreProcessor<XFEServerCoreProcessService>() // Core request processor
```

You can also register only custom services without using the standard suite:

```csharp
var server = XFEServerBuilder.CreateBuilder()
    .AddInitializer<MyInitializerService>()  // Custom initializer service (runs once before the server starts)
    .AddService<MyService>()                 // Custom synchronous service (runs when the server starts)
    .AddAsyncService<MyAsyncService>()       // Custom asynchronous service (runs asynchronously when the server starts)
    .AddCoreProcessor<MyCoreProcessor>()     // A core processor must be added
    .Build();
```

---

### Defining Standard Core Services (Route Handling)

Inherit `ServerCoreStandardServiceBase` and mark methods with `[EntryPoint("route path")]`.  
The class must be declared as `partial` for the Source Generator to automatically generate the route dictionary.

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
        await Close(a + b);     // Return result and close the request
    }

    [EntryPoint("math/subtract")]
    public void Subtract()
    {
        var a = Json?["a"]?.GetValue<double>() ?? 0;
        var b = Json?["b"]?.GetValue<double>() ?? 0;
        OK();                   // Mark as handled
        // Synchronous methods are also supported; the return type is void
    }
}
```

Register in `XFEServerCoreBuilder`:

```csharp
XFEServerCoreBuilder.CreateBuilder()
    .AddService<MathService>()   // Automatically reads route paths from [EntryPoint] and registers
    .Build(options => { options.BindIP("http://localhost:3300/"); });
```

**Commonly accessible properties in service methods:**

| Property | Type | Description |
|----------|------|-------------|
| `Route` | `string` | The route path of the current request |
| `ClientIP` | `string` | The client's IP address |
| `Json` | `QueryableJsonNode?` | The parsed JSON node of the request body |
| `Args` | `CyberCommRequestEventArgs` | The raw request event args (includes Headers, Request, etc.) |
| `XFEServerCore` | `XFEServerCore` | The server core instance this service belongs to |
| `Handled` | `bool` | Whether the request has been handled |
| `IsStandardError` | `bool` | Whether this is a standard error response |

---

### Wildcard Routes

An independent `*` in a route segment can match any single path segment, and multi-level wildcards are supported.  
The framework prioritizes patterns with more literal segments (most-specific-first principle), regardless of registration order.

```csharp
public partial class DynamicService : ServerCoreStandardServiceBase
{
    // Matches v1/anything/info, e.g., v1/users/info or v1/orders/info
    [EntryPoint("v1/*/info")]
    public async Task DynamicInfo()
    {
        // The Route property contains the actual request path, e.g., v1/users/info
        await Close($"Your route is: {Route}, your IP is: {ClientIP}");
    }

    // Matches paths under v1/test/* (* only matches a single path segment), e.g., v1/test/a, v1/test/hello
    [EntryPoint("v1/test/*")]
    public async Task TestAny()
    {
        await Close($"Server: {XFEServerCore.ServerCoreName}, Route: {Route}");
    }
}
```

> **Note:** The wildcard `*` must be a complete path segment and cannot be mixed with other characters (e.g., `a*b` is invalid).

---

### Using the XFE Standard Server Core

The `UseXFEStandardServerCore<T>()` extension method provides a complete standardized server core with built-in:
- User login (server route `user/login`, client can call using alias `login`), auto re-login (route `user/relogin`, alias `relogin`)
- Data table CRUD management (`table/get/{tableName}`, `table/add/{tableName}`, `table/change/{tableName}`, `table/remove/{tableName}`)
- IP banning (`ip/banned/get`, `ip/banned/add`, `ip/banned/remove`), daily counting
- Connection check (`check_connect`), server logging (route `log/get`, alias `get_log`; route `log/clear`, alias `clear_log`)

```csharp
var serverCore = XFEServerCoreBuilder.CreateBuilder()
    .UseXFEStandardServerCore<IUserFaceInfo>(options =>
    {
        // Provide user data source
        options.GetUserFunction = () => UserProfile.UserTable;
        options.AddUserFunction = user => UserProfile.UserTable.Add((User)user);
        options.GetEncryptedUserLoginModelFunction = () => UserProfile.EncryptedUserLoginModelTable;
        options.AddEncryptedUserLoginModelFunction = UserProfile.EncryptedUserLoginModelTable.Add;
        options.RemoveEncryptedUserLoginModelFunction = model => UserProfile.EncryptedUserLoginModelTable.Remove(model);
        options.GetLoginKeepDays = () => 7;  // Number of days to keep the login session

        // Convert the User object to the interface type returned to the client after a successful login
        options.LoginResultConvertFunction = user => (IUserFaceInfo)user;

        // Configure the data table manager
        options.DataTableManagerBuilder = XFEDataTableManagerBuilder.CreateBuilder()
            // Parameters: display name, add permission, remove permission, change permission, get permission
            .AddTable<Order, DataProfile>("Orders", addPermissionLevel: 1, removePermissionLevel: 2, changePermissionLevel: 1, getPermissionLevel: 1)
            .AddTable<User, UserProfile>("Users", addPermissionLevel: 2, removePermissionLevel: 2, changePermissionLevel: 2, getPermissionLevel: 1);
    })
    .AddService<MyCustomService>()   // Append custom business services
    .Build(options =>
    {
        options.MainEntryPoint = "api";     // Route prefix; the full path for a login request would be /api/user/login
        options.AcceptPost = true;
        options.AcceptGet = true;
        options.BindIP("http://localhost:3300/")
               .BindIP("https://localhost:3301/");
    });
```

Complete server example (with TServer reference implementation):

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
                    .AddTable<Order, DataProfile>("Orders", 1, 2, 1, 1)
                    .AddTable<Person, DataProfile>("Persons", 1, 2, 1, 1)
                    .AddTable<User, UserProfile>("Users", 2, 2, 2, 1);
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

### Manual Service Composition

If you don't need the standard suite, you can add individual services as needed:

```csharp
XFEServerCoreBuilder.CreateBuilder()
    // Register a standard service (routes are automatically read from [EntryPoint])
    .AddService<MyRouteService>()
    // Explicitly specify a route (useful for dynamic routes or when not using a Source Generator)
    .AddServiceWithRoute<MyDynamicService>("dynamic/*/process")
    // Original service (listens for server start events and all raw requests)
    .AddOriginalService<MyRawRequestHandler>()
    // Verify service (validates each request before route dispatch; returning false intercepts the request)
    .AddVerifyService<MyAuthVerifyService>()
    // Add user base parameters (used by login-related services)
    .AddUserParameterBase(getUserFn, addUserFn, getEncryptedModelFn, addEncryptedModelFn, removeEncryptedModelFn, getKeepDaysFn, convertFn)
    // Add a data table manager
    .AddDataTableManager(tableManagerBuilder, getUserFn, getEncryptedModelFn)
    // Add individual built-in services
    .AddEntryPointVerify()          // Entry point validation service
    .AddDailyCounterService()       // Daily request statistics
    .AddXFEErrorProcessService()    // Exception handling service
    .AddConnectService()            // Connection check service (check_connect route)
    .AddStandardLoginService<IUserFaceInfo>()  // Standard login service (login, relogin routes)
    .AddServerLogService()          // Server log query (get_log route)
    .AddIPBannerService()           // IP banning service (get_bannedIPList, add_bannedIP, remove_bannedIP routes)
    .Build(options =>
    {
        options.BindIP("http://localhost:3300/");
    });
```

---

### Original Services and Verify Services

**Original Service (`IServerCoreOriginalService`)**: Inherit `ServerCoreOriginalServiceBase` to listen for server start events and every raw request (before route dispatch).

```csharp
using XFEExtension.NetCore.CyberComm;
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

public class MyRawService : ServerCoreOriginalServiceBase
{
    public override void ServerStarted(object? sender, EventArgs e)
    {
        Console.WriteLine("Server has started!");
    }

    public override void RequestReceived(object? sender, CyberCommRequestEventArgs e)
    {
        Console.WriteLine($"Raw request received: {e.Request.Url}");
    }
}
```

**Verify Service (`IServerCoreVerifyService`)**: Inherit `ServerCoreVerifyServiceBase`. Every request is validated before route dispatch. Returning `false` or `Task<false>` interrupts further processing.

```csharp
using XFEExtension.NetCore.ServerInteractive.Implements.CoreService;

public class MyAuthService : ServerCoreVerifyServiceBase
{
    public override bool VerifyRequest()
    {
        // Synchronous validation; returning false intercepts the request
        var token = Args.Request.Headers["X-Token"];
        return !string.IsNullOrEmpty(token);
    }

    public override async Task<bool> VerifyRequestAsync()
    {
        // Asynchronous validation; returns true by default (does not intercept)
        return true;
    }
}
```

---

### Server Core Configuration Options

`XFEServerCoreBuilder.Build(options => { ... })` accepts `XFEServerCoreOptions`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BindingIPAddress` | `List<string>` | `[]` | List of URLs to bind the server to |
| `ServerCoreName` | `string` | Auto-generated | Server core name (used in logs) |
| `MainEntryPoint` | `string` | `""` | Main entry prefix (empty means matching starts from sub-routes directly) |
| `AcceptGet` | `bool` | `false` | Whether to accept GET requests |
| `AcceptPost` | `bool` | `true` | Whether to accept POST requests |
| `AutoUnescapeJson` | `bool` | `true` | Whether to automatically unescape JSON |
| `AcceptNonStandardJson` | `bool` | `true` | Whether to accept non-standard JSON request bodies |
| `GetIPFunction` | `Func<CyberCommRequestEventArgs, string>` | Reads from `ClientIP` | Custom IP retrieval function (e.g., for proxy scenarios) |

```csharp
.Build(options =>
{
    options.ServerCoreName = "MainServer";
    options.MainEntryPoint = "api";    // Request path format: /api/{sub-route}
    options.AcceptGet = true;
    options.AcceptPost = true;
    options.GetIPFunction = args => args.Request.Headers["X-Forwarded-For"] ?? args.ClientIP;
    options.BindIP("http://localhost:3300/")
           .BindIP("https://localhost:3301/");
});
```

---

### Responding to Clients in Service Methods

In subclasses of `ServerCoreStandardServiceBase`, the following methods are available for responding:

```csharp
// Send data (without closing the connection)
await Send("message text");
await Send(new { code = 0, data = "ok" }); // Automatically serialized to JSON

// Send data and close the request (most common)
await Close("response content");
await Close(new { result = 42 });

// Mark the request as handled (for synchronous methods, sends no response body)
OK();

// Return an error message
Error("Missing parameters", HttpStatusCode.BadRequest);
await CloseWithError("Unauthorized", HttpStatusCode.Unauthorized);
```

---

## Client Side

### Setting Up a Requester

Use `XFEClientRequesterBuilder` to build an `XFEClientRequester`:

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
        options.RequestAddress = "http://localhost:3300";  // Server address
        options.Session = string.Empty;                    // User session (updated after login)
        options.DeviceInfo = DeviceHelper.GetUniqueHardwareId(); // Unique device identifier
    });

// Make a request
var result = await requester.Request<string>("echo", "Hello, World!");
if (result.StatusCode == HttpStatusCode.OK)
    Console.WriteLine(result.Result);  // Output: Hello, World!
else
    Console.WriteLine($"Failed: {result.StatusCode} {result.Message}");
```

`XFEClientRequesterOptions` properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RequestAddress` | `string` | `"http://localhost:3300/"` | Server request address |
| `Session` | `string` | `""` | Current user session |
| `DeviceInfo` | `string` | Unique hardware ID | Device information |

---

### Inline Request Registration

`AddRequest(route, constructBody, processResponse)` is suitable for simple one-off requests:

```csharp
var requester = XFEClientRequesterBuilder.CreateBuilder()
    // Request with no parameters
    .AddRequest("status", (_, _, _) => new { execute = "status" }, response => response)
    // Request with parameters
    .AddRequest("math/add", (_, _, parameters) => new
    {
        execute = "math/add",
        a = parameters.Length > 0 ? parameters[0] : 0,
        b = parameters.Length > 1 ? parameters[1] : 0
    }, response => response)
    // Login request example with session and deviceInfo
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

- `constructBody`: `(string session, string deviceInfo, object[] parameters) => object` — returns the request body object (automatically JSON-serialized).
- `processResponse`: `(string response) => object` — parses the response string; can be `null` (no processing).

---

### Defining Standard Request Services (Source Generator)

Inherit `StandardRequestServiceBase` and use the `[Request]`/`[Response]` attributes.  
The class must be declared as `partial`.

```csharp
using XFEExtension.NetCore.ServerInteractive.Attributes;
using XFEExtension.NetCore.ServerInteractive.Implements.Requester;

public partial class MathRequestService : StandardRequestServiceBase
{
    // [Request] marks the method that constructs the request body; Path is the route path, Name is an optional call alias
    [Request("math/add", Name = "add")]
    public object BuildAddRequest() => new
    {
        execute = "math/add",
        a = Parameters.Length > 0 ? Parameters[0] : 0,
        b = Parameters.Length > 1 ? Parameters[1] : 0
    };

    // [Response] marks the method that parses the response; Path and Name correspond to those in [Request]
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

Properties accessible in `[Request]`/`[Response]` methods:

| Property | Description |
|----------|-------------|
| `Parameters` | The parameter array passed when calling `Request(name, params object[] parameters)` |
| `Session` | The current user session |
| `DeviceInfo` | Device information |
| `Response` | The raw response string |
| `UnescapedResponse` | The unescaped response string |
| `Route` | The actual request route path |

Register with the requester:

```csharp
var requester = XFEClientRequesterBuilder.CreateBuilder()
    .AddRequest<MathRequestService>()   // Automatically reads routes from [Request]/[Response] and registers
    .Build(options =>
    {
        options.RequestAddress = "http://localhost:3300";
    });

// Call using the route path
var result1 = await requester.Request<double>("math/add", 3.0, 5.0);
// Call using the Name alias (equivalent)
var result2 = await requester.Request<double>("add", 3.0, 5.0);
```

---

### Using XFE Standard Requests

The `UseXFEStandardRequest<T>()` extension method registers all standard service requests in one call (login, re-login, IP banning, logging, connection check):

```csharp
using XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;
using XFEExtension.NetCore.ServerInteractive.Utilities.Requester;

var requester = XFEClientRequesterBuilder.CreateBuilder()
    .UseXFEStandardRequest<UserFaceInfo>()   // T is the implementing class of the user info interface returned on login
    .Build(options =>
    {
        options.RequestAddress = "http://localhost:3300";
    });

// Login
var loginResult = await requester.Request<UserLoginResult<UserFaceInfo>>("login", "Admin", "123456");
if (loginResult.StatusCode == HttpStatusCode.OK)
{
    Console.WriteLine($"Session: {loginResult.Result.Session}");
    Console.WriteLine($"Expiry: {loginResult.Result.ExpireDate}");
    Console.WriteLine($"Nickname: {loginResult.Result.UserInfo.NickName}");
    requester.Session = loginResult.Result.Session;  // Save session for subsequent requests
}

// Re-login (automatically verified using Session + device info)
var reloginResult = await requester.Request<UserFaceInfo>("relogin");

// Check connection
var checkResult = await requester.Request<DateTime>("check_connect");

// Get logs
var logResult = await requester.Request<string>("get_log", DateTime.MinValue, DateTime.MaxValue);

// IP ban management
// Note: AddBannedIPRequest() registers call names get_bannedIPList / add_bannedIP / remove_bannedIP
// which correspond to server routes ip/banned/get / ip/banned/add / ip/banned/remove
var bannedIPs = await requester.Request<List<IPAddressInfo>>("get_bannedIPList");
await requester.Request<string>("add_bannedIP", "192.168.1.100", "Malicious request");
await requester.Request<bool>("remove_bannedIP", "192.168.1.100");
```

`UseXFEStandardRequest<T>()` is equivalent to:

```csharp
.AddLoginRequest<T>()        // Register login (→ user/login) and relogin (→ user/relogin) requests
.AddBannedIPRequest()        // Register get_bannedIPList, add_bannedIP, remove_bannedIP requests
                             //   corresponding to server routes: ip/banned/get, ip/banned/add, ip/banned/remove
.AddLogRequest()             // Register get_log (→ log/get) and clear_log (→ log/clear) requests
.AddCheckConnectRequest()    // Register check_connect request
```

---

### TableRequester (Data Table Requester)

`TableRequester` is specifically designed for CRUD interactions with the server-side data table manager:

```csharp
var tableRequester = new TableRequester
{
    RequestAddress = "http://localhost:3300",
    Session = "your-session-here",
    DeviceInfo = DeviceHelper.GetUniqueHardwareId()
};

// Get all orders (no pagination)
var result = await tableRequester.Get<Order>();
foreach (var order in result.DataList)
    Console.WriteLine($"ID:{order.Id}\tName:{order.Name}");

// Paginated get (10 per page, page 1)
var paged = await tableRequester.Get<Order>(pageCount: 10, page: 1);

// Add data
bool success = await tableRequester.Add(new Order { Name = "New Order", Description = "Description" });

// Change data (matched by Id)
var order = result.DataList[0];
order.Name = "Updated Name";
await tableRequester.Change(order);

// Remove data
await tableRequester.Remove<Order>(order.Id);
```

The table name is inferred from the type name by default: the type name with its first letter lowercased is used as the request table name (`TableNameInRequest`). For example, `Order` → `order`.  
This is the name actually used by `TableRequester` when communicating with the server, which differs from the `tableShowName` (display name, e.g., `"Orders"`) specified in `AddTable`.

```csharp
// Default: Order type → request table name is automatically inferred as "order" (type name with first letter lowercased)
await tableRequester.Get<Order>();

// Manually specify the request table name (must match the server-side TableNameInRequest, i.e., the type name with first letter lowercased)
// Note: This is the table name used in the request path, not the tableShowName specified in AddTable
await tableRequester.Get<Order>("order", pageCount: 10, page: 1);
```

Data models must implement the `IIdModel` interface (which includes a `string Id` property):

```csharp
public class Order : IIdModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

---

## Source Generator

This library provides two incremental source generators that automatically generate route dictionaries for service classes and request classes at compile time, eliminating the need for manual maintenance.

### Server Side: EntryPoint Generator

Automatically generates `SyncEntryPoints` and `AsyncEntryPoints` dictionaries for `partial` classes that inherit `ServerCoreStandardServiceBase`.

**Requirements:**
- The class must be declared as `partial`
- Method return types must be `void` (synchronous) or `Task`/`Task<T>` (asynchronous)
- Methods must not have parameters
- Route paths must not contain quotes (`"`) or backslashes (`\`)
- The wildcard `*` must be a complete path segment (cannot be mixed with other characters)
- A single class cannot have more than one handler for the same route path

**Example:**

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

    // Wildcard route: matches resource/anything/details
    [EntryPoint("resource/*/details")]
    public async Task ResourceDetails()
    {
        // Route contains the actual matched path
        await Close($"Resource detail for {Route}");
    }
}
```

The generator automatically produces (no manual writing required):

```csharp
// ApiService.EntryPoints.g.cs (auto-generated, shown for illustration only)
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

### Client Side: ClientRequest Generator

Automatically generates `RequestPoints`, `ResponsePoints`, and `RequestRouteMap` dictionaries for `partial` classes that inherit `StandardRequestServiceBase`.

**Requirements:**
- The class must be declared as `partial`
- Method return types must be `object`
- Methods must not have parameters
- The same path or name (within `[Request]` or `[Response]`) cannot be registered more than once within a single class

**Example:**

```csharp
public partial class UserRequestService<T> : StandardRequestServiceBase where T : IUserFaceInfo
{
    // Path is the route path; Name is an optional request alias (you can call using either Path or Name)
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

### Diagnostic Rules

The incremental generators check code at compile time and report diagnostics:

| Code | Description | Scope |
|------|-------------|-------|
| `XFE0003` | Classes containing `[EntryPoint]` methods must be `partial` | Server side |
| `XFE0004` | `[EntryPoint]` methods must not have parameters | Server side |
| `XFE0005` | `[EntryPoint]` method return type must be `void` or `Task` | Server side |
| `XFE0006` | `[EntryPoint]` path contains invalid characters (quotes or backslashes) | Server side |
| `XFE0007` | Classes containing `[Request]`/`[Response]` methods must be `partial` | Client side |
| `XFE0008` | `[Request]`/`[Response]` methods must not have parameters | Client side |
| `XFE0009` | `[Request]`/`[Response]` method return type must be `object` | Client side |
| `XFE0010` | `[Request]`/`[Response]` path contains invalid characters | Client side |
| `XFE0011` | `[Request]`/`[Response]` path or name is registered more than once | Client side |
| `XFE0012` | `[EntryPoint]` path is registered more than once in the same class | Server side |
| `XFE0013` | Invalid wildcard usage in `[EntryPoint]` (`*` must be a complete path segment) | Server side |

All diagnostics have corresponding online documentation at: `https://docs.xfegzs.com/View/Errors/ServerInteractive/XFE{code}`

---

## Common Errors and Solutions

**Q: The service is not receiving requests and there is no console output**
- Confirm that the correct address is bound in `XFEServerCoreBuilder.Build(options => { options.BindIP("..."); })`
- If `MainEntryPoint` is set, request paths must include that prefix — for example, if `api` is set, request `/api/echo`

**Q: `InvalidOperationException: EntryPointList for type MyService is empty`**
- Confirm that `MyService` inherits `ServerCoreStandardServiceBase`
- Confirm that the class is declared with the `partial` keyword
- Confirm that the method has a `[EntryPoint("path")]` attribute with a non-empty path

**Q: `XFEServerBuilderException: No core processor has been added`**
- Before calling `XFEServerBuilder.Build()`, you must call `AddCoreProcessor<T>()`, or use the `UseXFEServer()` extension method

**Q: A route is matching an unexpected service**
- The framework uses the "most-specific-first" principle: route patterns with more literal segments have higher priority, regardless of registration order
- Exact routes (without wildcards) always take priority over wildcard routes

**Q: The return type of a `[Request]`/`[Response]` method is causing an error**
- The method must declare `object` as its return type (it cannot be `string`, `int`, or any other specific type)

**Q: Client requests always return 500**
- Check whether `options.RequestAddress` has a trailing slash (the framework automatically concatenates the route: `RequestAddress + "/" + route`)
- Confirm that the server-side route exactly matches the request name (case-sensitive)

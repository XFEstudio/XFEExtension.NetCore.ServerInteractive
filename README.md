# XFEExtension.NetCore.ServerInteractive

## 描述

ServerInteractive是一个C#的DLL库，其负责搭建以XFEExtension.NetCore提供的CyberComm网络架构，让使用者可以快速构建服务器和客户端网络框架

# 示例（使用前记得进行相应的引用）

---

## 搭建服务器端网络框架

#### 使用XFE标准网络架构

```csharp
var server = XFEServerBuilder.CreateBuilder() // 创建服务器构建器
    .UseXFEServer()                           // 使用XFE服务器架构
    .AddCoreServer(                           // 添加核心服务器
                   XFEServerCoreBuilder.CreateBuilder()                                           // 创建核心服务器构建器
                                       .UseXFEStandardServerCore(                                 // 使用XFE标准服务器核心
                                                static () => UserProfile.UserTable,               // 获取用户表的方法
                                                static () => UserProfile.EncryptedUserLoginModelTable,  // 获取用户加密模型的方法
                                                UserProfile.EncryptedUserLoginModelTable.Add,           // 添加用户加密模型的方法
                                                static user => UserProfile.EncryptedUserLoginModelTable.Remove(user),   // 移除用户加密模型的方法
                                                XFEDataTableManagerBuilder.CreateBuilder()              // 创建DataTable管理器构建器
                                                                          .AddTable<Person, DataProfile>("人物", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员) // 添加名为人物的表格，Person类型，AutoConfig为DataProfile。添加更改获取权限为业务员，移除权限为经理
                                                                          .AddTable<Order, DataProfile>("订单", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)  // 添加名为订单的表格，Order类型，AutoConfig为DataProfile。添加更改获取权限为业务员，移除权限为经理
                                                                          .AddTable<User, UserProfile>("用户", (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.业务员))      // 添加名为用户的表格，User类型，AutoConfig为UserProfile。获取权限为业务员，添加移除更改权限为经理
                                       .Build()) // 构建核心服务器
    .Build(); // 构建服务器
await server.Start(); // 启动服务
```

#### 添加自定义服务

```csharp
var server = XFEServerBuilder.CreateBuilder()
                             .AddInitializer<MyInitilizerService>() // 自定义初始化服务
                             .AddService<MyService>()               // 自定义服务
                             .AddAsyncService<MyAsyncService>()     // 自定义异步服务
                             .Build();
```

---

## 搭建客户端请求器

#### 使用XFE标准请求器

```csharp
var xFEClientRequester = XFEClientRequesterBuilder.CreateBuilder("http://localhost:8080/", string.Empty, DeviceHelper.GetUniqueHardwareId()) // 服务器IP地址，用户Session以及硬件编码
                                                  .UseXFEStandardRequest() // 使用XFE标准请求器
                                                  .Build();                // 构建客户端请求器

var result = await xFEClientRequester.Request<(string session, DateTime expireDate)>("login", account, password); // 调用login方法
if (result.StatusCode == System.Net.HttpStatusCode.OK)
{
    Console.WriteLine(result.Result.session);
    Console.WriteLine(result.Result.expireDate);
}
else
{
    Console.WriteLine(result.StatusCode);
    Console.WriteLine(result.Message);
}
```
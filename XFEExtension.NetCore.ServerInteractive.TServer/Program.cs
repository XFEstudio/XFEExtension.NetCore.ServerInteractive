using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.TServer;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.TServer.Profiles;
using XFEExtension.NetCore.ServerInteractive.TServer.Services;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Extensions;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;
using UserProfile = XFEExtension.NetCore.ServerInteractive.TServer.Profiles.UserProfile;

var server = XFEServerBuilder.CreateBuilder() // 创建服务器构建器
    .UseXFEServer()                           // 使用XFE服务器架构
    .AddServerCore(                           // 添加核心服务器
                   XFEServerCoreBuilder.CreateBuilder()                // 创建核心服务器构建器
                                       .UseXFEStandardServerCore<IUserFaceInfo>(options =>
                                       {                               // 使用XFE标准服务器核心
                                           options.GetUserFunction = static () => UserProfile.UserTable;                                                         // 获取用户表的方法
                                           options.GetEncryptedUserLoginModelFunction = static () => UserProfile.EncryptedUserLoginModelTable;                   // 获取用户加密模型的方法
                                           options.AddEncryptedUserLoginModelFunction = UserProfile.EncryptedUserLoginModelTable.Add;                            // 添加用户加密模型的方法
                                           options.RemoveEncryptedUserLoginModelFunction = static user => UserProfile.EncryptedUserLoginModelTable.Remove(user); // 移除用户加密模型的方法
                                           options.GetLoginKeepDays = static () => 7;                                                                            // 获取登录维持天数方法
                                           options.LoginResultConvertFunction = static user => (IUserFaceInfo)user;                                              // 登录结果转换方法
                                           options.DataTableManagerBuilder = XFEDataTableManagerBuilder.CreateBuilder()                                          // 创建DataTable管理器构建器
                                               .AddTable<Person, DataProfile>("人物", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员) // 添加名为人物的格，   Person 类    型，      AutoConfig为     DataProfile。添加更改获取权限为业务员，移除权限为经理
                                               .AddTable<Order, DataProfile>("订单", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)  // 添加名为订单的表格Order    类   型，     AutoConfig   为      DataProfile。添加更改获取权限为业务员，移除权限为经理
                                               .AddTable<User, UserProfile>("用户", (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.业务员);      // 添加名为用户的表格User类    型，       AutoConfig为         UserProfile。获取权限为业务员，添加移除更改权限为经理
                                       })
                                       .AddService<TestCoreService>()
                                       .Build(options =>
                                       {
                                           options.BindIP("http://localhost:3305/")
                                                  .BindIP("https://localhost:3306/");
                                       })) // 构建核心服务器
    .Build(); // 构建服务器
await server.Start(); // 启动服务
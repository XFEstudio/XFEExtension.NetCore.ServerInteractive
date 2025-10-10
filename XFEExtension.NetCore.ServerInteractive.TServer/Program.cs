using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.TServer;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.TServer.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

var server = XFEServerBuilder.CreateBuilder() //创建服务器构建器
    .UseXFEServer()                           //使用XFE服务器架构
    .AddCoreServer(                           //添加核心服务器
                   XFEServerCoreBuilder.CreateBuilder()                                           //创建核心服务器构建器
                                       .UseXFEStandardServerCore<User, IUserFaceInfo>(            //使用XFE标准服务器核心 泛型：<存储的用户类型, 登录返回的用户模型>
                                                static () => UserProfile.UserTable,               //获取用户表的方法
                                                static () => UserProfile.EncryptedUserLoginModelTable,  //获取用户加密模型的方法
                                                UserProfile.EncryptedUserLoginModelTable.Add,           //添加用户加密模型的方法
                                                static user => UserProfile.EncryptedUserLoginModelTable.Remove(user),   //移除用户加密模型的方法
                                                static () => 7,                                   // 获取登录维持天数方法
                                                static user => user,                        //登录返回用户转换方法
                                                XFEDataTableManagerBuilder.CreateBuilder()              //创建DataTable管理器构建器
                                                                          .AddTable<Person, DataProfile>("人物", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员) //添加名为人物的表格，Person类型，AutoConfig为DataProfile。添加更改获取权限为业务员，移除权限为经理
                                                                          .AddTable<Order, DataProfile>("订单", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)  //添加名为订单的表格，Order类型，AutoConfig为DataProfile。添加更改获取权限为业务员，移除权限为经理
                                                                          .AddTable<User, UserProfile>("用户", (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.业务员))  //添加名为用户的表格，User类型，AutoConfig为UserProfile。获取权限为业务员，添加移除更改权限为经理
                                       .Build()) //构建核心服务器
    .Build(); //构建服务器
await server.Start(); //启动服务
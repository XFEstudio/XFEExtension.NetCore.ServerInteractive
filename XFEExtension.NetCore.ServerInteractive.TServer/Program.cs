using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.TServer;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.TServer.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()
    .AddCoreServer(XFEServerCoreBuilder.CreateBuilder()
                                       .UseXFEStandardServerCore(static () => UserProfile.UserTable,
                                       static () => UserProfile.EncryptedUserLoginModelTable,
                                       UserProfile.EncryptedUserLoginModelTable.Add,
                                       static user => UserProfile.EncryptedUserLoginModelTable.Remove(user),
                                       XFEDataTableManagerBuilder.CreateBuilder()
                                                                 .AddTable<Person, DataProfile>("人物", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)
                                                                 .AddTable<Order, DataProfile>("订单", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)
                                                                 .AddTable<User, UserProfile>("用户", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员))
                                       .Build())
    .Build();
await server.Start();
using XFEExtension.NetCore.ServerInteractive.Interfaces;
using XFEExtension.NetCore.ServerInteractive.Models.UserModels;
using XFEExtension.NetCore.ServerInteractive.TServer;
using XFEExtension.NetCore.ServerInteractive.TServer.Models;
using XFEExtension.NetCore.ServerInteractive.TServer.Profiles;
using XFEExtension.NetCore.ServerInteractive.Utilities;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

var options = new XFEStandardServerCoreOptions<IUserFaceInfo>
{
    GetUserFunction = static () => UserProfile.UserTable,
    GetEncryptedUserLoginModelFunction = static () => UserProfile.EncryptedUserLoginModelTable,
    AddEncryptedUserLoginModelFunction = UserProfile.EncryptedUserLoginModelTable.Add,
    RemoveEncryptedUserLoginModelFunction = static user => UserProfile.EncryptedUserLoginModelTable.Remove(user),
    GetLoginKeepDays = static () => 7,
    LoginResultConvertFunction = static user => (IUserFaceInfo)user,
    DataTableManagerBuilder = XFEDataTableManagerBuilder.CreateBuilder()
        .AddTable<Person, DataProfile>("人物", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)
        .AddTable<Order, DataProfile>("订单", (int)UserRole.业务员, (int)UserRole.经理, (int)UserRole.业务员, (int)UserRole.业务员)
        .AddTable<User, UserProfile>("用户", (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.经理, (int)UserRole.业务员)
};

var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()
    .AddServerCore(
        XFEServerCoreBuilder.CreateBuilder()
            .UseXFEStandardServerCore(options)
            .Build())
    .Build();

await server.Start();

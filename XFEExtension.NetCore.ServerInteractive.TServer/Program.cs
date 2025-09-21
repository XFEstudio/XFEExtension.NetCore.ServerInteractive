using XFEExtension.NetCore.ServerInteractive.TServer;
using XFEExtension.NetCore.ServerInteractive.Utilities;
using XFEExtension.NetCore.ServerInteractive.Utilities.DataTable;
using XFEExtension.NetCore.ServerInteractive.Utilities.Server;

var server = XFEServerBuilder.CreateBuilder()
    .UseXFEServer()
    .AddCoreServer(
    XFEServerCoreBuilder.CreateBuilder()
                        .UseXFEStandardServerCore(()=>UserProfile.UserList,
                        ()=>UserProfile.EncryptedUserLoginList,
                        UserProfile.EncryptedUserLoginList.Add,
                        user=>UserProfile.EncryptedUserLoginList.Remove(user),
                        XFEDataTableManagerBuilder.CreateBuilder()
                        .AddTable()).Build()).Build();
namespace XFEExtension.NetCore.ServerInteractive.TServer.Models;

/// <summary>
/// 用户角色
/// </summary>
public enum UserRole
{
    经理 = 6,
    审核员 = 5,
    业务员 = 4,
    操作员 = 3,
    理费员 = 2,
    会计员 = 1,
    无权限 = 0
}

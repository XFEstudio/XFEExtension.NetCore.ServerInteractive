# XFEExtension.NetCore.ServerInteractive

## ����

ServerInteractive��һ��C#��DLL�⣬�为����XFEExtension.NetCore�ṩ��CyberComm����ܹ�����ʹ���߿��Կ��ٹ����������Ϳͻ���������

# ʾ����ʹ��ǰ�ǵý�����Ӧ�����ã�

---

## ���������������

#### ʹ��XFE��׼����ܹ�

```csharp
var server = XFEServerBuilder.CreateBuilder() // ����������������
    .UseXFEServer()                           // ʹ��XFE�������ܹ�
    .AddCoreServer(                           // ��Ӻ��ķ�����
                   XFEServerCoreBuilder.CreateBuilder()                                           // �������ķ�����������
                                       .UseXFEStandardServerCore(                                 // ʹ��XFE��׼����������
                                                static () => UserProfile.UserTable,               // ��ȡ�û���ķ���
                                                static () => UserProfile.EncryptedUserLoginModelTable,  // ��ȡ�û�����ģ�͵ķ���
                                                UserProfile.EncryptedUserLoginModelTable.Add,           // ����û�����ģ�͵ķ���
                                                static user => UserProfile.EncryptedUserLoginModelTable.Remove(user),   // �Ƴ��û�����ģ�͵ķ���
                                                XFEDataTableManagerBuilder.CreateBuilder()              // ����DataTable������������
                                                                          .AddTable<Person, DataProfile>("����", (int)UserRole.ҵ��Ա, (int)UserRole.����, (int)UserRole.ҵ��Ա, (int)UserRole.ҵ��Ա) // �����Ϊ����ı��Person���ͣ�AutoConfigΪDataProfile����Ӹ��Ļ�ȡȨ��Ϊҵ��Ա���Ƴ�Ȩ��Ϊ����
                                                                          .AddTable<Order, DataProfile>("����", (int)UserRole.ҵ��Ա, (int)UserRole.����, (int)UserRole.ҵ��Ա, (int)UserRole.ҵ��Ա)  // �����Ϊ�����ı��Order���ͣ�AutoConfigΪDataProfile����Ӹ��Ļ�ȡȨ��Ϊҵ��Ա���Ƴ�Ȩ��Ϊ����
                                                                          .AddTable<User, UserProfile>("�û�", (int)UserRole.����, (int)UserRole.����, (int)UserRole.����, (int)UserRole.ҵ��Ա))      // �����Ϊ�û��ı��User���ͣ�AutoConfigΪUserProfile����ȡȨ��Ϊҵ��Ա������Ƴ�����Ȩ��Ϊ����
                                       .Build()) // �������ķ�����
    .Build(); // ����������
await server.Start(); // ��������
```

#### ����Զ������

```csharp
var server = XFEServerBuilder.CreateBuilder()
                             .AddInitializer<MyInitilizerService>() // �Զ����ʼ������
                             .AddService<MyService>()               // �Զ������
                             .AddAsyncService<MyAsyncService>()     // �Զ����첽����
                             .Build();
```

---

## ��ͻ���������

#### ʹ��XFE��׼������

```csharp
var xFEClientRequester = XFEClientRequesterBuilder.CreateBuilder("http://localhost:8080/", string.Empty, DeviceHelper.GetUniqueHardwareId()) // ������IP��ַ���û�Session�Լ�Ӳ������
                                                  .UseXFEStandardRequest() // ʹ��XFE��׼������
                                                  .Build();                // �����ͻ���������

var result = await xFEClientRequester.Request<(string session, DateTime expireDate)>("login", account, password); // ����login����
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
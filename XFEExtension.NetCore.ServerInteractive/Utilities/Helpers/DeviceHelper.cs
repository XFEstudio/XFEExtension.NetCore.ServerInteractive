using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

/// <summary>
/// 设备帮助
/// </summary>
[SupportedOSPlatform("windows")]
public static class DeviceHelper
{
    private static string GetMacAddress()
    {
        try
        {
            var nic = NetworkInterface.GetAllNetworkInterfaces()
                .FirstOrDefault(n =>
                    n.OperationalStatus == OperationalStatus.Up &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    !n.Description.Contains("virtual", StringComparison.CurrentCultureIgnoreCase) &&
                    !n.Description.Contains("vmware", StringComparison.CurrentCultureIgnoreCase));

            return nic?.GetPhysicalAddress().ToString() ?? "N/A";
        }
        catch
        {
            return "N/A";
        }
    }

    private static string GetCpuId()
    {
        return GetWmiProperty("Win32_Processor", "ProcessorId");
    }

    private static string GetBoardSerial()
    {
        return GetWmiProperty("Win32_BaseBoard", "SerialNumber");
    }

    private static string GetBiosSerial()
    {
        return GetWmiProperty("Win32_BIOS", "SerialNumber");
    }

    private static string GetDiskSerial()
    {
        return GetWmiProperty("Win32_PhysicalMedia", "SerialNumber");
    }

    private static string GetWmiProperty(string wmiClass, string wmiProperty)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher($"SELECT {wmiProperty} FROM {wmiClass}");
            foreach (var obj in searcher.Get())
            {
                return obj[wmiProperty]?.ToString()?.Trim() ?? "N/A";
            }
        }
        catch { }
        return "N/A";
    }

    private static string GetSha256Hash(string input)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    /// <summary>
    /// 获取独特的硬件编码ID
    /// </summary>
    /// <returns></returns>
    public static string GetUniqueHardwareId()
    {
        var cpuId = GetCpuId();
        var boardSerial = GetBoardSerial();
        var biosSerial = GetBiosSerial();
        var diskSerial = GetDiskSerial();
        var macAddress = GetMacAddress();
        var combined = $"{cpuId}-{boardSerial}-{biosSerial}-{diskSerial}-{macAddress}";
        return GetSha256Hash(combined);
    }
}

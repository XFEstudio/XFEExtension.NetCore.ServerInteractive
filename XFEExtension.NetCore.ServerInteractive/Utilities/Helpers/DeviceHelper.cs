using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace XFEExtension.NetCore.ServerInteractive.Utilities.Helpers;

[SupportedOSPlatform("windows")]
public static class DeviceHelper
{
    static string GetMacAddress()
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

    static string GetCpuId()
    {
        return GetWmiProperty("Win32_Processor", "ProcessorId");
    }

    static string GetBoardSerial()
    {
        return GetWmiProperty("Win32_BaseBoard", "SerialNumber");
    }

    static string GetBiosSerial()
    {
        return GetWmiProperty("Win32_BIOS", "SerialNumber");
    }

    static string GetDiskSerial()
    {
        return GetWmiProperty("Win32_PhysicalMedia", "SerialNumber");
    }

    static string GetWmiProperty(string wmiClass, string wmiProperty)
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

    static string GetSha256Hash(string input)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (byte b in hashBytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    public static string GetUniqueHardwareId()
    {
        string cpuId = GetCpuId();
        string boardSerial = GetBoardSerial();
        string biosSerial = GetBiosSerial();
        string diskSerial = GetDiskSerial();
        string macAddress = GetMacAddress();
        string combined = $"{cpuId}-{boardSerial}-{biosSerial}-{diskSerial}-{macAddress}";
        return GetSha256Hash(combined);
    }
}

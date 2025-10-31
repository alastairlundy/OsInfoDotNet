using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Specializations.Configurations;

using OsInfoDotNet.Windows.Abstractions;
using OsInfoDotNet.Windows.Exceptions;
using OsInfoDotNet.Windows.Helpers;

namespace OsInfoDotNet.Windows;

public class WindowsSystemInfoProvider : IWindowsSystemInfoProvider
{
    private readonly IProcessInvoker _processInvoker;
    private readonly DateParser _dateParser;

    public WindowsSystemInfoProvider(IProcessInvoker processInvoker)
    {
        _processInvoker = processInvoker;
        _dateParser = new DateParser();
    }
    
    /// <summary>
    /// Detects the Edition of Windows being run.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="WindowsEditionDetectionException">Throws an exception if operating system detection fails.</exception>
    /// <exception cref="PlatformNotSupportedException">Throws an exception if run on a platform that isn't Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<WindowsEdition> GetWindowsEditionAsync()
    {
        return GetWindowsEdition(await GetWindowsSystemInfoAsync());
    }

    /// <summary>
    /// Detects the Edition of Windows from specified WindowsSystemInformation.
    /// </summary>
    /// <param name="windowsSystemInformation"></param>
    /// <returns></returns>
    /// <exception cref="WindowsEditionDetectionException"></exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when not running on Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public WindowsEdition GetWindowsEdition(WindowsSystemInfo windowsSystemInformation)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        
        string edition = windowsSystemInformation.OsName.ToLower();
            
        if (edition.Contains("home"))
        {
            return WindowsEdition.Home;
        }
        else if (edition.Contains("pro") && edition.Contains("workstation"))
        {
            return WindowsEdition.ProfessionalForWorkstations;
        }
        else if (edition.Contains("pro") && !edition.Contains("education"))
        {
            return WindowsEdition.Professional;
        }
        else if (edition.Contains("pro") && edition.Contains("education"))
        {
            return WindowsEdition.ProfessionalForEducation;
        }
        else if (!edition.Contains("pro") && edition.Contains("education"))
        {
            return WindowsEdition.Education;
        }
        else if (edition.Contains("server"))
        {
            return WindowsEdition.Server;
        }
        else if (edition.Contains("enterprise") && edition.Contains("ltsc") &&
                 !edition.Contains("iot"))
        {
            return WindowsEdition.EnterpriseLTSC;
        }
        else if (edition.Contains("enterprise") && !edition.Contains("ltsc") &&
                 !edition.Contains("iot"))
        {
            return WindowsEdition.EnterpriseSemiAnnualChannel;
        }
        else if (edition.Contains("enterprise") && edition.Contains("ltsc") &&
                 edition.Contains("iot"))
        {
            return WindowsEdition.IoTEnterpriseLTSC;
        }
        else if (edition.Contains("enterprise") && !edition.Contains("ltsc") &&
                 edition.Contains("iot"))
        {
            return WindowsEdition.IoTEnterprise;
        }
        else if (edition.Contains("iot") && edition.Contains("core"))
        {
            return WindowsEdition.IoTCore;
        }
        else if (edition.Contains("team"))
        {
            return WindowsEdition.Team;
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
        {
            if (edition.Contains("se"))
            {
                return WindowsEdition.SE;
            }
        }

        throw new WindowsEditionDetectionException();

    }

    private int GetNetworkCardPositionInWindowsSysInfo(List<NetworkCardModel> networkCards, NetworkCardModel lastNetworkCard)
    {
        for (int position = 0; position < networkCards.Count; position++)
        {
            if (Equals(networkCards[position], lastNetworkCard))
            {
                return position;
            }
        }

        throw new ArgumentException();
    }

    /// <summary>
    /// Detect WindowsSystemInformation
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when not running on Windows.</exception>
#if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#endif
    public async Task<WindowsSystemInfo> GetWindowsSystemInfoAsync()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        
        WindowsSystemInfo windowsSystemInformation = new WindowsSystemInfo();
        HyperVRequirementsModel hyperVRequirements = new HyperVRequirementsModel();
        
        List<string> processors = new List<string>();
        List<NetworkCardModel> networkCards = new List<NetworkCardModel>();
        List<string> ipAddresses = new List<string>();
        
        NetworkCardModel? lastNetworkCard = null;
        
        CmdProcessConfiguration cmdProcessConfiguration = new CmdProcessConfiguration("/c systeminfo",
            false, true, true);
        
        BufferedProcessResult result = await _processInvoker.ExecuteBufferedAsync(cmdProcessConfiguration,
            ProcessExitConfiguration.Default, true, CancellationToken.None);

        string desc = result.StandardOutput;

        string[] array = desc.Split(Environment.NewLine);

#region Manual Detection

bool wasLastLineProcLine = false;
bool wasLastLineNetworkLine = false;

int networkCardNumber = 0;

for (int index = 0; index < array.Length; index++)
{
    string nextLine = "";

    array[index] = array[index].Replace("  ", string.Empty);

    if (index < (array.Length - 1))
    {
        nextLine = array[index + 1].Replace("  ", string.Empty);
    }
    else
    {
        nextLine = array[index].Replace("  ", string.Empty);
    }
    
    if (nextLine.ToLower().Contains("host name:"))
    {
        windowsSystemInformation.HostName =
            nextLine.Replace("Host Name:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("os name:"))
    {
        windowsSystemInformation.OsName = nextLine.Replace("OS Name:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("os version:") && !nextLine.ToLower().Contains("bios"))
    {
        windowsSystemInformation.OsVersion = nextLine.Replace("OS Version:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("os manufacturer:"))
    {
        windowsSystemInformation.OsManufacturer = nextLine.Replace("OS Manufacturer:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("os configuration:"))
    {
        windowsSystemInformation.OsConfiguration = nextLine.Replace("OS Configuration:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("os build type:"))
    {
        windowsSystemInformation.OsBuildType = nextLine.Replace("OS Build Type:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("registered owner:"))
    {
        windowsSystemInformation.RegisteredOwner = nextLine.Replace("Registered Owner:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("registered organization:"))
    {
        windowsSystemInformation.RegisteredOrganization =
            nextLine.Replace("Registered Organization:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("product id:"))
    {
        windowsSystemInformation.ProductId = nextLine.Replace("Product ID:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("original install date:"))
    {
        nextLine = nextLine.Replace("Original Install Date:", string.Empty);

        string[] info = nextLine.Split(',');
        
        windowsSystemInformation.OriginalInstallDate =
            info[0].Contains("/") ? DateTime.Parse(info[0]) : _dateParser.ParseDates(info);
    }
    else if (nextLine.ToLower().Contains("system boot time:"))
    {
        nextLine = nextLine.Replace("System Boot Time:", string.Empty);
        string[] info = nextLine.Split(',');
        
        windowsSystemInformation.SystemBootTime =
            info[0].Contains("/") ? DateTime.Parse(info[0]) : _dateParser.ParseDates(info);
    }
    else if (nextLine.ToLower().Contains("system manufacturer:"))
    {
        windowsSystemInformation.SystemManufacturer = nextLine.Replace("System Manufacturer:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("system model:"))
    {
        windowsSystemInformation.SystemModel = nextLine.Replace("System Model:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("system type:"))
    {
        windowsSystemInformation.SystemType = nextLine.Replace("System Type:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("processor(s):"))
    {
        processors.Add(nextLine.Replace("Processor(s):", string.Empty));

        wasLastLineProcLine = true;
        wasLastLineNetworkLine = false;
    }
    else if (nextLine.ToLower().Contains("bios version:"))
    {
        windowsSystemInformation.BiosVersion = nextLine.Replace("BIOS Version:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("windows directory:"))
    {
        windowsSystemInformation.WindowsDirectory = nextLine.Replace("Windows Directory:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("system directory:"))
    {
        windowsSystemInformation.SystemDirectory = nextLine.Replace("System Directory:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("boot device:"))
    {
        windowsSystemInformation.BootDevice = nextLine.Replace("Boot Device:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("system locale:"))
    {
        windowsSystemInformation.SystemLocale = nextLine.Replace("System Locale:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("input locale:"))
    {
        windowsSystemInformation.InputLocale = nextLine.Replace("Input Locale:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("time zone:"))
    {
        windowsSystemInformation.TimeZone = TimeZoneInfo.Local;
    }
    else if (nextLine.ToLower().Contains("memory:"))
    {
        nextLine = nextLine.Replace(",", string.Empty).Replace(" MB", string.Empty);

        if (nextLine.ToLower().Contains("total physical memory:"))
        {
            nextLine = nextLine.Replace("Total Physical Memory:", string.Empty);
            windowsSystemInformation.TotalPhysicalMemoryMB = int.Parse(nextLine);
        }
        else if (nextLine.ToLower().Contains("available physical memory"))
        {
            nextLine = nextLine.Replace("Available Physical Memory:", string.Empty);
            windowsSystemInformation.AvailablePhysicalMemoryMB = int.Parse(nextLine);
        }
        if (nextLine.ToLower().Contains("virtual memory: max size:"))
        {
            nextLine = nextLine.Replace("Virtual Memory: Max Size:", string.Empty);
            windowsSystemInformation.VirtualMemoryMaxSizeMB = int.Parse(nextLine);
        }
        else if (nextLine.ToLower().Contains("virtual memory: available:"))
        {
            nextLine = nextLine.Replace("Virtual Memory: Available:", string.Empty);
            windowsSystemInformation.VirtualMemoryAvailableSizeMB = int.Parse(nextLine);
        }
        else if (nextLine.ToLower().Contains("virtual memory: in use:"))
        {
            nextLine = nextLine.Replace("Virtual Memory: In Use:", string.Empty);
            windowsSystemInformation.VirtualMemoryInUse = int.Parse(nextLine);
        }
    }
    else if (nextLine.ToLower().Contains("page file location(s):"))
    {
        wasLastLineNetworkLine = false;
        wasLastLineProcLine = false;
        
        List<string> locations =
        [
            nextLine.Replace("Page File Location(s):", string.Empty)
        ];

        int locationNumber = 1;
        
        while (!array[index + 1 + locationNumber].ToLower().Contains("domain"))
        {
            locations.Add(array[index + 1 + locationNumber]);
            locationNumber++;
        }

        windowsSystemInformation.PageFileLocations = locations.ToArray();
    }
    else if (nextLine.ToLower().Contains("domain:"))
    {
        wasLastLineNetworkLine = false;
        wasLastLineProcLine = false;
        
        windowsSystemInformation.Domain = nextLine.Replace("Domain:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("logon server:"))
    {
        wasLastLineNetworkLine = false;
        wasLastLineProcLine = false;
        
        windowsSystemInformation.LogonServer = nextLine.Replace("Logon Server:", string.Empty);
    }
    else if (nextLine.ToLower().Contains("hotfix(s):"))
    {
        wasLastLineNetworkLine = false;
        wasLastLineProcLine = false;
        
        List<string> hotfixes = new List<string>();
        
        int hotfixCount = 0;
        while (array[index + 2 + hotfixCount].Contains("[") && array[index + 2 + hotfixCount].Contains("]:"))
        {
            hotfixes.Add(array[index + 2 + hotfixCount].Replace("  ", string.Empty));
            hotfixCount++;
        }

        windowsSystemInformation.HotfixesInstalled = hotfixes.ToArray();
    }
    else if (nextLine.ToLower().Contains("network card(s):"))
    {
        if (networkCardNumber > 0)
        {
            if (lastNetworkCard != null)  networkCards[networkCardNumber - 1].IpAddresses = ipAddresses.ToArray();
            ipAddresses.Clear();
        }
        
        wasLastLineProcLine = false;
        
        NetworkCardModel networkCard = new NetworkCardModel
        {
            Name = array[index + 2].Replace("  ", string.Empty)
        };

        networkCards.Add(networkCard);
        lastNetworkCard = networkCard; 

        wasLastLineNetworkLine = true;
        networkCardNumber++;
    }
    else if (nextLine.ToLower().Contains("connection name:"))
    {
        wasLastLineProcLine = false;
        
        if (networkCards.Contains(lastNetworkCard))
        {
           int position = GetNetworkCardPositionInWindowsSysInfo(networkCards, lastNetworkCard);
           networkCards[position].ConnectionName = nextLine.Replace("Connection Name:", string.Empty).Replace("  ", string.Empty);
        }
    }
    else if (nextLine.ToLower().Contains("dhcp enabled:"))
    {
        wasLastLineProcLine = false;
        
        int position = GetNetworkCardPositionInWindowsSysInfo(networkCards, lastNetworkCard);
        networkCards[position].DhcpEnabled = array[index + 4].ToLower().Contains("yes");
    }
    else if (nextLine.ToLower().Contains("dhcp server:"))
    {
        int position = GetNetworkCardPositionInWindowsSysInfo(networkCards, lastNetworkCard);
        networkCards[position].DhcpServer = nextLine.Replace("DHCP Server:", string.Empty).Replace("  ", string.Empty);
    }
    else if (nextLine.ToLower().Contains("[") && nextLine.ToLower().Contains("]"))
    {
        string compare = nextLine.Replace("[", string.Empty).Replace("]:", string.Empty);

        int dotCounter = compare.Count(c => c == '.');

        if (dotCounter >= 3 && wasLastLineNetworkLine)
        {
            ipAddresses.Add(nextLine);
        }
        else if (wasLastLineProcLine)
        {
            processors.Add(nextLine);
        }
    }

    else if (nextLine.ToLower().Contains("hyper-v requirements:"))
    {
        hyperVRequirements.VmMonitorModeExtensions = nextLine.Replace("Hyper-V Requirements:", string.Empty)
            .Replace("VM Monitor Mode Extensions: ", string.Empty).Contains("Yes");
    }
    else if (nextLine.ToLower().Contains("virtualization enabled in firmware:"))
    {
        hyperVRequirements.VirtualizationEnabledInFirmware =
            nextLine.Replace("Virtualization Enabled In Firmware:", string.Empty).Contains("Yes");
    }
    else if (nextLine.ToLower().Contains("second level address translation:"))
    {
        hyperVRequirements.SecondLevelAddressTranslation = nextLine
            .Replace("Second Level Address Translation:", string.Empty).Contains("Yes");
    }
    else if (nextLine.ToLower().Contains("data execution prevention available:"))
    {
        wasLastLineNetworkLine = false;
        wasLastLineProcLine = false;
        
        hyperVRequirements.DataExecutionPreventionAvailable = nextLine
            .Replace("Data Execution Prevention Available:", string.Empty).Contains("Yes");
        break;
    }
}
        
        if (networkCardNumber == 1)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (lastNetworkCard != null && ipAddresses != null && networkCards.Count > 0)
                networkCards[networkCardNumber - 1].IpAddresses = ipAddresses.ToArray();
            
            ipAddresses?.Clear();
        }
        #endregion
        
        windowsSystemInformation.NetworkCards = networkCards.ToArray();
        windowsSystemInformation.Processors = processors.ToArray();
        windowsSystemInformation.HyperVRequirements = hyperVRequirements;
        return windowsSystemInformation;
    }
}
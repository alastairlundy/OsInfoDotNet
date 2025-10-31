using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming

namespace OsInfoDotNet.Windows.Abstractions;

public interface IWMISearcher
{
    /// <summary>
    /// Gets information from a WMI class in WMI.
    /// </summary>
    /// <param name="wmiClass"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
    [SupportedOSPlatform("windows")]
    Task<string> GetWMIClass(string wmiClass);

    /// <summary>
    /// Gets a property/value in a WMI Class from WMI.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="wmiClass"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="PlatformNotSupportedException">Thrown if run on an Operating System that isn't Windows.</exception>
    [SupportedOSPlatform("windows")]
    Task<string> GetWMIValue(string property, string wmiClass);
}
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using AlastairLundy.OsInfoDotNet.Windows.Exceptions;

namespace AlastairLundy.OsInfoDotNet.Windows.Abstractions;

public interface IWindowsSystemInfoProvider
{
    /// <summary>
    /// Detects the Edition of Windows being run.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="WindowsEditionDetectionException">Throws an exception if operating system detection fails.</exception>
    /// <exception cref="PlatformNotSupportedException">Throws an exception if run on a platform that isn't Windows.</exception>
    [SupportedOSPlatform("windows")]
    Task<WindowsEdition> GetWindowsEdition();

    /// <summary>
    /// Detects the Edition of Windows from specified WindowsSystemInformation.
    /// </summary>
    /// <param name="windowsSystemInformation"></param>
    /// <returns></returns>
    /// <exception cref="WindowsEditionDetectionException"></exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when not running on Windows.</exception>
    [SupportedOSPlatform("windows")]
    WindowsEdition GetWindowsEdition(WindowsSystemInformationModel windowsSystemInformation);

    /// <summary>
    /// Detect WindowsSystemInformation
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Thrown when not running on Windows.</exception>
    [SupportedOSPlatform("windows")]
    Task<WindowsSystemInformationModel> GetWindowsSystemInformation();
}
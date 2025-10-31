using System;

using OsInfoDotNet.Abstractions;

namespace AlastairLundy.OsInfoDotNet.Windows.Extensions;

public static class WindowsPlatformIsVersionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    public static bool IsWindows10(this OperatingSystemInfo platform)
    {
        return platform.OSVersion >= new Version(10, 0, 10240)
               && platform.OSVersion < new Version(10, 0, 22000);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="platform"></param>
    /// <returns></returns>
    public static bool IsWindows11(this OperatingSystemInfo platform)
    {
        return platform.OSVersion >= new Version(10, 0, 22000)
               && platform.OSVersion < new Version(10, 0, 29000);
    }
}
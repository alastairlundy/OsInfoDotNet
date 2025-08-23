using System;
using System.Threading.Tasks;

namespace AlastairLundy.OsInfoDotNet.Mac;

/// <summary>
/// 
/// </summary>
public interface IMacOsInfoProvider
{
    /// <summary>
    /// Returns whether a Mac is Apple Silicon based.
    /// </summary>
    /// <returns>true if the currently running Mac uses Apple Silicon; false if running on an Intel Mac.</returns>
#if NET5_0_OR_GREATER
#endif
    bool IsAppleSiliconMac();
    
    /// <summary>
    /// Detects macOS System Information.
    /// </summary>
    /// <returns></returns>
#if NET5_0_OR_GREATER
#endif
    Task<MacOsSystemInfo> GetMacSystemInfoAsync();

    /// <summary>
    /// Detects the Darwin Version on macOS
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
#if NET5_0_OR_GREATER
#endif
    Version GetDarwinVersion();

    /// <summary>
    /// Detects macOS's XNU Version.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
#if NET5_0_OR_GREATER
#endif
    Version GetXnuVersion();

    /// <summary>
    /// Detects the macOS version and returns it as a System.Version object.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
#if NET5_0_OR_GREATER
#endif
    Task<Version> GetMacOsVersionAsync();

    /// <summary>
    /// Detects the Build Number of the installed version of macOS.
    /// </summary>
    /// <returns>the build number of the installed version of macOS.</returns>
    /// <exception cref="PlatformNotSupportedException">Throw if run on an Operating System that isn't macOS.</exception>
#if NET5_0_OR_GREATER
#endif
    Task<string> GetMacOsBuildNumberAsync();
}
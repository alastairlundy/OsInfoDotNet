using System;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace OsInfoDotnet.Android.Abstractions;

public interface IAndroidOperatingSystemInfoProvider
{
    Task<string> GetDeviceModelAsync(CancellationToken cancellationToken);
    
    Task<string> GetOSVersionCodenameAsync(CancellationToken cancellationToken);
    
    Task<Version> GetSdkApiLevelAsync(CancellationToken cancellationToken);
}
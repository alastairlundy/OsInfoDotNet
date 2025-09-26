using System;
using OsInfoDotNet.Core.Enums;
// ReSharper disable InconsistentNaming

namespace OsInfoDotNet.Core;

public class OperatingSystemInfo : IEquatable<OperatingSystemInfo>
{
    public string Name { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public Version OSVersion { get; }
        
    /// <summary>
    /// 
    /// </summary>
    public Version KernelVersion { get; }
    
    public OperatingSystemFamily OSFamily { get; }
    
    public string OSBuildNumber { get; }

    public OperatingSystemInfo(string name, Version osVersion, Version kernelVersion, OperatingSystemFamily osFamily, string osBuildNumber)
    {
        Name = name;
        OSVersion = osVersion;
        KernelVersion = kernelVersion;
        OSFamily = osFamily;
        OSBuildNumber = osBuildNumber;
    }

    public bool Equals(OperatingSystemInfo? other)
    {
        if (other is null) return false;
        
        return Name == other.Name
               && OSVersion.Equals(other.OSVersion)
               && KernelVersion.Equals(other.KernelVersion)
               && OSFamily == other.OSFamily
               && OSBuildNumber == other.OSBuildNumber;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        
        if(obj is OperatingSystemInfo other)
            return Equals(other);
        
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, OSVersion, KernelVersion, (int)OSFamily);
    }

    public static bool operator ==(OperatingSystemInfo? left, OperatingSystemInfo? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OperatingSystemInfo? left, OperatingSystemInfo? right)
    {
        return !Equals(left, right);
    }
}
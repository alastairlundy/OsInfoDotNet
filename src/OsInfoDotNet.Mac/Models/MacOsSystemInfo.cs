
using System;
using System.Reflection;

namespace AlastairLundy.OsInfoDotNet.Mac;

/// <summary>
/// A class to represent basic macOS System Information.
/// </summary>
public class MacOsSystemInfo
{
    public Version MacOsVersion { get; set; }
    
    public Version DarwinVersion { get; set; }
    
    public Version XnuVersion { get; set; } 
    
    public string MacOsBuildNumber { get; set; }
    
    public ProcessorArchitecture CpuArchitecture { get; set; }
}
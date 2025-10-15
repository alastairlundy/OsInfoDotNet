
// ReSharper disable InconsistentNaming
namespace OsInfoDotNet.Abstractions;

public enum OperatingSystemFamily
{
    WindowsNT,
    /// <summary>
    /// Darwin derived operating systems such as macOS, IOS, iPadOS, tvOS, watchOS, and visionOS are included in this.
    /// </summary>
    Darwin,
    Linux,
    BSD,
    Unix,
    /// <summary>
    /// Android based Operating Systems such as Android TV, wearOS, and FireOS are included in this.
    /// </summary>
    Android,
    Other
}
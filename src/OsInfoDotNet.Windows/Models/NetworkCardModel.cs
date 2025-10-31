
namespace OsInfoDotNet.Windows;

/// <summary>
/// 
/// </summary>
public class NetworkCardModel
{
    public string Name { get; set; }
    
    public string ConnectionName { get; set; }
    
    public bool DhcpEnabled { get; set; }
    
    public string DhcpServer { get; set; }

    public string[] IpAddresses { get; set; }
}
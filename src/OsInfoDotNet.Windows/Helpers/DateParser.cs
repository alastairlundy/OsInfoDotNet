using System;

namespace OsInfoDotNet.Windows.Helpers;

internal class DateParser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    internal DateTime ParseDates(string[] info)
    {
        DateTime dt = new DateTime();
        
        info[1] = info[1].Replace(" ", string.Empty).Replace(":", string.Empty);

        string hours = info[1].Substring(0, 2);
        string minutes = info[1].Substring(2, 2);
        string seconds = info[1].Substring(4, 2);

        dt = dt.AddHours(double.Parse(hours));
        dt = dt.AddMinutes(double.Parse(minutes));
        dt = dt.AddSeconds(double.Parse(seconds));
            
        return dt;
    }
}
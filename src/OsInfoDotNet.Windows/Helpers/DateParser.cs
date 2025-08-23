using System;

namespace AlastairLundy.OsInfoDotNet.Windows.Helpers;

public class DateParser
{
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
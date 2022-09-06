using System;

namespace MiigaikVkBot.Utils;

public class DateTimeProvider
{
    /// <summary>
    /// Provide datetime for Moscow
    /// </summary>
    public static DateTime Now => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Russian Standard Time");
}
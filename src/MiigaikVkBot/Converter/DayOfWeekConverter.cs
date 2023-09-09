using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MiigaikVkBot.Converters
{
    public static class DayOfWeekConverter
    {
        public static DayOfWeek FromStrToDOW(string inp)
        {
            switch (inp.ToLower())
            {
                case "понедельник":
                    return DayOfWeek.Monday;
                case "вторник":
                    return DayOfWeek.Tuesday;
                case "среда":
                    return DayOfWeek.Wednesday;
                case "четверг":
                    return DayOfWeek.Thursday;
                case "пятница":
                    return DayOfWeek.Friday;
                case "суббота":
                    return DayOfWeek.Saturday;
                case "воскресенье":
                    return DayOfWeek.Sunday;
                default:
                    throw new ArgumentException("Unrecognized day of week");
            }
        }

        public static string FromDOWToStr(DayOfWeek inp)
        {
            switch (inp)
            {
                case DayOfWeek.Monday:
                    return "Понедельник";
                case DayOfWeek.Tuesday:
                    return "Вторник";
                case DayOfWeek.Wednesday:
                    return "Среда";
                case DayOfWeek.Thursday:
                    return "Четверг";
                case DayOfWeek.Friday:
                    return "Пятница";
                case DayOfWeek.Saturday:
                    return "Суббота";
                case DayOfWeek.Sunday:
                    return "Воскресенье";
                default:
                    return "";
            }
        }
    }
}

using MiigaikVkBot.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TimetableGetter;

namespace MiigaikVkBot.Utils
{
    /// <summary>
    /// Получает предмет, который будет или уже должен идти сейчас
    /// </summary>
    public static class CurrentSubjectGetter
    {
        private static Tuple<int, string> GetCurrentSubjectNum(TimeSpan currTime)
        {
            int subjNum = 1;
            string status = "Иди и спи. Ещё слишком рано.";

            TimeSpan[] timeSpans = new TimeSpan[]
            {
                new TimeSpan(0, 0, 0),
                new TimeSpan(9, 0, 0),   //true  0
                new TimeSpan(10, 30, 0), //false 1
                new TimeSpan(10, 40, 0), //true  2
                new TimeSpan(12, 10, 0), //false 3
                new TimeSpan(12, 50, 0), //true  4
                new TimeSpan(14, 20, 0), //false 5 
                new TimeSpan(14, 30, 0), //true  6
                new TimeSpan(16, 00, 0), //false 7
                new TimeSpan(16, 10, 0), //true  8
                new TimeSpan(17, 40, 0), //false 9 
                new TimeSpan(17, 50, 0), //true 10 
                new TimeSpan(19, 20, 0), //false 11
                new TimeSpan(19, 30, 0), //true  12
                new TimeSpan(21, 00, 0), //false 13
                new TimeSpan(21, 10, 0), //true  14
                new TimeSpan(22, 40, 0), //false 15
                new TimeSpan(23, 59, 59) //true  16
            };

            for (int i = timeSpans.Length - 1; i > 0; i--)
                if (currTime > timeSpans[i])
                {
                    subjNum = (i / 2) + 1;
                    if (i % 2 == 0)
                        status = $"{Emoji.LetterI()} ПЕРЕРЫВ {Emoji.LetterI()}\n";
                    else
                        status = $"{Emoji.RedCircle()} ИДЁТ ПАРА {Emoji.RedCircle()}\n";

                    return Tuple.Create(subjNum, status);
                }

            return Tuple.Create(subjNum, status);
        }

        /// <summary>
        /// Получить предмет, который будет или уже должен идти сейчас
        /// </summary>
        /// <param name="shedule">Расписание</param>
        /// <returns></returns>
        public static string GetSubject(Shedule shedule, ConcurrentDictionary<string, string> urls = null)
        {
            //Tuple<int, string> currSubj = GetCurrentSubjectNum(DateTime.Now.TimeOfDay);
            Tuple<int, string> currSubj = GetCurrentSubjectNum(DateTime.Now.TimeOfDay);
            Day loadingDay = null;

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    loadingDay = shedule.Monday;
                    break;

                case DayOfWeek.Tuesday:
                    loadingDay = shedule.Tuesday;
                    break;

                case DayOfWeek.Wednesday:
                    loadingDay = shedule.Wednesday;
                    break;

                case DayOfWeek.Thursday:
                    loadingDay = shedule.Thursday;
                    break;

                case DayOfWeek.Friday:
                    loadingDay = shedule.Friday;
                    break;

                case DayOfWeek.Saturday:
                    loadingDay = shedule.Saturday;
                    break;
            }

            if (loadingDay.Timetable.Count == 0)
                return "Пар сегодня нет. Совсем нет.";

            WeekType weekType = shedule.IsLower(DateTime.Now) ? WeekType.Lower : WeekType.Upper;
            Subject subject = loadingDay.Timetable.SingleOrDefault(x => x.SubjectNumber == currSubj.Item1 && x.WeekType == weekType);

            if (subject == null)
                currSubj = Tuple.Create(currSubj.Item1, $"{Emoji.LetterI()} ПЕРЕРЫВ {Emoji.LetterI()}\nСледующая ближайшая пара:\n");

            for (int i = 0; i < 7 && i < loadingDay.Timetable.Count; i++)
                if (loadingDay.Timetable[i].SubjectNumber >= currSubj.Item1 && loadingDay.Timetable[i].WeekType == weekType)
                {
                    subject = loadingDay.Timetable[i];
                    break;
                }

            if (subject == null)
                return "Пар сегодня больше нет. Совсем нет.";

            string url = string.Empty;
            urls.TryGetValue(subject.SubjectName.ToLower(), out url);

            return $"{currSubj.Item2}\n{SheduleFormat.PrintSubject(subject, url)}";
        }
    }
}

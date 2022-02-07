using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TimetableGetter;

using StudBot.Converters;
using System.Collections.Concurrent;

namespace StudBot.Converters
{
    public static class SheduleFormat
    {
        /// <summary>
        /// Словарь, в котором содержится время начала пары
        /// </summary>
        private static Dictionary<int, string> LessonsStartTime = new Dictionary<int, string>()
        {
            { 1, "9:00" },
            { 2, "10:40" },
            { 3, "12:50" },
            { 4, "14:30" },
            { 5, "16:10" },
            { 6, "17:50" },
            { 7, "19:30" },
            { 8, "21:10" }
        };

        /// <summary>
        /// Возвращает расписание на определённый день недели
        /// </summary>
        /// <param name="day">День, на который нужно получить расписание</param>
        /// <param name="shedule">Само расписание</param>
        /// <param name="bothWeeeks">Расписание на обе недели (верхней и нижней) или только на текущей</param>
        /// <returns></returns>
        public static string GetSheduleOn(DateTime day, Shedule shedule, bool bothWeeks, ConcurrentDictionary<string, string> urls = null)
        {
            StringBuilder lower = new StringBuilder();
            StringBuilder upper = new StringBuilder();
            StringBuilder final = new StringBuilder();

            final.AppendLine($"{Converters.DayOfWeekConverter.FromDOWToStr(day.DayOfWeek).ToUpper()}:\n");
            if (shedule.IsLower(day))
            {
                lower.AppendLine("НИЖНЯЯ НЕДЕЛЯ:");
                upper.AppendLine("Верхняя неделя:");
            }
            else
            {
                lower.AppendLine("Нижняя неделя:");
                upper.AppendLine("ВЕРХНЯЯ НЕДЕЛЯ:");
            }

            Day loadingDay = null;

            switch (day.DayOfWeek)
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

            if (urls is null)
            { 
                foreach (Subject subj in loadingDay.Timetable)
                    if (subj.WeekType == WeekType.Lower)
                        lower.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: [{LessonsStartTime[subj.SubjectNumber]}] {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} {subj.SubjectType}\n{subj.EducatorName} ({subj.Auditory}){(subj.Comment == string.Empty ? "" : $"\n{Emoji.RedCircle()}{subj.Comment}{Emoji.RedCircle()}")}");
                        //lower.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} ({subj.Auditory}) {subj.SubjectType}\n{subj.EducatorName}{(subj.Comment == string.Empty ? "" : $"\n{Emoji.RedCircle()}{subj.Comment}{Emoji.RedCircle()}")}");
                    else
                        upper.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: [{LessonsStartTime[subj.SubjectNumber]}] {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} {subj.SubjectType}\n{subj.EducatorName} ({subj.Auditory}){(subj.Comment == string.Empty ? "" : $"\n{Emoji.RedCircle()}{subj.Comment}{Emoji.RedCircle()}")}");
                        //upper.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} ({subj.Auditory}) {subj.SubjectType}\n{subj.EducatorName}{(subj.Comment == string.Empty ? "" : $"\n{Emoji.RedCircle()}{subj.Comment}{Emoji.RedCircle()}")}");
            }
            else
            {
                foreach (Subject subj in loadingDay.Timetable)
                {
                    string url = string.Empty;
                    
                    string subjName = subj.SubjectName.ToLower();
                    // Remove multiple spaces
                    subjName = string.Join( " ", subjName.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ));
                    
                    urls.TryGetValue(subjName, out url);

                    if (subj.WeekType == WeekType.Lower)
                        lower.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: [{LessonsStartTime[subj.SubjectNumber]}] {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} {subj.SubjectType}\n{subj.EducatorName} ({subj.Auditory}){(string.IsNullOrEmpty(url) ? "" : $"\n{Emoji.RedCircle()}{url}{Emoji.RedCircle()}")}");
                    else
                        upper.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: [{LessonsStartTime[subj.SubjectNumber]}] {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} {subj.SubjectType}\n{subj.EducatorName} ({subj.Auditory}){(string.IsNullOrEmpty(url) ? "" : $"\n{Emoji.RedCircle()}{url}{Emoji.RedCircle()}")}");
                }
            }


            if (!bothWeeks)
            {
                if (shedule.IsLower(day))
                    final.Append(lower);
                else
                    final.Append(upper);
            }
            else
            {
                final.Append(lower);
                final.Append("\n\n");
                final.Append(upper);
            }

            if (loadingDay.Comments.Count != 0)
            {
                foreach (string str in loadingDay.Comments)
                    final.Append($"{str}\n");
            }

            return final.ToString();
        }

        /// <summary>
        /// Выводит подробную информацию о предмете
        /// </summary>
        /// <param name="subj">Рассматриваемая пара</param>
        /// <returns></returns>
        public static string PrintSubject(Subject subj, string url)
        {
            return $"{Emoji.Number(subj.SubjectNumber)}: [{LessonsStartTime[subj.SubjectNumber]}] {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} {subj.SubjectType}\n{subj.EducatorName} ({subj.Auditory}){(string.IsNullOrEmpty(url) ? "" : $"\n{Emoji.RedCircle()}{url}{Emoji.RedCircle()}")}";
        }

        /// <summary>
        /// Выдает расписание вебинаров. Наличие вебинара определяет по наличию ссылки в примечаниях
        /// </summary>
        /// <param name="day">День, на который надо получить расписание вебинаров</param>
        /// <param name="shedule">Ссылка на объектс загруженным расписанием</param>
        /// <param name="bothWeeks">Вернуть расписание на обе недели</param>
        /// <returns></returns>
        public static string GetWebinarsOn(DateTime day, Shedule shedule, bool bothWeeks)
        {
            StringBuilder lower = new StringBuilder();
            StringBuilder upper = new StringBuilder();
            StringBuilder final = new StringBuilder();

            final.AppendLine($"{Converters.DayOfWeekConverter.FromDOWToStr(day.DayOfWeek).ToUpper()}:\n");
            if (shedule.IsLower(day))
            {
                lower.AppendLine("НИЖНЯЯ НЕДЕЛЯ:");
                upper.AppendLine("Верхняя неделя:");
            }
            else
            {
                lower.AppendLine("Нижняя неделя:");
                upper.AppendLine("ВЕРХНЯЯ НЕДЕЛЯ:");
            }

            Day loadingDay = null;

            switch (day.DayOfWeek)
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

            StringBuilder lowerSubject = new StringBuilder();
            StringBuilder upperSubject = new StringBuilder();

            foreach (Subject subj in loadingDay.Timetable)
                if (subj.WeekType == WeekType.Lower)
                {
                    if (subj.Comment != string.Empty)
                        lowerSubject.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} ({subj.Auditory}) {subj.SubjectType}\n{subj.EducatorName}{(subj.Comment == string.Empty ? "" : $"\n{subj.Comment}")}");
                }
                else
                {
                    if (subj.Comment != string.Empty)
                        upperSubject.AppendLine($"{Emoji.Number(subj.SubjectNumber)}: {(subj.GroupNumber != 0 ? $"(группа {subj.GroupNumber})" : "")} {subj.SubjectName.ToUpper()} ({subj.Auditory}) {subj.SubjectType}\n{subj.EducatorName}{(subj.Comment == string.Empty ? "" : $"\n{subj.Comment}")}");
                }

            if (lowerSubject.ToString() == string.Empty)
                lowerSubject.AppendLine("Вебинары отсутствуют");

            if (upperSubject.ToString() == string.Empty)
                upperSubject.AppendLine("Вебинары отсутствуют");

            lower.Append(lowerSubject);
            upper.Append(upperSubject);

            if (!bothWeeks)
            {
                if (shedule.IsLower(day))
                    final.Append(lower);
                else
                    final.Append(upper);
            }
            else
            {
                final.Append(lower);
                final.Append("\n\n");
                final.Append(upper);
            }

            if (loadingDay.Comments.Count != 0)
            {
                foreach (string str in loadingDay.Comments)
                    final.Append($"{str}\n");
            }

            return final.ToString();
        }
    }
}


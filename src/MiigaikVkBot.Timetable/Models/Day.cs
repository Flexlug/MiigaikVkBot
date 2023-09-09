using MiigaikVkBot.Timetable.Utils;
using MiigaikVkBot.Timetable.Web.Models;

namespace MiigaikVkBot.Timetable.Models
{
    public class Day
    {
        /// <summary>
        /// Расписание пар
        /// </summary>
        public List<Subject> Timetable = new List<Subject>();

        /// <summary>
        /// Прочие комментарии
        /// </summary>
        public List<string> Comments = new List<string>();

        /// <summary>
        /// Очищает все списки с парами и комментариями
        /// </summary>
        public void Clear()
        {
            Timetable.Clear();
            Comments.Clear();
        }

        public static Day FromApi(List<SubjectResponse> apiDay)
        {
            var day = new Day();
            day.Timetable = apiDay
                .Select(x => new Subject()
                {
                    SubjectName = x.Subject,
                    SubjectNumber = (int)char.GetNumericValue(x.Lesson[0]),
                    WeekType = x.WeekType switch
                    {
                        "верхняя" => WeekType.Upper,
                        "нижняя" => WeekType.Lower
                    },
                    GroupNumber = string.IsNullOrEmpty(x.SubGroup) ? 0 : int.Parse(x.SubGroup),
                    EducatorName = x.Teacher,
                    Auditory = x.Classroom,
                    SubjectType = x.LessonType.Capitalize(),
                    Comment = x.AdditionalInfo

                }).ToList();

            day.Comments = apiDay.Select(x => x.AdditionalInfo).ToList();
            return day;
        }

        public static Day operator +(Day a, Day b)
        {
            var day = new Day();

            if (a is null)
                return b;

            if (b is null)
                return a;
            
            day.Timetable.AddRange(a.Timetable);
            day.Timetable.AddRange(b.Timetable);
            
            day.Comments.AddRange(a.Comments);
            day.Comments.AddRange(b.Comments);

            return day;
        }
    }
}
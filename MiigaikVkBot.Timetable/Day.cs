using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableGetter
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
    }
}

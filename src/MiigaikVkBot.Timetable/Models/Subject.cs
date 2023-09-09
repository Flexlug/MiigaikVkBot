namespace MiigaikVkBot.Timetable.Models
{
    /// <summary>
    /// Тип недели
    /// </summary>
    public enum WeekType
    {
        /// <summary>
        /// Верхняя неделя
        /// </summary>
        Upper,

        /// <summary>
        /// Нижняя неделя
        /// </summary>
        Lower
    }

    /// <summary>
    /// Представление одного предмета в расписании
    /// </summary>
    public class Subject
    {
        /// <summary>
        /// Название предмета
        /// </summary>
        public string SubjectName;

        /// <summary>
        /// Порядковый номер пары
        /// </summary>
        public int SubjectNumber;

        /// <summary>
        /// Верхняя или нижняя неделя
        /// </summary>
        public WeekType WeekType;

        /// <summary>
        /// Номер подгруппы
        /// </summary>
        public int GroupNumber;

        /// <summary>
        /// Имя преподавателя
        /// </summary>
        public string EducatorName;

        /// <summary>
        /// Название аудитории
        /// </summary>
        public string Auditory;

        /// <summary>
        /// Тип предмета (лекция или практика)
        /// </summary>
        public string SubjectType;

        /// <summary>
        /// Прочие комментарии
        /// </summary>
        public string Comment;
    }
}

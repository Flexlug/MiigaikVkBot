using MiigaikVkBot.Timetable.Models;

namespace MiigaikVkBot.Timetable;

public abstract class BaseShedule
{
    /// <summary>
    /// Расписание на понедельник
    /// </summary>
    public Day Monday;

    /// <summary>
    /// Расписание на вторник
    /// </summary>
    public Day Tuesday;

    /// <summary>
    /// Расписание на среду
    /// </summary>
    public Day Wednesday;

    /// <summary>
    /// Расписание на четверг
    /// </summary>
    public Day Thursday;

    /// <summary>
    /// Расписание на пятницу
    /// </summary>
    public Day Friday;

    /// <summary>
    /// Расписание на субботу
    /// </summary>
    public Day Saturday;

    /// <summary>
    /// Расписание на воскресенье
    /// </summary>
    public Day Sunday;

    /// <summary>
    /// Вычисляет тип недели - верхняя или нижняя
    /// </summary>
    public abstract bool IsLower(DateTime inp);

    /// <summary>
    /// Очистить расписание
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Запускает процесс обновления расписания
    /// </summary>
    public abstract void UpdateTimetable();
}
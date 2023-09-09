using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MiigaikVkBot.Timetable.Models;
using MiigaikVkBot.Timetable.Web;
using MiigaikVkBot.Timetable.Web.Models;
using Refit;
using RestSharp;

namespace MiigaikVkBot.Timetable.Implementation;

public class Shedule : BaseShedule
{
    private bool _reverseWeek = false;
    private const string BaseApiUrl = "https://study.miigaik.ru/api/v1";

    private Regex _groupUrlRegex =
        new("faculty=(?<faculty>..+)&course=(?<course>\\d)&group-name=(?<groupname>..+)", RegexOptions.Compiled);
    private ISheduleAPI _sheduleApi { get; init; }
    private string _faculty { get; init; }
    private int _cource { get; init; }
    private string _groupname { get; init; }
    
    public Shedule(string groupUrl)
    {        
        _sheduleApi = RestService.For<ISheduleAPI>(BaseApiUrl);

        var matches = _groupUrlRegex.Matches(groupUrl).First();
        _faculty = matches.Groups["faculty"].Value;
        _cource = int.Parse(matches.Groups["course"].Value);
        _groupname = matches.Groups["groupname"].Value;
    }
    
    public override bool IsLower(DateTime inp)
    {
        bool res = ((int)(inp - new DateTime(2019, 9, 1)).TotalDays + 13) / 7 % 2 == 0;
        return !_reverseWeek ? res : !res;
    }

    public override void Clear()
    {
        Monday = new Day();
        Monday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });

        Tuesday = new Day();
        Tuesday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });

        Wednesday = new Day();
        Wednesday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });

        Thursday = new Day();
        Thursday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });

        Friday = new Day();
        Friday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });

        Saturday = new Day();
        Saturday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });

        Sunday = new Day();
        Sunday.Timetable.Add(new Subject()
        {
            SubjectName = "Расписание отсутствует"
        });
    }

    public override void UpdateTimetable()
    {   
        Clear();
        
        var newSheduleTask = _sheduleApi.SheduleFor(_faculty, _cource, _groupname);
        newSheduleTask.ConfigureAwait(false);

        var newShedultResponse = newSheduleTask.Result;
        var newShedule = newShedultResponse.Content;

        // Совсем нет расписания
        if (newShedule?.LowerWeek is null && newShedule?.UpperWeek is null)
        {
            return;
        }
        
        var daysDict = new Dictionary<string, Day>()
        {
            { "Monday", Monday },
            { "Tuesday", Tuesday },
            { "Wednesday", Wednesday },
            { "Thursday", Thursday },
            { "Friday", Friday },
            { "Saturday", Saturday },
            { "Sunday", Sunday }
        };

        // Есть расписание только на одну неделю
        if (newShedule.LowerWeek is null || newShedule.UpperWeek is null)
        {
            var iter = (newShedule.LowerWeek ?? newShedule.UpperWeek).GetEnumerator();
            
            foreach (var day in daysDict)
            {
                var currentDay = iter.Current ?? new List<SubjectResponse>();
                var newDay = Day.FromApi(currentDay);
            
                day.Value.Clear();
                day.Value.Timetable.AddRange(newDay.Timetable);
                day.Value.Comments.AddRange(newDay.Comments);

                iter.MoveNext();
            }

            return;
        }
        
        // Есть расписание на обе недели
        var lowerIter = newShedule.LowerWeek.GetEnumerator();
        var upperIter = newShedule.UpperWeek.GetEnumerator();
        
        foreach (var day in daysDict)
        {
            var lowerDay = lowerIter.Current ?? new List<SubjectResponse>();
            var upperDay = upperIter.Current ?? new List<SubjectResponse>();

            Day newDay = null;

            if (lowerDay is not null && upperDay is not null)
            {
                newDay = Day.FromApi(lowerDay) + Day.FromApi(upperDay);
            }
            else
            {
                newDay = Day.FromApi(lowerDay ?? upperDay);
            }
            
            day.Value.Clear();
            day.Value.Timetable.AddRange(newDay.Timetable);
            day.Value.Comments.AddRange(newDay.Comments);

            lowerIter.MoveNext();
            upperIter.MoveNext();
        } 
    }
}
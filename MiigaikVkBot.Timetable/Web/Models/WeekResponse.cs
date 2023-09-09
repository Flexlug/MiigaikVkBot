using System.Collections;
using System.Text.Json.Serialization;

namespace MiigaikVkBot.Timetable.Web.Models;


//public class WeekResponse : IEnumerable<List<SubjectResponse>>
public class WeekResponse
{
    [JsonPropertyName("Понедельник")]
    public List<SubjectResponse> Monday { get; set; }
    
    [JsonPropertyName("Вторник")]
    public List<SubjectResponse> Tuesday { get; set; }
    
    [JsonPropertyName("Среда")]
    public List<SubjectResponse> Wednesday { get; set; }
    
    [JsonPropertyName("Четверг")]
    public List<SubjectResponse> Thursday { get; set; }
    
    [JsonPropertyName("Пятница")]
    public List<SubjectResponse> Friday { get; set; }
    
    [JsonPropertyName("Суббота")]
    public List<SubjectResponse> Saturday { get; set; }

    public IEnumerator<List<SubjectResponse>> GetEnumerator() => new WeekResponseEnumerator(this);

    private class WeekResponseEnumerator : IEnumerator<List<SubjectResponse>>
    {
        private int _position = 1;
        private WeekResponse _week;
        private List<SubjectResponse> _current => _getDayByIndex(_position);

        public WeekResponseEnumerator(WeekResponse week) => _week = week;
        
        public bool MoveNext()
        {
            if (_position < 6)
            {
                _position++;
                return true;
            }

            return false;
        }

        private List<SubjectResponse> _getDayByIndex(int i) => i switch
        {
            1 => _week.Monday,
            2 => _week.Tuesday,
            3 => _week.Wednesday,
            4 => _week.Thursday,
            5 => _week.Friday,
            6 => _week.Saturday
        };

        public void Reset() => _position = 1;
        
        public object Current { get => _current; }

        List<SubjectResponse> IEnumerator<List<SubjectResponse>>.Current => _getDayByIndex(_position);

        public void Dispose() {  }
    }
}
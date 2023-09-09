using System.Text.Json.Serialization;

namespace MiigaikVkBot.Timetable.Web.Models;

public class SheduleResponse
{
    [JsonPropertyName("верхняя")]
    public WeekResponse UpperWeek { get; set; }
    
    [JsonPropertyName("нижняя")]
    public WeekResponse LowerWeek { get; set; }
}
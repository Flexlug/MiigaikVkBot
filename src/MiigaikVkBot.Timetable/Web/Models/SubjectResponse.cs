using System.Text.Json.Serialization;

namespace MiigaikVkBot.Timetable.Web.Models;

public class SubjectResponse
{
    [JsonPropertyName("day")]
    public string Day { get; set; }
    
    [JsonPropertyName("lesson")]
    public string Lesson { get; set; }
    
    [JsonPropertyName("week-type")]
    public string WeekType { get; set; }
    
    [JsonPropertyName("subgroup")]
    public string SubGroup { get; set; }
    
    [JsonPropertyName("subject")]
    public string Subject { get; set; }
    
    [JsonPropertyName("teacher")]
    public string Teacher { get; set; }
    
    [JsonPropertyName("classroom")]
    public string Classroom { get; set; }
    
    [JsonPropertyName("lesson-type")]
    public string LessonType { get; set; }
    
    [JsonPropertyName("additional-info")]
    public string AdditionalInfo { get; set; }
}
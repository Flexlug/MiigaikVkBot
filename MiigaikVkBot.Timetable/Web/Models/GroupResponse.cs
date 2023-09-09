using System.Text.Json.Serialization;

namespace MiigaikVkBot.Timetable.Web.Models;

public class GroupResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("schedule-link")]
    public string SheduleLink { get; set; }
}
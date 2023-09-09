using MiigaikVkBot.Timetable.Web.Models;
using Refit;
using RestSharp;

namespace MiigaikVkBot.Timetable.Web;

public interface ISheduleAPI
{
    [Get("/groups")]
    public Task<ApiResponse<List<GroupResponse>>> Groups();

    [Get("/groups")]
    public Task<ApiResponse<SheduleResponse>> SheduleFor(
        [AliasAs("faculty")] string Faculty,
        [AliasAs("course")] int Course,
        [AliasAs("group-name")] string GroupName);
}
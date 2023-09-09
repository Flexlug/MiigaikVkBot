
using System;
using MiigaikVkBot.Responser;
using MiigaikVkBot.Utils;

using VkNet;
using VkNet.Model;
using VkNet.Enums.SafetyEnums;
using VkNet.Enums.StringEnums;

namespace MiigaikVkBot
{
    class Program
    {
        static VkApi api = new VkApi();
        static Reponser _baseResponser;

        static void Main(string[] args)
        {
            Console.WriteLine($"StudBot v.{VersionInfo.Ver}");

            string token = Environment.GetEnvironmentVariable("VK_TOKEN");
            string group_url = Environment.GetEnvironmentVariable("GROUP_URL");

            _baseResponser = new Reponser(group_url, 215791351, api, true);

            ApiAuthParams authParams = new ApiAuthParams()
            {
                AccessToken = token
            };
            api.Authorize(authParams);


            if (api.IsAuthorized)
            {
                Console.WriteLine("Authorized");
            }
            else
            {
                Console.WriteLine("Error");
            }

            while (true)
            {
                var s = api.Groups.GetLongPollServer(215791351);

                var poll = api.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams()
                                                             { 
                                                                Server = s.Server, 
                                                                Ts = s.Ts, 
                                                                Key = s.Key, 
                                                                Wait = 25 
                                                             });
                if (poll?.Updates == null) 
                    continue;

                foreach (var update in poll.Updates)
                {
                    if (update.Type.Value == GroupUpdateType.MessageNew)
                    {
                        var answer = _baseResponser.ConstructResponse(update.Instance as Message);
                        if (answer is not null)
                            api.Messages.Send(_baseResponser.ConstructResponse(update.Instance as Message));
                    }
                }
            }
        }
    }
}

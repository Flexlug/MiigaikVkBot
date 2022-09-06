using System;

using MiigaikVkBot.Responsers;
using MiigaikVkBot.Utils;

using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums.SafetyEnums;
using VkNet.Enums.Filters;
using System.IO;

namespace MiigaikVkBot
{
    class Program
    {
        static VkApi api = new VkApi();
        static ResponserIPD responser;

        static void Main(string[] args)
        {
            Console.WriteLine($"StudBot v.{VersionInfo.Ver}");

            responser = new ResponserIPD(215791351, api, true);

            string token = File.ReadAllText("data/token.txt");

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
                    if (update.Type == GroupUpdateType.MessageNew)
                    {
                        var answer = responser.ConstructResponse(update.Message);
                        if (answer is not null)
                            api.Messages.Send(responser.ConstructResponse(update.Message));
                    }
                }
            }
        }
    }
}

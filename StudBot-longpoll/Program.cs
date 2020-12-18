using System;

using StudBot.Responsers;
using StudBot.Utils;

using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums.SafetyEnums;
using VkNet.Enums.Filters;

namespace StudBot
{
    class Program
    {
        static VkApi api = new VkApi();
        static ResponserIPD responser;

        static void Main(string[] args)
        {
            Console.WriteLine($"StudBot v.{VersionInfo.Ver}");

            responser = new ResponserIPD(185277791, api, true);
            
            ApiAuthParams authParams = new ApiAuthParams()
            {
                AccessToken = "a0ae25b183f4fedca6fcae08d8dd56b298837e11135133a613804d5fe53001329a97988c0af30f9c6ea1d"
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
                var s = api.Groups.GetLongPollServer(185277791);

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
                    if (update.Type == GroupUpdateType.MessageNew)
                        api.Messages.Send(responser.ConstructResponse(update.Message));
            }
        }
    }
}

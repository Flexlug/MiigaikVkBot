using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace MiigaikVkBot.Responsers
{
    public class Responser
    {
        public long GroupID;
        public VkApi VkController;

        public Responser(long _groupId, VkApi _vkController)
        {
            VkController = _vkController;
            GroupID = _groupId;
        }

        public virtual MessagesSendParams ConstructResponse(Message message)
        {
            throw new NotImplementedException($"Responser with group id {GroupID} has unimplemented method ContructResponse");
        }
    }
}

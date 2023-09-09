using System;
using VkNet;
using VkNet.Model;

namespace MiigaikVkBot.Responser
{
    public class BaseResponser
    {
        public long GroupID;
        public VkApi VkController;

        public BaseResponser(long _groupId, VkApi _vkController)
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

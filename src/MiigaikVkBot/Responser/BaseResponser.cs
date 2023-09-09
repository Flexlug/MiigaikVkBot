using System;
using VkNet;
using VkNet.Model;

namespace MiigaikVkBot.Responser
{
    public class BaseResponser
    {
        public ulong GroupID;
        public VkApi VkController;

        public BaseResponser(ulong _groupId, VkApi _vkController)
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Constants
{
    public class ChatHubEvent
    {
        public const string ReceiveMessage = "ReceiveMessage";
        public const string UserOffline = "UserOffline";
        public const string UserOnline = "UserOnline";
        public const string SeenMessage = "SeenMessage";
        public const string UserLeft = "UserLeft";
        public const string UserJoined = "UserJoined";
        public const string UserTyping = "UserTyping";
    }
}
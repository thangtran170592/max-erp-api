namespace Core.Constants
{
    public static class ChatHubEvent
    {
        public const string UserOnline = "UserOnline";
        public const string UserOffline = "UserOffline";
        public const string UserStatusChanged = "UserStatusChanged";
        public const string UserJoined = "UserJoined";
        public const string UserLeft = "UserLeft";
        public const string ReceiveMessage = "ReceiveMessage";
        public const string MessageStatusUpdated = "MessageStatusUpdated";
        public const string ReceiveBroadcast = "ReceiveBroadcast";
        public const string BroadcastSent = "BroadcastSent";
        public const string UserStartedTyping = "UserStartedTyping";
        public const string UserStoppedTyping = "UserStoppedTyping";
    }
}
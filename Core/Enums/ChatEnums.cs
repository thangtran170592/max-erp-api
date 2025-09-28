namespace Core.Enums
{
    public enum ConversationType
    {
        Private = 1,
        Group = 2,
        Broadcast = 3
    }

    public enum MessageType
    {
        Text = 1,
        Image = 2,
        File = 3,
        System = 4
    }

    public enum MessageStatus
    {
        Sent = 1,
        Delivered = 2,
        Read = 3,
        Failed = 4
    }

    public enum ReceiptStatus
    {
        Sent = 1,
        Delivered = 2,
        Read = 3
    }

    public enum BroadcastStatus
    {
        Pending = 1,
        Sent = 2,
        Failed = 3,
        Scheduled = 4
    }

    public enum ChatRole
    {
        Owner = 1,
        Member = 2
    }

    public static class ChatEnumExtensions
    {
        public static string GetTitle(this ChatRole role) => role switch
        {
            ChatRole.Owner => "Owner",
            ChatRole.Member => "Member",
            _ => "Unknown"
        };

        public static string GetTitle(this ReceiptStatus status) => status switch
        {
            ReceiptStatus.Sent => "Sent",
            ReceiptStatus.Delivered => "Delivered",
            ReceiptStatus.Read => "Read",
            _ => "Unknown"
        };
    }
}
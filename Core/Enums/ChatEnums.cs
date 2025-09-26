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
        Delivered = 1,
        Read = 2
    }

    public enum BroadcastStatus
    {
        Pending = 1,
        Sent = 2,
        Failed = 3,
        Scheduled = 4
    }
}
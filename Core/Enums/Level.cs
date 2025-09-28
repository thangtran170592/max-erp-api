namespace Core.Enums
{
    public enum Level
    {
        Beginner = 0,
        Intermediate = 1,
        Advanced = 2,
        Expert = 3
    }

    public static class LevelExtensions
    {
        public static string GetTitle(this Level level) => level switch
        {
            Level.Beginner => "Beginner",
            Level.Intermediate => "Intermediate",
            Level.Advanced => "Advanced",
            Level.Expert => "Expert",
            _ => "Unknown"
        };
    }
}
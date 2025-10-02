namespace Core.Enums
{
    public enum Level
    {
        Beginner = 1,
        Intermediate = 2,
        Advanced = 3,
        Expert = 4
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
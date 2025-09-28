namespace Core.Enums
{
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2,
        Other = 3
    }

    public static class GenderExtensions
    {
        public static string GetTitle(this Gender gender) => gender switch
        {
            Gender.Male => "Male",
            Gender.Female => "Female",
            Gender.Other => "Other",
            _ => "Unknown"
        };
    }
}
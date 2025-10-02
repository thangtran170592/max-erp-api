namespace Core.Enums
{
    public enum Gender
    {
        Unknown = 1,
        Male = 2,
        Female = 3,
        Other = 4
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
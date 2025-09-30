namespace Application.Common.Helpers
{
    public static class StringHelper
    {
        public static string FriendlyUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var url = input.ToLowerInvariant()
                .Trim()
                .Replace(" ", "-")
                .Replace("_", "-")
                .Replace("--", "-");
            url = new string([.. url.Where(c => char.IsLetterOrDigit(c) || c == '-')]);
            url = url.Trim('-');
            return url;
        }
    }
}
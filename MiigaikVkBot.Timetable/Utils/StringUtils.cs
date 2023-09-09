namespace MiigaikVkBot.Timetable.Utils;

public static class StringUtils
{ 
    public static string Capitalize(this string source)
    {
        if (string.IsNullOrEmpty(source))
            return string.Empty;
        char[] letters = source.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }
}
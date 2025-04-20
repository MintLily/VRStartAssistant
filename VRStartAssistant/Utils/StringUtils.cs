namespace VRStartAssistant.Utils;

public static class StringUtils {
    /// <summary>
    /// Checks if the string contains multiple string values
    /// </summary>
    /// <param name="str1">this</param>
    /// <param name="chars">As many strings as you want to compare to the target string</param>
    /// <returns>Boolean indicating that any and all of your specified strings are contained in the target string (this)</returns>
    public static bool AndContainsMultiple(this string str1, params string[] chars) => chars.All(str1.ToLower().Contains);
    
    /// <summary>
    /// Checks if the string contains multiple values
    /// </summary>
    /// <param name="str1">this</param>
    /// <param name="strs">As many strings as you want to compare to the target string</param>
    /// <returns>Boolean indicating that any and all of your specified strings are contained in the target string (this)</returns>
    public static bool OrContainsMultiple(this string str1, params string[] strs) => strs.Any(str1.Contains);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="str1">this</param>
    /// <param name="strs">As many strings as you want to compare to the target string</param>
    /// <returns>Boolean indicating the target string is either equal or contains your specified list of strings, ignoring upper or lower case</returns>
    public static bool ContainsOrEqualsOrdinalIgnoreCase(this string str1, params string[] strs) =>
        strs.Any(s => str1.Contains(s, StringComparison.OrdinalIgnoreCase)) || strs.Any(s => str1.Equals(s, StringComparison.OrdinalIgnoreCase));
    
    /// <summary>
    /// Shows only the first n lines of a list of strings
    /// </summary>
    public static string ShowLines(this List<string> lines, int maxLines) => string.Join(Environment.NewLine, lines.Take(maxLines));
    
    /// <summary>
    /// Returns a bool if the inputted string is "true" or not
    /// </summary>
    /// <param name="input">this</param>
    /// <returns>Boolean indicating if the inputted string is "true" or not</returns>
    public static bool AsBool(this string input) => input.ToLower().Equals("true");

    /// <summary>
    /// Returns a bool if the inputted string is "true" or "false"
    /// </summary>
    /// <param name="boolean">this</param>
    /// <returns>"true" or "false"</returns>
    public static string AsString(this bool boolean) => boolean ? "true" : "false";

    /// <summary>
    /// Trys to parse string data as a double
    /// </summary>
    /// <param name="str">this</param>
    /// <returns>double</returns>
    public static double AsDouble(this string? str) => double.Parse(str!);

    /// <summary>
    /// Trys to parse string data as an integer
    /// </summary>
    /// <param name="str">this</param>
    /// <returns>integer</returns>
    public static int AsInt(this string? str) => int.Parse(str!);

    /// <summary>
    /// Replaces each character in a string with an asterisk
    /// </summary>
    /// <param name="thisString"></param>
    /// <returns>Each character as an asterisk</returns>
    public static string Redact(this string? thisString) {
        string? temp = null;
        temp = thisString!.ToCharArray().Aggregate(temp, (current, empty) => current + "*");
        return temp ?? "***************";
    }
    public static string SimpleRedact(this string? thisString) => "***************";
}
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
}
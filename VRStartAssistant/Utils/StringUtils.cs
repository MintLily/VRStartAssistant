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
}
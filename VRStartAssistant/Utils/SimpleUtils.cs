namespace VRStartAssistant.Utils;

public static class SimpleUtils {
    /// <summary>
    /// Runs an async Task without awaiting
    /// </summary>
    /// <param name="task">Task to be run</param>
    internal static void RunWithoutAwait(this Task task)
        => Task.Run(async () => await task).ConfigureAwait(false);

    /// <summary>
    /// A combination of floor and ceil for comparables
    /// </summary>
    /// <typeparam name="T">Type to compare</typeparam>
    /// <param name="value">Value to compare</param>
    /// <param name="min">Minimum value</param>
    /// <param name="max">Maximum value</param>
    /// <returns>Value, if within bounds. Min, if value smaller than min. Max, if value larger than max. If max is smaller than min, min has priority</returns>
    internal static T MinMax<T>(T value, T min, T max) where T : IComparable {
        if (value.CompareTo(min) < 0) return min;
        return value.CompareTo(max) > 0 ? max : value;
    }

    /// <summary>
    /// Makes the first character of a string into an uppercase char
    /// </summary>
    /// <param name="input">String to modify</param>
    /// <returns>Modified string</returns>
    internal static string FirstCharToUpper(this string input) =>
        string.IsNullOrEmpty(input) ? input : string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1));
}
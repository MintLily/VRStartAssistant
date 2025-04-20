namespace VRStartAssistant.Utils;

public static class Try {
    public static void Catch(Action action, bool emptyOnError = false) {
        try {
            action();
        }
        catch (Exception ex) {
            if (!emptyOnError) 
                Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    public static T Catch<T>(Func<T> func, bool emptyOnError = false) {
        try {
            return func();
        }
        catch (Exception ex) {
            if (!emptyOnError) 
                Console.WriteLine($"Exception: {ex.Message}");
            return default!;
        }
    }
}
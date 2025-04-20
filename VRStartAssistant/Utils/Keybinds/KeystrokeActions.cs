namespace VRStartAssistant.Utils.Keybinds;

public static class KeystrokeActions {
    private static readonly Dictionary<string, Action> KeyActions = new();
    private static readonly Dictionary<string, Func<Task>> KeyActionsAsync = new();
    
    public static void RegisterKeyCombinationAsync(ConsoleKey keyCode, Func<Task> action, bool ctrl = false, bool alt = false, bool shift = false)
        => KeyActionsAsync[GetCombinationKey(keyCode, ctrl, alt, shift)] = action;
    
    public static void RegisterKeyCombination(ConsoleKey keyCode, Action action, bool ctrl = false, bool alt = false, bool shift = false)
        => KeyActions[GetCombinationKey(keyCode, ctrl, alt, shift)] = action;

    private static string GetCombinationKey(ConsoleKey keyCode, bool ctrl, bool alt, bool shift) 
        => $"{(ctrl ? "CTRL+" : "")}{(alt ? "ALT+" : "")}{(shift ? "SHIFT+" : "")}{keyCode}";

    public static async Task OnKeyReleaseAsync(ConsoleKeyInfo keyInfo) {
        var isCtrl = (keyInfo.Modifiers & ConsoleModifiers.Control) != 0;
        var isAlt = (keyInfo.Modifiers & ConsoleModifiers.Alt) != 0;
        var isShift = (keyInfo.Modifiers & ConsoleModifiers.Shift) != 0;
        var combinationKey = GetCombinationKey(keyInfo.Key, isCtrl, isAlt, isShift);

        if (KeyActionsAsync.TryGetValue(combinationKey, out var action))
            await Try.Catch(action.Invoke);
        if (KeyActions.TryGetValue(combinationKey, out var action2))
            Try.Catch(action2.Invoke);
    }
}
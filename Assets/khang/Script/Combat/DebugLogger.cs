using UnityEngine;

public static class DebugLogger
{
    public static void Log(string message)
    {
        Debug.Log($"[BattleSystem] {message}");
    }

    public static void LogWarning(string message)
    {
        Debug.LogWarning($"[BattleSystem] {message}");
    }

    public static void LogError(string message)
    {
        Debug.LogError($"[BattleSystem] {message}");
    }
}
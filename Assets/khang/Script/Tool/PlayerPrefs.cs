using UnityEngine;
using UnityEditor;

public class ResetPlayerPrefs
{
    [MenuItem("Tools/Reset PlayerPrefs")]
    public static void Reset()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been reset.");
    }
}
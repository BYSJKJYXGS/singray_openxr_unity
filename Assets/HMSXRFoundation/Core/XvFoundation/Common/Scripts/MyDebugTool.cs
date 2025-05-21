using UnityEngine;


public sealed class MyDebugTool
{
    private static string TAG = "singray:";

    public static bool logEnable = true;


    public static void Log(object message)
    {
       
        if (!logEnable)
        {
            return;
        }

        Debug.Log(TAG + message);
    }

    public static void Log(object message, Object context)
    {

        if (!logEnable)
        {
            return;
        }

        Debug.Log(TAG + message, context);
    }

    public static void LogError(object message)
    {
        if (!logEnable)
        {
            return;
        }

        Debug.LogError(TAG + message);


    }

    public static void LogError(object message, Object context)
    {
        if (!logEnable)
        {
            return;
        }

        Debug.LogError(TAG + message, context);
    }

    public static void LogWarning(object message)
    {
        if (!logEnable)
        {
            return;
        }

        Debug.LogWarning(TAG + message);
    }
    public static void LogWarning(object message, Object context)
    {
        if (!logEnable)
        {
            return;
        }

        Debug.LogWarning(TAG + message, context);
    }
}

using UnityEngine;

public class GameSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Settings()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = -1;
#elif UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
#endif
    }
}
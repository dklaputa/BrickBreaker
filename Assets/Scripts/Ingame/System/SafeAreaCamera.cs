using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SafeAreaCamera : MonoBehaviour
{
    private void Awake()
    {
        var targetCamera = GetComponent<Camera>();
        if (targetCamera == null) return;
        var safeArea = Screen.safeArea;
        var bottomLength = safeArea.yMin;
        var topLength = Screen.height - safeArea.yMax;
        var availableLength = Screen.height - 2 * Mathf.Max(bottomLength, topLength);
        targetCamera.orthographicSize = 5 / availableLength * Screen.height;
    }
}

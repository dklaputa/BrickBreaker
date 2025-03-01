using TMPro;
using UnityEngine;

public class FpsScript : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    // Update is called once per frame
    private void Update()
    {
        float fps = Time.timeScale / Time.deltaTime;
        text.text = fps <= 59 ? fps.ToString("N1") : string.Empty;
    }
}
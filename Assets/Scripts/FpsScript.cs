using UnityEngine;
using UnityEngine.UI;

public class FpsScript : MonoBehaviour
{
    private Text text;

    // Use this for initialization
    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        var fps = Time.timeScale / Time.deltaTime;
        text.text = fps <= 55 ? fps.ToString() : "";
    }
}
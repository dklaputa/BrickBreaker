using UnityEngine;
using UnityEngine.UI;

public class FpsScript : MonoBehaviour
{
    private Text text;

    // Use this for initialization
    private void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        text.text = (1f / Time.deltaTime).ToString();
    }
}
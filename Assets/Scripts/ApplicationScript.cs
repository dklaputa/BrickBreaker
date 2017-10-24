using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationScript : MonoBehaviour
{
    public static ApplicationScript instance;

    private Text ropeTimes;
    private static readonly Color Yellow = new Color(215f / 255, 165f / 255, 57f / 255);

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Input.multiTouchEnabled = false;
        ropeTimes = GameObject.Find("RopeTimes").GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void SetRopeTimes(int times)
    {
        ropeTimes.text = times == -1 ? "" : times.ToString();
        ropeTimes.color = Color.Lerp(Color.white, Yellow, (times - 10) / 10f);
    }
}
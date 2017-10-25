﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationScript : MonoBehaviour
{
    public static ApplicationScript instance;

    public string miss = "Oops!";
    public string[] perfect = {"Good", "Great", "Perfect"};

    private Text ropeTimes;
    private Text subtitle;
    private int prefectCount;
    private static readonly Color Yellow = new Color(215f / 255, 165f / 255, 57f / 255);

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Input.multiTouchEnabled = false;
        ropeTimes = GameObject.Find("RopeTimes").GetComponent<Text>();
        subtitle = GameObject.Find("Subtitle").GetComponent<Text>();
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

    public void ShowPerfect()
    {
        CancelInvoke();
        prefectCount++;
        if (prefectCount <= perfect.Length) subtitle.text = perfect[prefectCount - 1];
        else subtitle.text = perfect[perfect.Length - 1] + "\n× " + (prefectCount - perfect.Length + 1);
        Invoke("HideSubtitle", 0.4f);
    }

    public void resetPerfectCount()
    {
        prefectCount = 0;
    }

    public void ShowMiss()
    {
        CancelInvoke();
        subtitle.text = miss;
        Invoke("HideSubtitle", 0.4f);
    }

    private void HideSubtitle()
    {
        subtitle.text = "";
    }
}
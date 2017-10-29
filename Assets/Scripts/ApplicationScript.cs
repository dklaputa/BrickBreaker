using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationScript : MonoBehaviour
{
    public static ApplicationScript instance;

    public GameObject ball;

    public string miss = "Oops!";
    public string[] perfect = {"Good", "Great", "Perfect"};

    private Text ropeGain;
    private Text subtitle;
    private Text subtitleTimes;
    private GameObject startTips;
    private GameObject startIntro;
    private int prefectCount;
    private bool isGameStart;
    private static readonly Color Yellow = new Color(215f / 255, 165f / 255, 57f / 255);

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Input.multiTouchEnabled = false;
        ropeGain = GameObject.Find("RopeGain").GetComponent<Text>();
        subtitle = GameObject.Find("Subtitle").GetComponent<Text>();
        subtitleTimes = GameObject.Find("SubtitleTimes").GetComponent<Text>();
        startTips = GameObject.Find("StartTips");
        startIntro = GameObject.Find("StartIntroAnim");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void ShowRopeGain(int gain)
    {
        ropeGain.text = gain == -1 ? "" : gain.ToString();
        ropeGain.color = Color.Lerp(Color.white, Yellow, (gain - 10) / 10f);
    }

    public bool IsGameStart()
    {
        return isGameStart;
    }

    public void GameStart(Vector2 position1, Vector2 position2)
    {
        isGameStart = true;
        startTips.SetActive(false);
        var ropeVector = position1 - position2;
        Vector2 up;
        if (ropeVector.x < 0) up = new Vector2(ropeVector.y, -ropeVector.x).normalized;
        else up = new Vector2(-ropeVector.y, ropeVector.x).normalized;
        var o = Instantiate(ball, (position1 + position2) / 2 + up, Quaternion.identity);
        o.GetComponent<BallScript>().setInitialSpeedDirection(-up);
    }

    public void HideStartIntroAnim()
    {
        if (startIntro.activeInHierarchy) startIntro.SetActive(false);
    }

    public void ShowPerfect()
    {
        CancelInvoke();
        prefectCount++;
        if (prefectCount <= perfect.Length) subtitle.text = perfect[prefectCount - 1];
        else
        {
            subtitle.text = perfect[perfect.Length - 1];
            subtitleTimes.text = "+" + (prefectCount - perfect.Length);
        }
        Invoke("HideSubtitle", 0.4f);
    }

    public void ResetPerfectCount()
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
        subtitleTimes.text = "";
    }
}
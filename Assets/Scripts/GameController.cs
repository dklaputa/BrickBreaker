﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject ballPrefab;

    public string miss = "Oops!";
    public string[] perfect = {"Good!", "Great!", "Perfect!!"};

    private Text assessmentText;
    private Text assessmentCount;
    private GameObject IntroductionInfo;
    private GameObject IntroductionAnimation;
    private int prefectShootCount;
    private int comboCount;
    private int totalPoints;
    private bool isGameStart;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Input.multiTouchEnabled = false;
        assessmentText = GameObject.Find("AssessmentText").GetComponent<Text>();
        assessmentCount = GameObject.Find("AssessmentCount").GetComponent<Text>();
        IntroductionInfo = GameObject.Find("IntroInfo");
        IntroductionAnimation = GameObject.Find("IntroAnim");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public bool IsGameStart()
    {
        return isGameStart;
    }

    public void GameStart(Vector2 position1, Vector2 position2)
    {
        isGameStart = true;
        var ropeVector = position1 - position2;
        Vector2 up;
        if (ropeVector.x < 0) up = new Vector2(ropeVector.y, -ropeVector.x).normalized;
        else up = new Vector2(-ropeVector.y, ropeVector.x).normalized;
        var o = Instantiate(ballPrefab, (position1 + position2) / 2 + up, Quaternion.identity);
        o.GetComponent<BallScript>().setInitialSpeedDirection(-up);
        BrickManager.instance.GameStart();
    }

    public void HideIntroductionInfo()
    {
        if (IntroductionAnimation.activeInHierarchy) IntroductionAnimation.SetActive(false);
        if (IntroductionInfo.activeInHierarchy) IntroductionInfo.SetActive(false);
    }

    public void PerfectShoot()
    {
        CancelInvoke();
        prefectShootCount++;
        if (prefectShootCount <= perfect.Length) assessmentText.text = perfect[prefectShootCount - 1];
        else
        {
            assessmentText.text = perfect[perfect.Length - 1];
            assessmentCount.text = "×" + (prefectShootCount - perfect.Length + 1);
        }
        Invoke("HideAssessment", 0.4f);
    }

    public void ResetPerfectCount()
    {
        prefectShootCount = 0;
    }

    public void Miss()
    {
        CancelInvoke();
        prefectShootCount = 0;
        assessmentText.text = miss;
        Invoke("HideAssessment", 0.4f);
    }

    private void HideAssessment()
    {
        assessmentText.text = "";
        assessmentCount.text = "";
    }

    public void GetPoints(Vector2 position, int points)
    {
        comboCount++;
        totalPoints += points * comboCount;
        PointsTextManager.instance.ShowPointsText(position, points * comboCount, comboCount);
    }

    public void ResetComboCount()
    {
        comboCount = 0;
    }
}
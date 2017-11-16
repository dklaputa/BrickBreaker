using System.Collections;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject ball;
    public GameObject gameOverMenu;

    public string miss;
    public string[] perfect;
    public string win;
    public string loss;

    private Text assessmentText;
    private Text assessmentCount;
    private Text scoreText;
    private Text energyText;
    private Image energyFill;
    private Image scoreBar;
    private float energyFillAmount;
    private bool isChangingEnergyFillAmount;

    private GameObject[] gameOverStar;
    private Text gameOverMessage;
    private Text gameOverScore;

    private GameObject introductionInfo;
    private GameObject introductionAnimation;
    private GameObject[] star;
    private int starNum;
    private int perfectShootCount;
    private int comboCount;
    private int totalPoints;
    private bool isGameStart;
    private bool isGameOver;
    private bool needRefreshScore;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Input.multiTouchEnabled = false;

        assessmentText = GameObject.Find("AssessmentText").GetComponent<Text>();
        assessmentCount = GameObject.Find("AssessmentCount").GetComponent<Text>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        introductionInfo = GameObject.Find("IntroInfo");
        introductionAnimation = GameObject.Find("IntroAnim");
        energyText = GameObject.Find("EnergyLvl").GetComponent<Text>();
        energyFill = GameObject.Find("EnergyFill").GetComponent<Image>();

        var bar = GameObject.Find("ScoreBar").transform;
        scoreBar = bar.GetChild(0).gameObject.GetComponent<Image>();
        star = new[] {bar.GetChild(1).gameObject, bar.GetChild(2).gameObject, bar.GetChild(3).gameObject};

        var gameOverTransform = gameOverMenu.transform;
        gameOverStar = new[]
        {
            gameOverTransform.GetChild(0).GetChild(0).gameObject, gameOverTransform.GetChild(1).GetChild(0).gameObject,
            gameOverTransform.GetChild(2).GetChild(0).gameObject
        };
        gameOverMessage = gameOverTransform.GetChild(3).gameObject.GetComponent<Text>();
        gameOverScore = gameOverTransform.GetChild(4).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void LateUpdate()
    {
        if (needRefreshScore)
        {
            scoreText.text = totalPoints.ToString();
            var percent = totalPoints / 80000f;
            scoreBar.fillAmount = percent;
            if (percent >= 1 && starNum < 3)
            {
                if (!star[2].activeInHierarchy) star[2].SetActive(true);
                if (!star[1].activeInHierarchy) star[1].SetActive(true);
                if (!star[0].activeInHierarchy) star[0].SetActive(true);
                starNum = 3;
            }
            else if (percent >= .75f && starNum < 2)
            {
                if (!star[1].activeInHierarchy) star[1].SetActive(true);
                if (!star[0].activeInHierarchy) star[0].SetActive(true);
                starNum = 2;
            }
            else if (percent >= .5f && starNum < 1)
            {
                if (!star[0].activeInHierarchy && percent >= .5f) star[0].SetActive(true);
                starNum = 1;
            }
            needRefreshScore = false;
        }
    }

    public bool IsGameStart()
    {
        return isGameStart;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public void GameStart(Vector2 position1, Vector2 position2)
    {
        isGameStart = true;
        var ropeVector = position1 - position2;
        Vector2 up;
        if (ropeVector.x < 0) up = new Vector2(ropeVector.y, -ropeVector.x).normalized;
        else up = new Vector2(-ropeVector.y, ropeVector.x).normalized;
        ball.transform.position = (position1 + position2) / 2 + up;
        ball.GetComponent<BallScript>().setInitialSpeedDirection(-up);
        ball.SetActive(true);
        BrickManager.instance.GameStart();
    }

    public void GameOver(bool isWin)
    {
        isGameOver = true;
        gameOverMessage.text = isWin ? win : loss;
        gameOverScore.text = "Score: " + totalPoints;
        if (isWin)
        {
            for (var i = 0; i < starNum; i++)
            {
                gameOverStar[i].SetActive(true);
            }
        }
        gameOverMenu.SetActive(true);
        BrickManager.instance.GameOver();
    }

    public void GameRestart()
    {
        SceneManager.LoadScene("Main");
    }

    public void HideIntroductionInfo()
    {
        if (introductionAnimation.activeInHierarchy) introductionAnimation.SetActive(false);
        if (introductionInfo.activeInHierarchy) introductionInfo.SetActive(false);
    }

    public void PerfectShoot()
    {
        StopCoroutine("HideAssessment");
        perfectShootCount++;
        if (perfectShootCount <= perfect.Length) assessmentText.text = perfect[perfectShootCount - 1];
        else
        {
            assessmentText.text = perfect[perfect.Length - 1];
            assessmentCount.text = "×" + (perfectShootCount - perfect.Length + 1);
        }
        StartCoroutine("HideAssessment");
    }

    public void ResetPerfectCount()
    {
        perfectShootCount = 0;
    }

    public void Miss()
    {
        StopCoroutine("HideAssessment");
        perfectShootCount = 0;
        assessmentText.text = miss;
        StartCoroutine("HideAssessment");
    }

    private IEnumerator HideAssessment()
    {
        yield return new WaitForSeconds(.4f);
        assessmentText.text = "";
        assessmentCount.text = "";
    }

    public int BreakBrick(int points)
    {
        comboCount++;
        var finalPoint = points * comboCount * (perfectShootCount + 1);
        totalPoints += finalPoint;
//        PointsTextManager.instance.ShowPointsText(position, finalPoint);
        needRefreshScore = true;
        return finalPoint;
    }

    public void ResetComboCount()
    {
        comboCount = 0;
    }

    public void SetBallLevel(int level, int maxLevel, Color ballColor)
    {
        energyText.text = level == maxLevel - 1 ? "Max" : (level + 1).ToString();
        ballColor.a = .4f;
        energyFill.color = ballColor;
        energyFillAmount = (float) level / (maxLevel - 1);
        if (!isChangingEnergyFillAmount) StartCoroutine("ChangeEnergyFillAmount");
    }

    private IEnumerator ChangeEnergyFillAmount()
    {
        isChangingEnergyFillAmount = true;
        for (;;)
        {
            var diff = Mathf.Abs(energyFillAmount - energyFill.fillAmount);
            if (diff < .03f)
            {
                energyFill.fillAmount = energyFillAmount;
                break;
            }
            energyFill.fillAmount += .03f * diff / (energyFillAmount - energyFill.fillAmount);
            yield return new WaitForSeconds(.03f);
        }
        isChangingEnergyFillAmount = false;
    }
    
    public void SlowDownTimeScale()
    {
        StopCoroutine("RestoreTimeScale");
        Time.timeScale = .1f;
        StartCoroutine("RestoreTimeScale");
    }

    private IEnumerator RestoreTimeScale()
    {
        yield return new WaitForSecondsRealtime(1);
        for (;;)
        {
            Time.timeScale += 2 * Time.deltaTime;
            if (Time.timeScale < 1) yield return null;
            else
            {
                Time.timeScale = 1;
                break;
            }
        }
    }
}
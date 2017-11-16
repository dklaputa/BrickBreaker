using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject ball;
    public GameObject gameOverMenu;

    public string miss;
    public string[] perfect;

    private Text stageText;
    private Text assessmentText;
    private Text assessmentCount;
    private Text scoreText;
    private Text energyText;
    private Image energyFill;
    private float energyFillAmount;
    private bool isChangingEnergyFillAmount;

    private Text gameOverScore;
    private Text gameOverStarNum;
    private Text gameOverHighestScore;
    private Text gameOverTotalStarNum;

    private GameObject introductionInfo;
    private GameObject introductionAnimation;
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
        stageText = GameObject.Find("StageNum").GetComponent<Text>();
        introductionInfo = GameObject.Find("IntroInfo");
        introductionAnimation = GameObject.Find("IntroAnim");
        energyText = GameObject.Find("EnergyLvl").GetComponent<Text>();
        energyFill = GameObject.Find("EnergyFill").GetComponent<Image>();

        var gameOverTransform = gameOverMenu.transform;
        gameOverStarNum = gameOverTransform.GetChild(2).gameObject.GetComponent<Text>();
        gameOverScore = gameOverTransform.GetChild(4).gameObject.GetComponent<Text>();
        gameOverHighestScore = gameOverTransform.GetChild(5).gameObject.GetComponent<Text>();
        gameOverTotalStarNum = gameOverTransform.GetChild(8).gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    private void LateUpdate()
    {
        if (!needRefreshScore) return;
        scoreText.text = totalPoints.ToString();

        int s;
        if ((s = BrickManager.instance.CheckStage(totalPoints)) > 0) stageText.text = (s + 1).ToString();
        needRefreshScore = false;
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
        up = ropeVector.x < 0
            ? new Vector2(ropeVector.y, -ropeVector.x).normalized
            : new Vector2(-ropeVector.y, ropeVector.x).normalized;
        ball.transform.position = (position1 + position2) / 2 + up;
        ball.GetComponent<BallScript>().SetInitialSpeedDirection(-up);
        ball.SetActive(true);
        BrickManager.instance.GameStart();
    }

    public void GameOver()
    {
        isGameOver = true;

        var starNumber = totalPoints / 20000;
        gameOverStarNum.text = starNumber.ToString();

        var totalStar = PlayerPrefs.GetInt("StarNumber");
        gameOverTotalStarNum.text = totalStar.ToString();
        if (starNumber > 0)
        {
            PlayerPrefs.SetInt("StarNumber", totalStar + starNumber);
            StartCoroutine(TotalStarNumberAnimation(totalStar, starNumber));
        }

        gameOverScore.text = "Score: " + totalPoints;
        var highest = PlayerPrefs.GetInt("HighestScore");
        if (totalPoints > highest)
        {
            highest = totalPoints;
            PlayerPrefs.SetInt("HighestScore", highest);
        }
        gameOverHighestScore.text = "Highest: " + highest;

        gameOverMenu.SetActive(true);
        BrickManager.instance.GameOver();
    }

    public void GameRestart()
    {
        SceneManager.LoadScene("Main");
    }

    private IEnumerator TotalStarNumberAnimation(int old, int diff)
    {
        yield return new WaitForSeconds(1.1f);
        for (var i = 1; i <= diff; i++)
        {
            gameOverTotalStarNum.text = (old + i).ToString();
            yield return new WaitForSeconds(.01f);
        }
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
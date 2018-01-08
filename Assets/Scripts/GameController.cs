using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
/// Game conductor.
/// </summary>
public class GameController : MonoBehaviour
{
    public static GameController instance;

    public GameObject ball;
    public GameObject gameOverMenu;
    public GameObject tutorial1;
    public GameObject tutorial2;
    public GameObject sampleBrick;

    public string miss;
    public string[] perfect;

    private Text distanceText;
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

    private GameObject introductionAnimation;

    private int perfectShootCount;
    private int comboCount;
    private int totalPoints;
    private int distanceUI;
    private int distance;

    private bool isGameStart;
    private bool isGameOver;
    private bool isShowingTutorial;
    private bool isRefreshingScore;
    private bool isRefreshingDistance;
    private static bool showTutorial = true;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        Input.multiTouchEnabled = false;

        assessmentText = GameObject.Find("AssessmentText").GetComponent<Text>();
        assessmentCount = GameObject.Find("AssessmentCount").GetComponent<Text>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        distanceText = GameObject.Find("DistanceNum").GetComponent<Text>();
        introductionAnimation = GameObject.Find("IntroAnim");

        energyText = GameObject.Find("EnergyLvl").GetComponent<Text>();
        energyFill = GameObject.Find("EnergyFill").GetComponent<Image>();

        var gameOverTransform = gameOverMenu.transform;
        gameOverStarNum = gameOverTransform.GetChild(2).gameObject.GetComponent<Text>();
        gameOverScore = gameOverTransform.GetChild(4).gameObject.GetComponent<Text>();
        gameOverHighestScore = gameOverTransform.GetChild(5).gameObject.GetComponent<Text>();
        gameOverTotalStarNum = gameOverTransform.GetChild(8).gameObject.GetComponent<Text>();
    }

    private void Start()
    {
        if (!showTutorial) return;
        StartCoroutine("ShowTutorial");
    }

    private IEnumerator ShowTutorial()
    {
        yield return new WaitForSeconds(.01f);
        tutorial1.SetActive(true);
        sampleBrick.SetActive(true);
        isShowingTutorial = true;
    }

    public void OnTutorialClick(int number)
    {
        if (number == 0)
        {
            Destroy(tutorial1);
            Destroy(sampleBrick);
            tutorial2.SetActive(true);
        }
        else
        {
            Destroy(tutorial2);
            isShowingTutorial = false;
            showTutorial = false;
        }
    }

    public bool IsShowingTutorial()
    {
        return isShowingTutorial;
    }

    // Game end on esc pressed.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    // Refresh score at the end of the frame.
    private IEnumerator RefreshScore()
    {
        isRefreshingScore = true;
        yield return new WaitForEndOfFrame();
        scoreText.text = totalPoints.ToString();
        isRefreshingScore = false;
    }

    // Refresh score at the end of the frame.
    private IEnumerator RefreshDistance()
    {
        isRefreshingDistance = true;
        for (;;)
        {
            distanceUI++;
            distanceText.text = distanceUI.ToString();
            yield return new WaitForSeconds(.05f);
            if (distanceUI == distance) break;
        }

        isRefreshingDistance = false;
    }

    public void SetDistance(int d)
    {
        distance = d;
        if (!isRefreshingDistance) StartCoroutine("RefreshDistance");
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
        var up = ropeVector.x < 0
            ? new Vector2(ropeVector.y, -ropeVector.x).normalized
            : new Vector2(-ropeVector.y, ropeVector.x).normalized;
        // Let the ball appear above the rope.
        ball.transform.position = (position1 + position2) / 2 + up;
        ball.GetComponent<BallScript>().SetInitialSpeedDirection(-up);
        ball.SetActive(true);
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
    }

    /// <summary>
    /// The ball hits the middle of the rope. Show UI and perfect count plus 1.
    /// </summary>
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

    /// <summary>
    /// Reset perfect count.
    /// </summary>
    public void ResetPerfectCount()
    {
        perfectShootCount = 0;
    }

    /// <summary>
    /// The ball hits the side of the rope and gets reflected. Show UI and perfect count reset.
    /// </summary>
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

    /// <summary>
    /// According to combo and perfect count, we calculate the points that the player should get.
    /// Final points = brick points * combo count * (perfect count + 1)
    /// </summary>
    /// <param name="points">Points that brick(s) has.</param>
    /// <returns>Final points that the player should get by breaking the brick(s).</returns>
    public int AddPointsConsiderCombo(int points)
    {
        comboCount++;
        var finalPoint = points * comboCount * (perfectShootCount + 1);
        totalPoints += finalPoint;
        if (!isRefreshingScore) StartCoroutine("RefreshScore");
        return finalPoint;
    }

    /// <summary>
    /// Add points without considering combo and perfect count.
    /// </summary>
    /// <param name="points">Points that brick(s) has.</param> 
    public void AddPointsIgnoreCombo(int points)
    {
        totalPoints += points;
        if (!isRefreshingScore) StartCoroutine("RefreshScore");
    }

    public void ResetComboCount()
    {
        comboCount = 0;
    }

    // Show ball level on UI.
    public void SetBallLevel(int level, int maxLevel, Color ballColor)
    {
        energyText.text = level == maxLevel - 1 ? "MAX" : (level + 1).ToString();
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
        yield return new WaitForSecondsRealtime(.5f);
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
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public enum Item
    {
        None,
        Division
    }

    public GameObject ballPrefab;

    public string miss;
    public string[] perfect;

    private Text assessmentText;
    private Text assessmentCount;
    private Text scoreText;
    private Text energyText;
    private Image energyFill;
    private Image scoreBar;
    private float energyFillAmount;
    private bool isChangeEnergyFillAmount;
    private GameObject introductionInfo;
    private GameObject introductionAnimation;
    private GameObject[] star;
    private BallScript ball;
    private int perfectShootCount;
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
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        introductionInfo = GameObject.Find("IntroInfo");
        introductionAnimation = GameObject.Find("IntroAnim");
        energyText = GameObject.Find("EnergyLvl").GetComponent<Text>();
        energyFill = GameObject.Find("EnergyFill").GetComponent<Image>();

        var bar = GameObject.Find("ScoreBar").transform;
        scoreBar = bar.GetChild(0).gameObject.GetComponent<Image>();
        star = new[] {bar.GetChild(1).gameObject, bar.GetChild(2).gameObject, bar.GetChild(3).gameObject};
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (!isChangeEnergyFillAmount) return;
        var diff = Mathf.Abs(energyFillAmount - energyFill.fillAmount);
        if (diff < .02f)
        {
            energyFill.fillAmount = energyFillAmount;
            isChangeEnergyFillAmount = false;
        }
        else
        {
            energyFill.fillAmount += .02f * diff / (energyFillAmount - energyFill.fillAmount);
        }
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
        ball = o.GetComponent<BallScript>();
        ball.setInitialSpeedDirection(-up);
        BrickManager.instance.GameStart();
    }

    public void HideIntroductionInfo()
    {
        if (introductionAnimation.activeInHierarchy) introductionAnimation.SetActive(false);
        if (introductionInfo.activeInHierarchy) introductionInfo.SetActive(false);
    }

    public void PerfectShoot()
    {
        CancelInvoke();
        perfectShootCount++;
        if (perfectShootCount <= perfect.Length) assessmentText.text = perfect[perfectShootCount - 1];
        else
        {
            assessmentText.text = perfect[perfect.Length - 1];
            assessmentCount.text = "×" + (perfectShootCount - perfect.Length + 1);
        }
        Invoke("HideAssessment", 0.4f);
    }

    public void ResetPerfectCount()
    {
        perfectShootCount = 0;
    }

    public void Miss()
    {
        CancelInvoke();
        perfectShootCount = 0;
        assessmentText.text = miss;
        Invoke("HideAssessment", 0.4f);
    }

    private void HideAssessment()
    {
        assessmentText.text = "";
        assessmentCount.text = "";
    }

    public void BreakBrick(Vector2 position, int points)
    {
        comboCount++;
        var finalPoint = points * comboCount * (perfectShootCount + 1);
        totalPoints += finalPoint;
        PointsTextManager.instance.ShowPointsText(position, finalPoint);
        scoreText.text = totalPoints.ToString();
        var percent = totalPoints / 100000f;
        scoreBar.fillAmount = percent;
        if (percent >= 1)
        {
            if (!star[2].activeInHierarchy) star[2].SetActive(true);
            if (!star[1].activeInHierarchy) star[1].SetActive(true);
            if (!star[0].activeInHierarchy) star[0].SetActive(true);
        }
        else if (percent >= .75f)
        {
            if (!star[1].activeInHierarchy) star[1].SetActive(true);
            if (!star[0].activeInHierarchy) star[0].SetActive(true);
        }
        else if (percent >= .5f)
        {
            if (!star[0].activeInHierarchy && percent >= .5f) star[0].SetActive(true);
        }
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
        isChangeEnergyFillAmount = true;
    }
}
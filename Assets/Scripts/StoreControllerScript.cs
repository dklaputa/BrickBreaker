using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreControllerScript : MonoBehaviour
{
    private const int Price = 20;
    private static readonly Color StarNotEnoughColor = new Color(255f / 255, 135f / 255, 135f / 255);

    private Text divisionCostText;
    private Text blackHoleCostText;
    private Text divisionCountText;
    private Text blackHoleCountText;
    private Text totalStarText;

    private int divisionCount;
    private int blackHoleCount;
    private int totalStar;

    public void onExitClick()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("DivisionCount")) PlayerPrefs.SetInt("DivisionCount", 1);
        if (!PlayerPrefs.HasKey("BlackHoleCount")) PlayerPrefs.SetInt("BlackHoleCount", 1);
        totalStarText = GameObject.Find("TotalStarNum").GetComponent<Text>();
        totalStar = PlayerPrefs.GetInt("StarNumber");
        totalStarText.text = totalStar.ToString();
        divisionCountText = GameObject.Find("DivisionCount").GetComponent<Text>();
        divisionCount = PlayerPrefs.GetInt("DivisionCount");
        divisionCountText.text = divisionCount.ToString();
        blackHoleCountText = GameObject.Find("BlackHoleCount").GetComponent<Text>();
        blackHoleCount = PlayerPrefs.GetInt("BlackHoleCount");
        blackHoleCountText.text = blackHoleCount.ToString();
        divisionCostText = GameObject.Find("DivisionCost").GetComponent<Text>();
        blackHoleCostText = GameObject.Find("BlackHoleCost").GetComponent<Text>();
        if (totalStar >= Price) return;
        divisionCostText.color = StarNotEnoughColor;
        blackHoleCostText.color = StarNotEnoughColor;
    }

    public void onBuyDivisionItem()
    {
        if (totalStar < Price) return;
        totalStar -= Price;
        divisionCount++;
        PlayerPrefs.SetInt("StarNumber", totalStar);
        PlayerPrefs.SetInt("DivisionCount", divisionCount);
        totalStarText.text = totalStar.ToString();
        divisionCountText.text = divisionCount.ToString();
        if (totalStar >= Price) return;
        divisionCostText.color = StarNotEnoughColor;
        blackHoleCostText.color = StarNotEnoughColor;
    }

    public void onBuyBlackHoleItem()
    {
        if (totalStar < Price) return;
        totalStar -= Price;
        blackHoleCount++;
        PlayerPrefs.SetInt("StarNumber", totalStar);
        PlayerPrefs.SetInt("BlackHoleCount", blackHoleCount);
        totalStarText.text = totalStar.ToString();
        blackHoleCountText.text = blackHoleCount.ToString();
        if (totalStar >= Price) return;
        divisionCostText.color = StarNotEnoughColor;
        blackHoleCostText.color = StarNotEnoughColor;
    }
}
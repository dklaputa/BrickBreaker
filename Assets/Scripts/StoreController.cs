using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreController : MonoBehaviour
{
    private readonly Color starNotEnough = new Color(255f / 255, 135f / 255, 135f / 255);
    private Text totalStar;
    private Text divisionCount;
    private Text blackHoleCount;
    private Text divisionCost;
    private Text blackHoleCost;

    public void onExitClick()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("DivisionCount")) PlayerPrefs.SetInt("DivisionCount", 1);
        if (!PlayerPrefs.HasKey("BlackHoleCount")) PlayerPrefs.SetInt("BlackHoleCount", 1);
        totalStar = GameObject.Find("TotalStarNum").GetComponent<Text>();
        var star = PlayerPrefs.GetInt("StarNumber");
        totalStar.text = star.ToString();
        divisionCount = GameObject.Find("DivisionCount").GetComponent<Text>();
        divisionCount.text = PlayerPrefs.GetInt("DivisionCount").ToString();
        blackHoleCount = GameObject.Find("BlackHoleCount").GetComponent<Text>();
        blackHoleCount.text = PlayerPrefs.GetInt("BlackHoleCount").ToString();
        divisionCost = GameObject.Find("DivisionCost").GetComponent<Text>();
        blackHoleCost = GameObject.Find("BlackHoleCost").GetComponent<Text>();
        if (star >= 10) return;
        divisionCost.color = starNotEnough;
        blackHoleCost.color = starNotEnough;
    }

    public void onBuyDivisionItem()
    {
        var star = PlayerPrefs.GetInt("StarNumber");
        if (star < 10) return;
        var item = PlayerPrefs.GetInt("DivisionCount");
        star -= 10;
        item++;
        PlayerPrefs.SetInt("StarNumber", star);
        PlayerPrefs.SetInt("DivisionCount", item);
        totalStar.text = star.ToString();
        divisionCount.text = item.ToString();
        if (star >= 10) return;
        divisionCost.color = starNotEnough;
        blackHoleCost.color = starNotEnough;
    }

    public void onBuyBlackHoleItem()
    {
        var star = PlayerPrefs.GetInt("StarNumber");
        if (star < 10) return;
        var item = PlayerPrefs.GetInt("BlackHoleCount");
        star -= 10;
        item++;
        PlayerPrefs.SetInt("StarNumber", star);
        PlayerPrefs.SetInt("BlackHoleCount", item);
        totalStar.text = star.ToString();
        blackHoleCount.text = item.ToString();
        if (star >= 10) return;
        divisionCost.color = starNotEnough;
        blackHoleCost.color = starNotEnough;
    }
}
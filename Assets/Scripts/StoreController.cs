using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreController : MonoBehaviour
{
    private Text totalStar;
    private Text divisionCount;
    private Text blackHoleCount;

    public void onExitClick()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("DivisionCount")) PlayerPrefs.SetInt("DivisionCount", 1);
        if (!PlayerPrefs.HasKey("BlackHoleCount")) PlayerPrefs.SetInt("BlackHoleCount", 1);
        totalStar = GameObject.Find("TotalStarNum").GetComponent<Text>();
        totalStar.text = PlayerPrefs.GetInt("StarNumber").ToString();
        divisionCount = GameObject.Find("DivisionCount").GetComponent<Text>();
        divisionCount.text = PlayerPrefs.GetInt("DivisionCount").ToString();
        blackHoleCount = GameObject.Find("BlackHoleCount").GetComponent<Text>();
        blackHoleCount.text = PlayerPrefs.GetInt("BlackHoleCount").ToString();
    }

    public void onBuyDivisionItem()
    {
        var star = PlayerPrefs.GetInt("StarNumber");
        if (star < 50) return;
        var item = PlayerPrefs.GetInt("DivisionCount");
        star -= 50;
        item++;
        PlayerPrefs.SetInt("StarNumber", star);
        PlayerPrefs.SetInt("DivisionCount", item);
        totalStar.text = star.ToString();
        divisionCount.text = item.ToString();
    }

    public void onBuyBlackHoleItem()
    {
        var star = PlayerPrefs.GetInt("StarNumber");
        if (star < 50) return;
        var item = PlayerPrefs.GetInt("BlackHoleCount");
        star -= 50;
        item++;
        PlayerPrefs.SetInt("StarNumber", star);
        PlayerPrefs.SetInt("BlackHoleCount", item);
        totalStar.text = star.ToString();
        blackHoleCount.text = item.ToString();
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreControllerScript : MonoBehaviour
{
    private const int Price = 20;
    private static readonly Color StarNotEnoughColor = new Color(255f / 255, 135f / 255, 135f / 255);

    [SerializeField] private TextMeshProUGUI divisionCostText;
    [SerializeField] private TextMeshProUGUI blackHoleCostText;
    [SerializeField] private TextMeshProUGUI divisionCountText;
    [SerializeField] private TextMeshProUGUI blackHoleCountText;
    [SerializeField] private TextMeshProUGUI totalStarText;

    private int divisionCount;
    private int blackHoleCount;
    private int totalStar;

    public void OnExitClick()
    {
        SceneManager.LoadScene("Menu");
    }

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("DivisionCount")) PlayerPrefs.SetInt("DivisionCount", 1);
        if (!PlayerPrefs.HasKey("BlackHoleCount")) PlayerPrefs.SetInt("BlackHoleCount", 1);
        totalStar = PlayerPrefs.GetInt("StarNumber");
        totalStarText.text = totalStar.ToString();
        divisionCount = PlayerPrefs.GetInt("DivisionCount");
        divisionCountText.text = divisionCount.ToString();
        blackHoleCount = PlayerPrefs.GetInt("BlackHoleCount");
        blackHoleCountText.text = blackHoleCount.ToString();
        if (totalStar >= Price) return;
        divisionCostText.color = StarNotEnoughColor;
        blackHoleCostText.color = StarNotEnoughColor;
    }

    public void OnBuyDivisionItem()
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

    public void OnBuyBlackHoleItem()
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
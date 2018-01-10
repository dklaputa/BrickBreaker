using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemManagerScript : MonoBehaviour
{
    public const int Division = 0;
    public const int BlackHole = 1;

    public static ItemManagerScript instance;

    private Image[] itemCovers;
    private Animator[] itemNumAnimators;
    private Text[] itemNumTexts;

    private bool[] itemActivated;
    private int[] itemDuration;
    private int[] itemLevel;
    private int[] itemNum;

    // Use this for initialization
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        itemCovers = new[]
        {
            transform.GetChild(0).GetChild(0).gameObject.GetComponent<Image>(),
            transform.GetChild(1).GetChild(0).gameObject.GetComponent<Image>()
        };
        itemNumTexts = new[]
        {
            transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Text>(),
            transform.GetChild(1).GetChild(1).GetChild(0).gameObject.GetComponent<Text>()
        };
        itemNumAnimators = new[]
        {
            transform.GetChild(0).GetChild(1).gameObject.GetComponent<Animator>(),
            transform.GetChild(1).GetChild(1).gameObject.GetComponent<Animator>()
        };
        itemActivated = new[] {false, false};

        if (!PlayerPrefs.HasKey("DivisionCount")) PlayerPrefs.SetInt("DivisionCount", 1);
        if (!PlayerPrefs.HasKey("BlackHoleCount")) PlayerPrefs.SetInt("BlackHoleCount", 1);
        if (!PlayerPrefs.HasKey("DivisionLevel")) PlayerPrefs.SetInt("DivisionLevel", 1);
        if (!PlayerPrefs.HasKey("BlackHoleLevel")) PlayerPrefs.SetInt("BlackHoleLevel", 1);
        if (!PlayerPrefs.HasKey("DivisionTime")) PlayerPrefs.SetInt("DivisionTime", 1);
        if (!PlayerPrefs.HasKey("BlackHoleTime")) PlayerPrefs.SetInt("BlackHoleTime", 1);
        itemNum = new[] {PlayerPrefs.GetInt("DivisionCount"), PlayerPrefs.GetInt("BlackHoleCount")};
        itemDuration = new[] {PlayerPrefs.GetInt("DivisionTime") * 5 + 5, PlayerPrefs.GetInt("BlackHoleTime") * 5 + 5};
        itemLevel = new[] {PlayerPrefs.GetInt("DivisionLevel"), PlayerPrefs.GetInt("BlackHoleLevel")};

        itemNumTexts[Division].text = itemNum[Division].ToString();
        itemNumTexts[BlackHole].text = itemNum[BlackHole].ToString();
    }

    // Check whether the item is activated. If activated reture the item level. Otherwise reture -1.
    public int CheckItemActivated(int item)
    {
        return itemActivated[item] ? itemLevel[item] : -1;
    }

    // Obtain a item by break the item brick.
    public void ObtainItem()
    {
        if (Random.value < .5f)
        {
            itemNum[Division]++;
            PlayerPrefs.SetInt("DivisionCount", itemNum[Division]);
            itemNumTexts[Division].text = itemNum[Division].ToString();
            itemNumAnimators[Division].SetTrigger("Shake");
        }
        else
        {
            itemNum[BlackHole]++;
            PlayerPrefs.SetInt("BlackHoleCount", itemNum[BlackHole]);
            itemNumTexts[BlackHole].text = itemNum[BlackHole].ToString();
            itemNumAnimators[BlackHole].SetTrigger("Shake");
        }
    }

    public void OnItemClick(int item)
    {
        if (itemActivated[item] || itemNum[item] < 1 || !GameControllerScript.instance.IsGameStart()) return;
        itemNum[item]--;
        PlayerPrefs.SetInt(item == Division ? "DivisionCount" : "BlackHoleCount", itemNum[item]);
        itemNumTexts[item].text = itemNum[item].ToString();
        itemActivated[item] = true;
        itemCovers[item].fillAmount = 1;
        itemCovers[item].gameObject.SetActive(true);
        StartCoroutine("CountDown", item);
    }

    public void OnItemDurationEnd(int item)
    {
        itemActivated[item] = false;
    }

    private IEnumerator CountDown(int item)
    {
        for (;;)
        {
            itemCovers[item].fillAmount -= .05f / itemDuration[item];
            if (itemCovers[item].fillAmount <= 0)
            {
                OnItemDurationEnd(item);
                itemCovers[item].gameObject.SetActive(false);
                break;
            }

            yield return new WaitForSeconds(.05f);
        }
    }
}
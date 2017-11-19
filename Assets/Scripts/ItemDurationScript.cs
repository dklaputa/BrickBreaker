using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
/// Manage item's duration and level.
/// </summary>
public class ItemDurationScript : MonoBehaviour
{
    private Image cover;
    private int itemIndex;
    private Text num;
    private float duration;
    private int level;

    public void Initialize(int item, float time)
    {
        itemIndex = item;
        duration = time;
    }

    public void LevelUp()
    {
        level++;
        num.text = level.ToString();
    }

    public int GetLevel()
    {
        return level;
    }

    // Use this for initialization
    private void Awake()
    {
        cover = transform.GetChild(0).gameObject.GetComponent<Image>();
        num = transform.GetChild(1).gameObject.GetComponent<Text>();
        level = 1;
    }

    private void OnEnable()
    {
        StartCoroutine("Refresh");
    }

    // Update is called once per frame
    private IEnumerator Refresh()
    {
        for (;;)
        {
            cover.fillAmount += .1f / duration;
            if (cover.fillAmount >= 1)
            {
                cover.fillAmount = 0;
                if (level > 1)
                {
                    level--;
                    num.text = level > 1 ? level.ToString() : "";
                }
                else
                {
                    Destory();
                    break;
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    private void Destory()
    {
        ItemManager.instance.OnItemDurationEnd(itemIndex);
        gameObject.SetActive(false);
    }
}
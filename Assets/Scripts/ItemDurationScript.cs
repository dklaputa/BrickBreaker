using UnityEngine;
using UnityEngine.UI;

public class ItemDurationScript : MonoBehaviour
{
    private Image cover;
    private int itemIndex;
    private Text num;
    private float duration;
    private int stack;

    public void Initialize(int item, float time)
    {
        itemIndex = item;
        duration = time;
    }

    public void AddStack()
    {
        stack++;
        num.text = stack.ToString();
    }

    public int GetStack()
    {
        return stack;
    }

    // Use this for initialization
    private void Awake()
    {
        cover = transform.GetChild(0).gameObject.GetComponent<Image>();
        num = transform.GetChild(1).gameObject.GetComponent<Text>();
        stack = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        cover.fillAmount += Time.deltaTime / duration;
        if (cover.fillAmount < 1) return;
        cover.fillAmount = 0;
        if (stack > 1)
        {
            stack--;
            num.text = stack > 1 ? stack.ToString() : "";
        }
        else Destory();
    }

    private void Destory()
    {
        ItemManager.instance.OnItemInvalid(itemIndex);
        gameObject.SetActive(false);
    }
}
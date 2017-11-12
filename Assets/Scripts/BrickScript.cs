using UnityEngine;
using UnityEngine.UI;

public class BrickScript : MonoBehaviour
{
    public int level;
    public Sprite[] numbers;
    private int points;

    private SpriteRenderer textLvl;
    private SpriteRenderer spriteRenderer;
    private bool isContainItem;

    private static readonly Color[] Colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 135f / 255, 135f / 255),
        new Color(139f / 255, 135f / 255, 255f / 255)
    };

    public void AddItem()
    {
        isContainItem = true;
        spriteRenderer.color = Color.white;
        Invoke("RemoveItem", 10);
    }

    public void RemoveItem()
    {
        isContainItem = false;
        spriteRenderer.color = Colors[level];
    }

    public void SetLevel(int lvl)
    {
        level = lvl;
        points = (lvl + 1) * 100;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textLvl = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.color = Colors[level];
        textLvl.sprite = numbers[level];
    }

    private void Remove()
    {
        gameObject.SetActive(false);
        BrickParticleManager.instance.ShowParticle(transform.position, Colors[level]);
    }

    public void Break(BallScript callback)
    {
        if (isContainItem && callback != null)
        {
            callback.SetCurrentItem(GameController.Item.Division);
        }
        GameController.instance.BreakBrick(transform.position, points);
        Remove();
        BrickManager.instance.CheckIsBrickAllDead();
    }
}
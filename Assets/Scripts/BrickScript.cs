using UnityEngine;
using UnityEngine.UI;

public class BrickScript : MonoBehaviour
{
    public int level;
    private int points;

    private Text textLvl;
    private SpriteRenderer spriteRenderer;
    private bool isContainItem;

    private static readonly Color[] Colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 152f / 255, 135f / 255),
        new Color(214f / 255, 135f / 255, 255f / 255)
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
        textLvl = transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
    }

    private void OnEnable()
    {
        spriteRenderer.color = Colors[level];
        textLvl.text = (level + 1).ToString();
    }

    private void Remove()
    {
        gameObject.SetActive(false);
        BrickParticleManager.instance.ShowParticle(transform.position, Colors[level]);
    }

    public void Break()
    {
        GameController.instance.HitBrick(transform.position, points);
        BrickManager.instance.CheckIsBrickAllDead();
        Remove();
    }
}
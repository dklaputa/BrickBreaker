using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BrickScript : MonoBehaviour
{
    public int level;
    public Sprite[] numbers;
    private int points;

    private SpriteRenderer textLvl;
    private SpriteRenderer brickBackground;
    private bool isContainItem;

    private static readonly Color[] Colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 135f / 255, 135f / 255),
        new Color(139f / 255, 135f / 255, 255f / 255)
    };

    public void AddItem()
    {
        isContainItem = true;
        brickBackground.color = Color.white;
        StartCoroutine("RemoveItem");
    }

    private IEnumerator RemoveItem()
    {
        yield return new WaitForSeconds(10);
        isContainItem = false;
        brickBackground.color = Colors[level];
    }

    public void SetLevel(int lvl)
    {
        level = lvl;
        points = (lvl + 1) * 100;
    }

    private void Awake()
    {
        textLvl = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        brickBackground = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        brickBackground.color = Colors[level];
        textLvl.sprite = numbers[level];
    }

    private void Remove()
    {
        gameObject.SetActive(false);
        BrickParticleManager.instance.ShowParticle(transform.position, Colors[level]);
    }

    public int Break()
    {
        if (isContainItem)
        {
            //item
        }
        Remove();
        BrickManager.instance.CheckIsBrickAllDead();
        return GameController.instance.BreakBrick(points);
    }
}
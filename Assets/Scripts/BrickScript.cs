using System.Collections;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    public int level;
    public Sprite[] numbers;
    private int points;

    private SpriteRenderer textLvl;
    private SpriteRenderer brickBackground;
    private bool isContainItem;
    private bool dead;

    private static readonly Color[] Colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 135f / 255, 135f / 255),
        new Color(135f / 255, 195f / 255, 255f / 255), new Color(139f / 255, 135f / 255, 255f / 255),
        new Color(180f / 255, 135f / 255, 255f / 255)
    };

    public void AddItem()
    {
        isContainItem = true;
        brickBackground.color = Color.white;
        StartCoroutine("RemoveItem"); // Item should be removed after 20 seconds if player does not break it.
    }

    private IEnumerator RemoveItem()
    {
        yield return new WaitForSeconds(20);
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
        dead = false;
    }

    private void Remove()
    {
        gameObject.SetActive(false);
        BrickParticleManager.instance.ShowParticle(transform.position, Colors[level]);
    }

    /// <summary>
    /// The brick broken.
    /// </summary>
    /// <returns>Points the player should get.</returns>
    public int Break()
    {
        if (dead) return 0; // Check whether the brick is already dead to avoid duplicated breaks.
        dead = true;
        if (isContainItem)
        {
            ItemManager.instance.ObtainItem();
            isContainItem = false;
        }
        Remove();
        BrickManager.instance.CheckIsBrickAllDead();
        return GameController.instance.BreakBrick(points);
    }
}
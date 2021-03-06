﻿using UnityEngine;

public class BrickScript : MonoBehaviour
{
    private static readonly Color[] Colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 135f / 255, 135f / 255),
        new Color(135f / 255, 195f / 255, 255f / 255), new Color(139f / 255, 135f / 255, 255f / 255),
        new Color(180f / 255, 135f / 255, 255f / 255)
    };

    public Sprite itemBrick;
    public int level;
    public Sprite normalBrick;
    public Sprite[] numbers;

    private int points;
    private bool dead;

    private SpriteRenderer textLvl;
    private SpriteRenderer brickBackground;

    public void SetLevel(int lvl)
    {
        level = lvl;
        points = lvl == -1 ? 100 : (lvl + 1) * 100;
//        brickBackground.color = Colors[level];
//        textLvl.sprite = numbers[level];
    }

    private void Awake()
    {
        textLvl = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        brickBackground = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (level == -1)
        {
            brickBackground.sprite = itemBrick;
            brickBackground.color = Color.white;
            textLvl.sprite = numbers[0];
        }
        else
        {
            brickBackground.sprite = normalBrick;
            brickBackground.color = Colors[level];
            textLvl.sprite = numbers[level];
        }

        dead = false;
    }

    private void Remove()
    {
        gameObject.SetActive(false);
        BrickParticlePoolScript.instance.ShowParticle(transform.position, level == -1 ? Color.white : Colors[level]);
    }

    /// <summary>
    ///     The brick broken.
    /// </summary>
    /// <returns>Points the player should get.</returns>
    public int Break()
    {
        if (dead) return 0; // Check whether the brick is already dead to avoid duplicated breaks.
        dead = true;
        if (level == -1) ItemManagerScript.instance.ObtainItem();
        Remove();
        BrickManagerScript.instance.CheckNeedNewRow();
        return points;
    }
}
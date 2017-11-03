﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickScript : MonoBehaviour
{
    public int level;

    public Color[] colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 152f / 255, 135f / 255),
        new Color(214f / 255, 135f / 255, 255f / 255)
    };

    public void SetLevel(int lvl)
    {
        level = lvl;
    }

    private void OnEnable()
    {
        GetComponent<SpriteRenderer>().color = colors[level];
        transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = (level + 1).ToString();
    }

    public void Remove()
    {
        gameObject.SetActive(false);
    }

//    private void OnCollisionEnter2D(Collision2D other)
//    {
//        if (other.gameObject.CompareTag("Ball"))
//        {
//            if (BallScript.instance.speedLvl >= level)
//            {
//                BallScript.instance.SpeedLevelChange(-level - 1);
//                Destroy(gameObject);
//            }
//        }
//    }
}
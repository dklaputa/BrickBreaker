﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsTextManager : ObjectPoolBehavior
{ 
    public static PointsTextManager instance; 

    private void Awake()
    {
        instance = this;
    } 

    public void ShowPointsText(Vector2 position, int points)
    {
        var text = GetAvailableObject();
        text.transform.position = position;
        text.GetComponent<Text>().text = "+" + points;
        text.SetActive(true);
    } 
}
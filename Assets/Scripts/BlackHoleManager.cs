using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleManager : ObjectPoolBehavior
{
    public static BlackHoleManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowBlackHole(Vector2 position)
    {
        var hole = GetAvailableObject();
        hole.transform.position = position; 
        hole.SetActive(true);
    }
}
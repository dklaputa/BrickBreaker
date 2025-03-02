﻿using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Black hole generator
/// </summary>
public class BlackHolePoolScript : ObjectPoolBehavior
{
    public static BlackHolePoolScript instance;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnDestroy()
    {
        instance = null;
    }

    public void ShowBlackHole(Vector2 position, int itemLevel)
    {
        float size = .1f + itemLevel * .1f;
        GameObject hole = GetAvailableObject();
        hole.transform.localPosition = position;
        hole.GetComponent<BlackHoleScript>().SetRange(size);
        hole.SetActive(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUIPoolScript : ObjectPoolBehavior
{
    public static ComboUIPoolScript instance;
    private bool needNewUI = true;
    private ComboUIScript comboUiScript;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnDestroy()
    {
        instance = null;
    }

    public void UpdateCombo(int comboCount)
    {
        if (needNewUI)
        {
            GameObject comboUI = GetAvailableObject();
            comboUiScript = comboUI.GetComponent<ComboUIScript>();
            comboUI.SetActive(true);
            needNewUI = false;
        }

        comboUiScript.SetCombo(comboCount);
    }

    public void SetPointsAndRemove(int points)
    {
        comboUiScript.SetPointsAndRemove(points);
        needNewUI = true;
    }
}
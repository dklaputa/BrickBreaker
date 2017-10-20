using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationScript : MonoBehaviour
{
    // Use this for initialization
    private void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
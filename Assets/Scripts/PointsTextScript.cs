using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsTextScript : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>(); 
    }

    private void OnEnable()
    { 
        text.color = Color.white;
        Invoke("Destory", .5f);
    }

    private void Update()
    {
        var color = text.color;
        color.a -= 2 * Time.deltaTime;
        text.color = color;
        transform.position = transform.position + Vector3.up * .1f * Time.deltaTime;
    }

    private void Destory()
    {
        gameObject.SetActive(false);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PointsTextScript : MonoBehaviour
{
    private Text text;
    private int p;

    public void SetPoints(int points)
    {
        p = points;
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        text.text = "+" + p;
        if (p > 10000) text.fontSize = 50;
        else if (p > 1000) text.fontSize = 40;
        else text.fontSize = 32;
        text.color = Color.white;
        StartCoroutine("Destory");
    }

    private void Update()
    {
        var color = text.color;
        color.a -= 2 * Time.deltaTime;
        text.color = color;
        transform.position = transform.position + Vector3.up * .1f * Time.deltaTime;
    }

    private IEnumerator Destory()
    {
        yield return new WaitForSeconds(.5f);
        gameObject.SetActive(false);
    }
}
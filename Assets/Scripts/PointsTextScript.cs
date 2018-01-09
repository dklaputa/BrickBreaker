using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PointsTextScript : MonoBehaviour
{
    private int p;
    private Text text;

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
        else if (p > 1000)
            text.fontSize = 40;
        else
            text.fontSize = 32;
        text.color = Color.white;
        StartCoroutine("Destory");
        StartCoroutine("Animation");
    }

    private IEnumerator Animation()
    {
        for (;;)
        {
            Color color = text.color;
            color.a -= .03f;
            text.color = color;
            transform.position = transform.position + Vector3.up * .003f;
            yield return new WaitForSeconds(.03f);
        }
    }

    private IEnumerator Destory()
    {
        yield return new WaitForSeconds(1);
        StopCoroutine("Animation");
        gameObject.SetActive(false);
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

public class PointsTextScript : MonoBehaviour
{
    private int p;
    private TextMeshPro text;

    public void SetPoints(int points)
    {
        p = points;
    }

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    private void OnEnable()
    {
        text.text = "+" + p;
        if (p > 10000) text.fontSize = 2.5f;
        else if (p > 1000)
            text.fontSize = 2f;
        else
            text.fontSize = 1.6f;
        text.color = Color.white;
        StartCoroutine(Remove());
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        for (;;)
        {
            Color color = text.color;
            color.a -= .05f;
            text.color = color;
            transform.position += Vector3.up * .005f;
            yield return new WaitForSeconds(.05f);
        }
    }

    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(1);
        StopCoroutine(Animation());
        gameObject.SetActive(false);
    }
}
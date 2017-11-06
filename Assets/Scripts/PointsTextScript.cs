using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsTextScript : MonoBehaviour
{
    private Text points;
    private Text combo;
    private int mPoints;
    private int mCombos;

    public Color Yellow = new Color(255f / 255, 197f / 255, 71f / 255);

    public void SetText(int points, int combos)
    {
        mPoints = points;
        mCombos = combos;
    }

    private void Awake()
    {
        points = transform.GetChild(0).GetComponent<Text>();
        combo = transform.GetChild(1).GetComponent<Text>();
    }

    private void OnEnable()
    {
        points.color = Color.white;
        combo.color = Color.Lerp(Color.white, Yellow, (mCombos - 1) / 4f);
        points.text = "+" + mPoints;
        combo.text = mCombos > 1 ? "コンボ×" + mCombos : "";
        Invoke("Destory", .5f);
    }

    private void Update()
    {
        var pointsColor = points.color;
        pointsColor.a -= 2 * Time.deltaTime;
        points.color = pointsColor;
        var comboColor = combo.color;
        comboColor.a -= 2 * Time.deltaTime;
        combo.color = comboColor;
        transform.position = transform.position + Vector3.up * .1f * Time.deltaTime;
    }

    private void Destory()
    {
        gameObject.SetActive(false);
    }
}
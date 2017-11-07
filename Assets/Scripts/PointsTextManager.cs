using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsTextManager : MonoBehaviour
{
    public int initialPoolSize;
    public GameObject pointsTextPrefab;
    public static PointsTextManager instance;

    private List<GameObject> pointsTextPool;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        pointsTextPool = new List<GameObject>();
        for (var i = 0; i < initialPoolSize; i++)
        {
            pointsTextPool.Add(Instantiate(pointsTextPrefab, transform));
        }
    }

    public void ShowPointsText(Vector2 position, int points)
    {
        var text = GetAvailablePointsText();
        text.transform.position = position;
        text.GetComponent<Text>().text = "+" + points;
        text.SetActive(true);
    }

    private GameObject GetAvailablePointsText()
    {
        foreach (var text in pointsTextPool)
        {
            if (!text.activeInHierarchy)
            {
                return text;
            }
        }
        var newText = Instantiate(pointsTextPrefab, transform);
        pointsTextPool.Add(newText);
        return newText;
    }
}
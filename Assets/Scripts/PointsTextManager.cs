using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Points text generator.
/// </summary>
public class PointsTextManager : ObjectPoolBehavior
{
    public static PointsTextManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowPointsText(Vector2 position, int points)
    {
        GameObject text = GetAvailableObject();
        text.transform.position = position;
        text.GetComponent<PointsTextScript>().SetPoints(points);
        text.SetActive(true);
    }
}
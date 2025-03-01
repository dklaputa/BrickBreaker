using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Points text generator.
/// </summary>
public class PointsTextPoolScript : ObjectPoolBehavior
{
    public static PointsTextPoolScript instance;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnDestroy()
    {
        instance = null;
    }

    public void ShowPointsText(Vector2 position, int points)
    {
        GameObject text = GetAvailableObject();
        text.transform.localPosition = position;
        text.GetComponent<PointsTextScript>().SetPoints(points);
        text.SetActive(true);
    }
}
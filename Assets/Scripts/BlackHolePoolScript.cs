using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Black hole generator
/// </summary>
public class BlackHolePoolScript : ObjectPoolBehavior
{
    public static BlackHolePoolScript instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowBlackHole(Vector2 position, int itemLevel)
    {
        float size = .1f + itemLevel * .1f;
        GameObject hole = GetAvailableObject();
        hole.transform.position = position;
        hole.GetComponent<BlackHoleScript>().SetRange(size);
        hole.SetActive(true);
    }
}
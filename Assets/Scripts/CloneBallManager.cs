using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Division balls generator.
/// </summary>
public class CloneBallManager : ObjectPoolBehavior
{
    public static CloneBallManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ShowCloneBalls(Vector2 position, Vector2 direction, int speedLvl, int itemLevel)
    {
        int pairNum = itemLevel;
        for (int i = 1; i <= pairNum; i++)
        {
            GameObject ball1 = GetAvailableObject();
            ball1.transform.position = position;
            ball1.GetComponent<CloneBallScript>().SetInitialSpeed(RotateVector(direction, .2f * i / pairNum), speedLvl);
            ball1.SetActive(true);
            GameObject ball2 = GetAvailableObject();
            ball2.transform.position = position;
            ball2.GetComponent<CloneBallScript>()
                .SetInitialSpeed(RotateVector(direction, -.2f * i / pairNum), speedLvl);
            ball2.SetActive(true);
        }
    }

    private static Vector2 RotateVector(Vector2 vector, float angle)
    {
        return new Vector2(vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
            vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle)).normalized;
    }
}
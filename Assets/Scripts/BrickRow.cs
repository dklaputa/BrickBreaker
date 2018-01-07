using System.Linq;
using UnityEngine;

public class BrickRow : MonoBehaviour
{
    public GameObject brickPrefab;
    public float hSpacing;

    private const float brickWidth = 0.512f;

    // Probability of each brick level for each stage level. 
    private static readonly float[][] Possibility =
    {
        new[] {.8f, .2f, 0, 0}, new[] {.7f, .25f, .05f, 0}, new[] {.6f, .3f, .1f, 0}, new[] {.5f, .35f, .1f, .05f},
        new[] {.4f, .4f, .15f, .05f}, new[] {.3f, .4f, .15f, .1f}, new[] {.3f, .4f, .1f, .1f}
    };

    private int count;
    private BrickScript[] bricks;

    public void Initialize(int brickCount)
    {
        count = brickCount;
        bricks = new BrickScript[count];
        var hPosition = -(brickWidth + hSpacing) * (count - 1) / 2;

        for (var i = 0; i < count; i++)
        {
            var brick = Instantiate(brickPrefab, transform);
            brick.transform.position = new Vector2(hPosition + (hSpacing + brickWidth) * i, transform.position.y);
            bricks[i] = brick.GetComponent<BrickScript>();
        }
    }

    public bool IsAllDead()
    {
        return !bricks.Any(brick => brick.isActiveAndEnabled);
    }

    /// <summary>
    /// Generate a row of bricks. Each brick is presented as a vector3 (x, y, z) which means the brick is at the position (x, y) with brick level z.
    /// </summary>
    /// <param name="stage">Current stage.</param>
    public void SetStage(int stage)
    {
        var accumPos = new float[4];
        var level = Mathf.Min(stage, 6);
        accumPos[0] = Possibility[level][0];
        for (var n = 1; n < 4; n++)
        {
            accumPos[n] = accumPos[n - 1] + Possibility[level][n];
        }

        for (var i = 0; i < count; i++)
        {
            var random = Random.value;
            if (random < accumPos[0]) bricks[i].SetLevel(0);
            else if (random < accumPos[1])
                bricks[i].SetLevel(1);
            else if (random < accumPos[2])
                bricks[i].SetLevel(2);
            else if (random < accumPos[3])
                bricks[i].SetLevel(3);
            else
                bricks[i].SetLevel(4);
            bricks[i].gameObject.SetActive(true);
        }
    }
}
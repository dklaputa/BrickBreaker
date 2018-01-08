using System.Linq;
using UnityEngine;

public class BrickRow : MonoBehaviour
{
    public GameObject brickPrefab;
    public float hSpacing;

    private const float brickWidth = 0.512f;

    // Probability of each brick level for each stage level. 
    private static readonly float[][] Probability =
    {
        new[] {.8f, .2f, 0, 0}, new[] {.7f, .25f, .05f, 0}, new[] {.6f, .3f, .1f, 0}, new[] {.5f, .35f, .1f, .05f},
        new[] {.4f, .4f, .15f, .05f}, new[] {.3f, .4f, .15f, .1f}, new[] {.3f, .4f, .1f, .1f}
    };

    private const float itemProbability = .005f;


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
    /// Randomly set brick levels according to current stage.
    /// </summary>
    /// <param name="stage">Current stage.</param>
    public void SetStage(int stage)
    {
        var accumProbability = new float[4];
        var level = Mathf.Min(stage, 6);
        accumProbability[0] = Probability[level][0];
        for (var n = 1; n < 4; n++)
        {
            accumProbability[n] = accumProbability[n - 1] + Probability[level][n];
        }

        for (var i = 0; i < count; i++)
        {
            var random = Random.value;
            if (random < itemProbability)
            {
                bricks[i].SetLevel(-1);
                bricks[i].gameObject.SetActive(true);
                continue;
            }

            random = Random.value;
            if (random < accumProbability[0]) bricks[i].SetLevel(0);
            else if (random < accumProbability[1])
                bricks[i].SetLevel(1);
            else if (random < accumProbability[2])
                bricks[i].SetLevel(2);
            else if (random < accumProbability[3])
                bricks[i].SetLevel(3);
            else
                bricks[i].SetLevel(4);
            bricks[i].gameObject.SetActive(true);
        }
    }
}
using System.Linq;
using UnityEngine;

public class BrickRow : MonoBehaviour
{
    private const float BrickWidth = 0.512f; 
    private const float ItemProbability = .005f;

    // Probability of each brick level for each stage level. 
    private static readonly float[][] Probability =
    {
        new[] {.8f, .2f, 0, 0}, new[] {.7f, .25f, .05f, 0}, new[] {.6f, .3f, .1f, 0}, new[] {.5f, .35f, .1f, .05f},
        new[] {.4f, .4f, .15f, .05f}, new[] {.3f, .4f, .15f, .1f}, new[] {.3f, .4f, .1f, .1f}
    };

    public GameObject brickPrefab;
    public float hSpacing;
    
    private BrickScript[] bricks;
    private int count;

    public void Initialize(int brickCount)
    {
        count = brickCount;
        bricks = new BrickScript[count];
        float hPosition = -(BrickWidth + hSpacing) * (count - 1) / 2;

        for (int i = 0; i < count; i++)
        {
            GameObject brick = Instantiate(brickPrefab, transform);
            brick.transform.position = new Vector2(hPosition + (hSpacing + BrickWidth) * i, transform.position.y);
            bricks[i] = brick.GetComponent<BrickScript>();
        }
    }

    public bool IsAllDead()
    {
        return !bricks.Any(brick => brick.isActiveAndEnabled);
    }

    /// <summary>
    ///     Randomly set brick levels according to current stage.
    /// </summary>
    /// <param name="stage">Current stage.</param>
    public void SetStage(int stage)
    {
        float[] accumProbability = new float[4];
        int level = Mathf.Min(stage, 6);
        accumProbability[0] = Probability[level][0];
        for (int n = 1; n < 4; n++) accumProbability[n] = accumProbability[n - 1] + Probability[level][n];

        for (int i = 0; i < count; i++)
        {
            float random = Random.value;
            if (random < ItemProbability)
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
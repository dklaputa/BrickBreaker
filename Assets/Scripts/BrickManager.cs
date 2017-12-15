using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <inheritdoc />
/// <summary>
/// Brick generator
/// </summary>
public class BrickManager : ObjectPoolBehavior
{
    public static BrickManager instance;

    // The position where the brick parent should be.
    private Vector2 targetPosition;

    // Probability of each brick level for each stage level. 
    private static readonly float[][] Possibility =
    {
        new[] {.8f, .2f, 0, 0}, new[] {.7f, .25f, .05f, 0}, new[] {.6f, .3f, .1f, 0}, new[] {.5f, .35f, .1f, .05f},
        new[] {.4f, .4f, .15f, .05f}, new[] {.3f, .4f, .15f, .1f}, new[] {.3f, .4f, .1f, .1f}
    };

    private int stage;
    private float nextRowPosition;
    private int nextRow;

    private bool isCheckinigAllDead;
    private bool isCheckingTouchLine;
    private bool isMoving;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        for (; nextRow < 7; nextRow++)
        {
            var row = RandomRow(stage, nextRow % 2 == 0, nextRowPosition);
            nextRowPosition += .34f;
            foreach (var vector in row)
            {
                var brick = GetAvailableObject();
                brick.transform.position = (Vector2) (transform.position + vector);
                brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                brick.SetActive(true);
            }
        }
        targetPosition = transform.position;
    }

    /// <summary>
    /// Generate a row of bricks. Each brick is presented as a vector3 (x, y, z) which means the brick is at the position (x, y) with brick level z.
    /// </summary>
    /// <param name="stage">Current stage.</param>
    /// <param name="flag">The row should has 6 or 7 bricks? true for 6 bricks and false for 7 bricks.</param>
    /// <param name="vPosition">Position of the row related to the parent.</param>
    private static IEnumerable<Vector3> RandomRow(int stage, bool flag, float vPosition)
    {
        var length = flag ? 6 : 7;
        var bricks = new Vector3[length];
        var hPosition = flag ? -1.5f : -1.8f;
        var accumPos = new float[4];
        var level = Mathf.Min(stage, 6);
        accumPos[0] = Possibility[level][0];
        for (var n = 1; n < 4; n++)
        {
            accumPos[n] = accumPos[n - 1] + Possibility[level][n];
        }
        for (var i = 0; i < length; i++)
        {
            var random = Random.value;
            int brickLevel;
            if (random < accumPos[0]) brickLevel = 0;
            else if (random < accumPos[1]) brickLevel = 1;
            else if (random < accumPos[2]) brickLevel = 2;
            else if (random < accumPos[3]) brickLevel = 3;
            else brickLevel = 4;
            bricks[i] = new Vector3(hPosition + i * .6f, vPosition, brickLevel);
        }
        return bricks;
    }

    private IEnumerator StartMove()
    {
        isMoving = true;
        for (;;)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime);
            if (!isCheckingTouchLine) StartCoroutine("CheckTouchLine");
            if ((Vector2) transform.position != targetPosition) yield return null;
            else break;
        }
        isMoving = false;
    }

    private IEnumerator CheckAllDead()
    {
        isCheckinigAllDead = true;
        yield return new WaitForSeconds(.2f);
        if (!pool.Any(brick => brick.activeInHierarchy))
        {
            StopCoroutine("NewBricksRow");
            var rows = 0;
            while (rows < 4)
            {
                var row = RandomRow(stage, nextRow++ % 2 == 0, nextRowPosition);
                nextRowPosition += .34f;
                foreach (var vector in row)
                {
                    var brick = GetAvailableObject();
                    brick.transform.position = (Vector2) (transform.position + vector);
                    brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                    brick.SetActive(true);
                }
                rows++;
            }
            targetPosition += Vector2.down * .34f * rows;
            if (!isMoving) StartCoroutine("StartMove");
            StartCoroutine("NewBricksRow");
        }
        isCheckinigAllDead = false;
    }

    private IEnumerator CheckTouchLine()
    {
        isCheckingTouchLine = true;
        yield return new WaitForSeconds(.2f);
        if (pool.Any(brick => brick.activeInHierarchy && brick.transform.position.y < 0))
        {
            if (!GameController.instance.IsGameOver())
                GameController.instance.GameOver();
        }
        isCheckingTouchLine = false;
    }

    private IEnumerator NewBricksRow()
    {
        for (;;)
        {
            yield return new WaitForSeconds(Mathf.Max(15 - stage * .5f, 1));
            targetPosition += Vector2.down * .34f;
            if (!isMoving) StartCoroutine("StartMove");
            var row = RandomRow(stage, nextRow++ % 2 == 0, nextRowPosition);
            nextRowPosition += .34f;
            foreach (var vector in row)
            {
                var brick = GetAvailableObject();
                brick.transform.position = (Vector2) (transform.position + vector);
                brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                brick.SetActive(true);
            }
        }
    }

    public void GameStart()
    {
        StartCoroutine("NewBricksRow");
        StartCoroutine("AddItem");
    }

    /// <summary>
    /// Need stage up?
    /// </summary>
    /// <param name="points">Current total points</param>
    /// <returns>New stage level if should stage up, or -1 otherwise.</returns>
    public int CheckStage(int points)
    {
        if (points / (40000 * Mathf.Pow(2, stage)) < 1) return -1;
        stage++;
        return stage;
    }

    public void GameOver()
    {
        StopAllCoroutines();
    }

    public void CheckIsBrickAllDead()
    {
        if (!isCheckinigAllDead) StartCoroutine("CheckAllDead");
    }

    // Add item to random bricks every 20 seconds. If the player breaks the brick, he/she will get a item.
    private IEnumerator AddItem()
    {
        for (;;)
        {
            yield return new WaitForSeconds(20);
            var bricks = pool.FindAll(brick => brick.activeInHierarchy);
            if (bricks.Count > 0)
                bricks[Random.Range(0, bricks.Count)].GetComponent<BrickScript>().AddItem();
        }
    }
}
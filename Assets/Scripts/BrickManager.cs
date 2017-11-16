using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BrickManager : ObjectPoolBehavior
{
    public static BrickManager instance;
    private Vector2 targetPosition;

    private static readonly float[][] Possibility =
    {
        new[] {.8f, .2f, 0, 0}, new[] {.7f, .25f, .05f, 0}, new[] {.6f, .3f, .1f, 0}, new[] {.5f, .35f, .1f, .05f},
        new[] {.4f, .4f, .15f, .05f}, new[] {.3f, .4f, .15f, .1f}, new[] {.3f, .4f, .1f, .1f}
    };

    private int stage;
    private float nextRowPosition;
    private int nextRow;
    private bool needCheckAllDead;
    private bool needCheckTouchLine;
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
            needCheckTouchLine = true;
            if ((Vector2) transform.position != targetPosition) yield return null;
            else break;
        }
        isMoving = false;
    }

    private IEnumerator CheckAllDead()
    {
        for (;;)
        {
            if (needCheckAllDead)
            {
                needCheckAllDead = false;
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
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    private IEnumerator CheckTouchLine()
    {
        for (;;)
        {
            if (needCheckTouchLine)
            {
                needCheckTouchLine = false;
                if (pool.Any(brick => brick.activeInHierarchy && brick.transform.position.y < 0))
                {
                    if (!GameController.instance.IsGameOver())
                        GameController.instance.GameOver();
                }
            }
            yield return new WaitForSeconds(.2f);
        }
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
        StartCoroutine("CheckAllDead");
        StartCoroutine("CheckTouchLine");
        StartCoroutine("NewBricksRow");
        StartCoroutine("AddItem");
    }

    public int CheckStage(int points)
    {
        if (points / (50000 * Mathf.Pow(2, stage)) < 1) return -1;
        stage++;
        return stage;
    }

    public void GameOver()
    {
        StopAllCoroutines();
    }

    public void CheckIsBrickAllDead()
    {
        needCheckAllDead = true;
    }

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
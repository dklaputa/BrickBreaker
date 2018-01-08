using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <inheritdoc />
/// <summary>
/// Brick generator
/// </summary>
public class BrickManager : MonoBehaviour
{
    public GameObject rowPrefab;
    public static BrickManager instance;
    public float vSpacing;
    public int rowCount;
    public float playAreaTop;

    private const float brickHeight = 0.256f;
    private Queue<BrickRow> rows;

    // The position where the brick container should be.
    private Vector2 targetPosition;

    private int distance;
    private int stage;
    private float nextRowPosition;

    private bool isCheckingNeedNewRow;
    private bool isMoving;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        targetPosition = new Vector2(0, playAreaTop - (vSpacing + brickHeight) * (rowCount - .5f));
        transform.position = targetPosition;
        rows = new Queue<BrickRow>(rowCount);
        for (var i = 0; i < rowCount; i++)
        {
            var row = Instantiate(rowPrefab, transform).GetComponent<BrickRow>();
            row.transform.position = new Vector2(0, transform.position.y + nextRowPosition);
            row.Initialize(i % 2 == 0 ? 6 : 7);
            row.SetStage(stage);
            rows.Enqueue(row);
            nextRowPosition += vSpacing + brickHeight;
        }
    }

    private IEnumerator StartMove()
    {
        isMoving = true;
        for (;;)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime);
            if ((Vector2) transform.position != targetPosition) yield return null;
            else break;
        }

        isMoving = false;
    }

    private IEnumerator CheckNewRow()
    {
        isCheckingNeedNewRow = true;
        yield return new WaitForSeconds(.2f);
        var count = 0;
        while (rows.Peek().IsAllDead())
        {
            var row = rows.Dequeue();
            row.transform.position = new Vector2(0, transform.position.y + nextRowPosition);
            row.SetStage(stage);
            rows.Enqueue(row);
            nextRowPosition += vSpacing + brickHeight;
            count++;
        }

        if (count != 0)
        {
            distance += count;
            GameController.instance.SetDistance(distance);
            CheckStage();
            targetPosition += Vector2.down * (vSpacing + brickHeight) * count;
            if (!isMoving) StartCoroutine("StartMove");
        }

        isCheckingNeedNewRow = false;
    }

    private void CheckStage()
    {
        if (distance / (5 * Mathf.Pow(2, stage)) > 1) stage++;
    }

    public void CheckNeedNewRow()
    {
        if (!isCheckingNeedNewRow) StartCoroutine("CheckNewRow");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Brick generator
/// </summary>
public class BrickManagerScript : MonoBehaviour
{
    private const float BrickHeight = 0.256f;

    public static BrickManagerScript instance;

    public float vSpacing;
    public float playAreaTop;
    public int rowCount;
    public GameObject rowPrefab;

    // The total rows the player has broken
    private int distance;
    private int stage;

    private bool isCheckingNeedNewRow;
    private bool isMoving;

    private Queue<BrickRowScript> rows;

    // The position where the brick container should be.
    private Vector2 targetPosition;
    private float nextRowPosition;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        targetPosition = new Vector2(0, playAreaTop - (vSpacing + BrickHeight) * (rowCount - .5f));
        transform.position = targetPosition;
        rows = new Queue<BrickRowScript>(rowCount);
        for (int i = 0; i < rowCount; i++)
        {
            BrickRowScript row = Instantiate(rowPrefab, transform).GetComponent<BrickRowScript>();
            row.transform.position = new Vector2(0, transform.position.y + nextRowPosition);
            row.Initialize(i % 2 == 0 ? 6 : 7); // 6 bricks for odd row, and 7 bricks for even row, from bottom to top. 
            row.SetStage(stage);
            rows.Enqueue(row);
            nextRowPosition += vSpacing + BrickHeight;
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
        int count = 0;
        while (rows.Peek().IsAllDead())
        {
            BrickRowScript row = rows.Dequeue();
            row.transform.position = new Vector2(0, transform.position.y + nextRowPosition);
            row.SetStage(stage);
            rows.Enqueue(row);
            nextRowPosition += vSpacing + BrickHeight;
            count++;
        }

        if (count != 0)
        {
            distance += count;
            GameControllerScript.instance.SetDistance(distance);
            CheckStage();
            targetPosition += Vector2.down * (vSpacing + BrickHeight) * count;
            if (!isMoving) StartCoroutine("StartMove");
        }

        isCheckingNeedNewRow = false;
    }

    private void CheckStage()
    {
        if (distance / (5 * Mathf.Pow(2, stage)) > 1)
            stage++; // When distance reaches 5, 10, 20, 40, ..., go to the next stage.
    }

    public void CheckNeedNewRow()
    {
        if (!isCheckingNeedNewRow) StartCoroutine("CheckNewRow");
    }
}
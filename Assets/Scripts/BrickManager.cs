using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickManager : ObjectPoolBehavior
{
    public static BrickManager instance;
    private Vector2 targetPosition;
    private bool startMove;

    private static readonly List<Vector3>[] Bricks =
    {
        new List<Vector3>
        {
            new Vector3(-1.8f, 0, 0),
            new Vector3(-1.2f, 0, 0),
            new Vector3(-0.6f, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0.6f, 0, 1),
            new Vector3(1.2f, 0, 0),
            new Vector3(1.8f, 0, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 0.34f, 0),
            new Vector3(-0.9f, 0.34f, 1),
            new Vector3(-0.3f, 0.34f, 0),
            new Vector3(0.3f, 0.34f, 0),
            new Vector3(0.9f, 0.34f, 1),
            new Vector3(1.5f, 0.34f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 0.68f, 0),
            new Vector3(-1.2f, 0.68f, 1),
            new Vector3(-0.6f, 0.68f, 0),
            new Vector3(0, 0.68f, 2),
            new Vector3(0.6f, 0.68f, 0),
            new Vector3(1.2f, 0.68f, 1),
            new Vector3(1.8f, 0.68f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 1.02f, 0),
            new Vector3(-0.9f, 1.02f, 1),
            new Vector3(-0.3f, 1.02f, 0),
            new Vector3(0.3f, 1.02f, 0),
            new Vector3(0.9f, 1.02f, 1),
            new Vector3(1.5f, 1.02f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 1.36f, 0),
            new Vector3(-1.2f, 1.36f, 0),
            new Vector3(-0.6f, 1.36f, 1),
            new Vector3(0, 1.36f, 1),
            new Vector3(0.6f, 1.36f, 1),
            new Vector3(1.2f, 1.36f, 0),
            new Vector3(1.8f, 1.36f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 1.7f, 0),
            new Vector3(-0.9f, 1.7f, 1),
            new Vector3(-0.3f, 1.7f, 0),
            new Vector3(0.3f, 1.7f, 0),
            new Vector3(0.9f, 1.7f, 1),
            new Vector3(1.5f, 1.7f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 2.04f, 0),
            new Vector3(-1.2f, 2.04f, 1),
            new Vector3(-0.6f, 2.04f, 0),
            new Vector3(0, 2.04f, 2),
            new Vector3(0.6f, 2.04f, 0),
            new Vector3(1.2f, 2.04f, 1),
            new Vector3(1.8f, 2.04f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 2.38f, 0),
            new Vector3(-0.9f, 2.38f, 1),
            new Vector3(-0.3f, 2.38f, 0),
            new Vector3(0.3f, 2.38f, 0),
            new Vector3(0.9f, 2.38f, 1),
            new Vector3(1.5f, 2.38f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 2.72f, 0),
            new Vector3(-1.2f, 2.72f, 0),
            new Vector3(-0.6f, 2.72f, 1),
            new Vector3(0, 2.72f, 1),
            new Vector3(0.6f, 2.72f, 1),
            new Vector3(1.2f, 2.72f, 0),
            new Vector3(1.8f, 2.72f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 3.06f, 0),
            new Vector3(-0.9f, 3.06f, 1),
            new Vector3(-0.3f, 3.06f, 0),
            new Vector3(0.3f, 3.06f, 0),
            new Vector3(0.9f, 3.06f, 1),
            new Vector3(1.5f, 3.06f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 3.4f, 0),
            new Vector3(-1.2f, 3.4f, 1),
            new Vector3(-0.6f, 3.4f, 0),
            new Vector3(0, 3.4f, 2),
            new Vector3(0.6f, 3.4f, 0),
            new Vector3(1.2f, 3.4f, 1),
            new Vector3(1.8f, 3.4f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 3.74f, 0),
            new Vector3(-0.9f, 3.74f, 1),
            new Vector3(-0.3f, 3.74f, 0),
            new Vector3(0.3f, 3.74f, 0),
            new Vector3(0.9f, 3.74f, 1),
            new Vector3(1.5f, 3.74f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 4.08f, 0),
            new Vector3(-1.2f, 4.08f, 0),
            new Vector3(-0.6f, 4.08f, 1),
            new Vector3(0, 4.08f, 1),
            new Vector3(0.6f, 4.08f, 1),
            new Vector3(1.2f, 4.08f, 0),
            new Vector3(1.8f, 4.08f, 0)
        },
    };

    private int nextRow;
    private bool needCheckAllDead;

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
            var row = Bricks[nextRow];
            foreach (var vector in row)
            {
                var brick = GetAvailableObject();
                brick.transform.position = (Vector2) (transform.position + vector);
                brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                brick.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (needCheckAllDead)
        {
            needCheckAllDead = false;
            if (!pool.Any(brick => brick.activeInHierarchy))
            {
                if (nextRow < Bricks.Length)
                {
                    CancelInvoke();
                    var rows = 0;
                    while (rows < 4 && nextRow < Bricks.Length)
                    {
                        var row = Bricks[nextRow++];
                        foreach (var vector in row)
                        {
                            var brick = GetAvailableObject();
                            brick.transform.position = (Vector2) (transform.position + vector);
                            brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                            brick.SetActive(true);
                        }
                        rows++;
                    }
                    targetPosition = new Vector2(transform.position.x, transform.position.y - .34f * rows);
                    startMove = true;
                    Invoke("NewBricksRow", 15);
                }
                else
                {
                    //You Won!
                }
            }
        }
        if (!startMove) return;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime);
        if ((Vector2) transform.position == targetPosition) startMove = false;
    }

    private void NewBricksRow()
    {
        targetPosition = new Vector2(transform.position.x, transform.position.y - .34f);
        startMove = true;
        if (nextRow < Bricks.Length)
        {
            var row = Bricks[nextRow++];
            foreach (var vector in row)
            {
                var brick = GetAvailableObject();
                brick.transform.position = (Vector2) (transform.position + vector);
                brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                brick.SetActive(true);
            }
        }
        Invoke("NewBricksRow", 15);
    }

    public void GameStart()
    {
        Invoke("NewBricksRow", 15);
        Invoke("RandomItem", 20);
    }

    public void CheckIsBrickAllDead()
    {
        needCheckAllDead = true;
    }

    private void RandomItem()
    {
        var bricks = pool.FindAll(brick => brick.activeInHierarchy);
        bricks[Random.Range(0, bricks.Count)].GetComponent<BrickScript>().AddItem();
        Invoke("RandomItem", 20);
    }
}
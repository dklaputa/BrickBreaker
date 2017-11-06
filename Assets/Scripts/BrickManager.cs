using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public int initialPoolSize = 50;
    public GameObject brickPrefab;
    public static BrickManager instance;
    private Vector2 targetPosition;
    private bool startMove;

    private readonly List<Vector3>[] bricks =
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
            new Vector3(-1.5f, 0.35f, 0),
            new Vector3(-0.9f, 0.35f, 1),
            new Vector3(-0.3f, 0.35f, 0),
            new Vector3(0.3f, 0.35f, 0),
            new Vector3(0.9f, 0.35f, 1),
            new Vector3(1.5f, 0.35f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 0.7f, 0),
            new Vector3(-1.2f, 0.7f, 1),
            new Vector3(-0.6f, 0.7f, 0),
            new Vector3(0, 0.7f, 2),
            new Vector3(0.6f, 0.7f, 0),
            new Vector3(1.2f, 0.7f, 1),
            new Vector3(1.8f, 0.7f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 1.05f, 0),
            new Vector3(-0.9f, 1.05f, 1),
            new Vector3(-0.3f, 1.05f, 0),
            new Vector3(0.3f, 1.05f, 0),
            new Vector3(0.9f, 1.05f, 1),
            new Vector3(1.5f, 1.05f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 1.4f, 0),
            new Vector3(-1.2f, 1.4f, 0),
            new Vector3(-0.6f, 1.4f, 1),
            new Vector3(0, 1.4f, 1),
            new Vector3(0.6f, 1.4f, 1),
            new Vector3(1.2f, 1.4f, 0),
            new Vector3(1.8f, 1.4f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 1.75f, 0),
            new Vector3(-0.9f, 1.75f, 1),
            new Vector3(-0.3f, 1.75f, 0),
            new Vector3(0.3f, 1.75f, 0),
            new Vector3(0.9f, 1.75f, 1),
            new Vector3(1.5f, 1.75f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 2.1f, 0),
            new Vector3(-1.2f, 2.1f, 1),
            new Vector3(-0.6f, 2.1f, 0),
            new Vector3(0, 2.1f, 2),
            new Vector3(0.6f, 2.1f, 0),
            new Vector3(1.2f, 2.1f, 1),
            new Vector3(1.8f, 2.1f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 2.45f, 0),
            new Vector3(-0.9f, 2.45f, 1),
            new Vector3(-0.3f, 2.45f, 0),
            new Vector3(0.3f, 2.45f, 0),
            new Vector3(0.9f, 2.45f, 1),
            new Vector3(1.5f, 2.45f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 2.8f, 0),
            new Vector3(-1.2f, 2.8f, 0),
            new Vector3(-0.6f, 2.8f, 1),
            new Vector3(0, 2.8f, 1),
            new Vector3(0.6f, 2.8f, 1),
            new Vector3(1.2f, 2.8f, 0),
            new Vector3(1.8f, 2.8f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 3.15f, 0),
            new Vector3(-0.9f, 3.15f, 1),
            new Vector3(-0.3f, 3.15f, 0),
            new Vector3(0.3f, 3.15f, 0),
            new Vector3(0.9f, 3.15f, 1),
            new Vector3(1.5f, 3.15f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 3.5f, 0),
            new Vector3(-1.2f, 3.5f, 1),
            new Vector3(-0.6f, 3.5f, 0),
            new Vector3(0, 3.5f, 2),
            new Vector3(0.6f, 3.5f, 0),
            new Vector3(1.2f, 3.5f, 1),
            new Vector3(1.8f, 3.5f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.5f, 3.85f, 0),
            new Vector3(-0.9f, 3.85f, 1),
            new Vector3(-0.3f, 3.85f, 0),
            new Vector3(0.3f, 3.85f, 0),
            new Vector3(0.9f, 3.85f, 1),
            new Vector3(1.5f, 3.85f, 0)
        },
        new List<Vector3>
        {
            new Vector3(-1.8f, 4.2f, 0),
            new Vector3(-1.2f, 4.2f, 0),
            new Vector3(-0.6f, 4.2f, 1),
            new Vector3(0, 4.2f, 1),
            new Vector3(0.6f, 4.2f, 1),
            new Vector3(1.2f, 4.2f, 0),
            new Vector3(1.8f, 4.2f, 0)
        },
    };

    private List<GameObject> brickPool = new List<GameObject>();
    private int nextRow;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        for (var i = 0; i < initialPoolSize; i++)
        {
            brickPool.Add(Instantiate(brickPrefab, transform));
        }
        for (; nextRow < 7; nextRow++)
        {
            var row = bricks[nextRow];
            foreach (var vector in row)
            {
                var brick = GetAvailableBrick();
                brick.transform.position = (Vector2) (transform.position + vector);
                brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                brick.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (!startMove) return;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime);
        if ((Vector2) transform.position == targetPosition) startMove = false;
    }

    private GameObject GetAvailableBrick()
    {
        foreach (var brick in brickPool)
        {
            if (!brick.activeInHierarchy)
            {
                return brick;
            }
        }
        var newBrick = Instantiate(brickPrefab, transform);
        brickPool.Add(newBrick);
        return newBrick;
    }

    private void NewBricksRow()
    {
        targetPosition = new Vector2(transform.position.x, transform.position.y - .35f);
        startMove = true;
        if (nextRow < bricks.Length)
        {
            var row = bricks[nextRow++];
            foreach (var vector in row)
            {
                var brick = GetAvailableBrick();
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
    }

    public void CheckIsBrickAllDead()
    {
        if (brickPool.Any(brick => brick.activeInHierarchy)) return;
        if (nextRow < bricks.Length)
        {
            CancelInvoke();
            var rows = 0;
            while (rows < 4 && nextRow < bricks.Length)
            {
                var row = bricks[nextRow++];
                foreach (var vector in row)
                {
                    var brick = GetAvailableBrick();
                    brick.transform.position = (Vector2) (transform.position + vector);
                    brick.GetComponent<BrickScript>().SetLevel((int) vector.z);
                    brick.SetActive(true);
                }
                rows++;
            }
            targetPosition = new Vector2(transform.position.x, transform.position.y - .35f * rows);
            startMove = true;
            Invoke("NewBricksRow", 15);
        }
        else
        {
            //You Won!
        }
    }
}
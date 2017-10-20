using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public int initialPoolSize = 2;
    public float minLength = .5f;
    public GameObject ropePrefab;

    List<GameObject> ropePool = new List<GameObject>();
    Vector2[] endPositions = new Vector2[2];

    // Use this for initialization
    private void Start()
    {
        for (var i = 0; i < initialPoolSize; i++)
        {
            ropePool.Add(Instantiate(ropePrefab, transform));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            endPositions[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endPositions[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((endPositions[1] - endPositions[0]).magnitude < minLength)
                return;
            var done = false;
            foreach (var rope in ropePool)
            {
                if (rope.activeInHierarchy)
                {
                    rope.GetComponent<RopeScript>().Remove();
                }
                else if (!done)
                {
                    rope.GetComponent<RopeScript>().SetEndPointPositions(endPositions[0], endPositions[1]);
                    rope.SetActive(true);
                    done = true;
                }
            }
            if (done) return;
            var newRope = Instantiate(ropePrefab, transform);
            newRope.GetComponent<RopeScript>().SetEndPointPositions(endPositions[0], endPositions[1]);
            newRope.SetActive(true);
            ropePool.Add(newRope);
        }
    }
}
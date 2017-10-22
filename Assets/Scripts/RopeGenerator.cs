using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public int initialPoolSize = 2;
    public float minLength = .5f;
    public GameObject ropePrefab;

    private List<GameObject> ropePool = new List<GameObject>();
    private List<Vector3> trail = new List<Vector3>();

    private LineRenderer lineRenderer;

    // Use this for initialization
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
            trail.Clear();
            trail.Add((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (trail.Count < 2 || Vector2.Distance(trail[trail.Count - 1], trail[trail.Count - 2]) > minLength &&
                Vector2.Angle(point - (Vector2) trail[trail.Count - 1],
                    trail[trail.Count - 1] - trail[trail.Count - 2]) > 120)
                trail.Add(point);
            else trail[trail.Count - 1] = point;
            lineRenderer.positionCount = trail.Count;
            lineRenderer.SetPositions(trail.ToArray());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.positionCount = 0;
            if (trail.Count < 2 || trail.Count == 2 && Vector2.Distance(trail[0], trail[1]) < minLength) return;
            var done = false;
            foreach (var rope in ropePool)
            {
                if (rope.activeInHierarchy)
                {
                    rope.GetComponent<RopeScript>().Remove();
                }
                else if (!done)
                {
                    rope.GetComponent<RopeScript>().SetEndPointPositions(trail[0], trail[1]);
                    rope.SetActive(true);
                    done = true;
                }
            }
            if (done) return;
            var newRope = Instantiate(ropePrefab, transform);
            newRope.GetComponent<RopeScript>().SetEndPointPositions(trail[0], trail[1]);
            newRope.SetActive(true);
            ropePool.Add(newRope);
        }
    }
}
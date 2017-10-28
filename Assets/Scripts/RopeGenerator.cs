using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    public int initialPoolSize = 2;
    public float minLength = .5f;
    public float maxLength = 3f;
    public GameObject ropePrefab;

    private List<GameObject> ropePool = new List<GameObject>();
    private List<Vector3> trail = new List<Vector3>();
    private GameObject firstRope;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        firstRope = transform.GetChild(0).gameObject;
    }

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
        if (!ApplicationScript.instance.IsGameStart())
        {
            if (Input.GetMouseButtonDown(0))
            {
                trail.Clear();
                ApplicationScript.instance.hideStartIntroAnim();
                trail.Add((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (trail.Count < 2) trail.Add(point);
                else trail[1] = point;
                var d = Vector2.Distance(trail[0], trail[1]);
                if (d > maxLength)
                    trail[1] = Vector2.Lerp(trail[0], trail[1], maxLength / d);
                lineRenderer.positionCount = 30;
                for (var i = 0; i < 30; i++)
                {
                    lineRenderer.SetPosition(i, Vector3.Lerp(trail[0], trail[1], i / 29f));
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                lineRenderer.positionCount = 0;
                if (trail.Count < 2 || Vector2.Distance(trail[0], trail[1]) < minLength) return;

                firstRope.GetComponent<FirstRopeScript>().Initialize(trail[0], trail[1]);
                firstRope.SetActive(true);
                ApplicationScript.instance.GameStart();
            }
            return;
        }

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
            {
                trail.Add(point);
                if (trail.Count > 3) ApplicationScript.instance.SetRopeTimes(trail.Count - 1);
            }
            else trail[trail.Count - 1] = point;
            var d = Vector2.Distance(trail[trail.Count - 1], trail[trail.Count - 2]);
            if (d > maxLength)
                trail[trail.Count - 1] = Vector2.Lerp(trail[trail.Count - 2], trail[trail.Count - 1], maxLength / d);
            if (trail.Count != 2 || d < minLength) return;
            lineRenderer.positionCount = 30;
            for (var i = 0; i < 30; i++)
            {
                lineRenderer.SetPosition(i, Vector3.Lerp(trail[0], trail[1], i / 29f));
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ApplicationScript.instance.SetRopeTimes(-1);
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
                    rope.GetComponent<RopeScript>().Initialize(trail[0], trail[1], trail.Count - 1);
                    rope.SetActive(true);
                    done = true;
                }
            }
            if (done) return;
            var newRope = Instantiate(ropePrefab, transform);
            newRope.GetComponent<RopeScript>().Initialize(trail[0], trail[1], trail.Count - 1);
            newRope.SetActive(true);
            ropePool.Add(newRope);
        }
    }
}
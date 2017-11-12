using System.Collections.Generic;
using UnityEngine;

public class RopeManager : ObjectPoolBehavior
{
    public float minLength;
    public float maxLength;

    public static RopeManager instance;
    private Vector2[] positions;
    private bool isPreventNewRope;
    private bool drawStart;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector2[2];
    }

    public void PreventNewRope(bool preventNewRope)
    {
        isPreventNewRope = preventNewRope;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            positions[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (positions[0].y > 0 || positions[0].y < -4.25f || positions[0].x > 2.5f ||
                positions[0].x < -2.5f) return;
            drawStart = true;
            if (!GameController.instance.IsGameStart()) GameController.instance.HideIntroductionInfo();
        }
        else if (Input.GetMouseButton(0))
        {
            positions[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!drawStart)
            {
                if (positions[1].y > 0 || positions[1].y < -4.25f || positions[1].x > 2.5f ||
                    positions[1].x < -2.5f) return;
                positions[0] = positions[1];
                drawStart = true;
                if (!GameController.instance.IsGameStart()) GameController.instance.HideIntroductionInfo();
                return;
            }
            if (positions[1].y > 0)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    positions[0].y / (positions[0].y - positions[1].y));
            }
            else if (positions[1].y < -4.25f)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].y + 4.25f) / (positions[0].y - positions[1].y));
            }
            if (positions[1].x > 2.5f)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x - 2.5f) / (positions[0].x - positions[1].x));
            }
            else if (positions[1].x < -2.5f)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x + 2.5f) / (positions[0].x - positions[1].x));
            }
            var d = Vector2.Distance(positions[0], positions[1]);
            if (d > maxLength)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1], maxLength / d);
                d = maxLength;
            }
            lineRenderer.material.mainTextureScale = new Vector2(d * 4, 1);
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] {positions[0], positions[1]});
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!drawStart) return;
            drawStart = false;
            lineRenderer.positionCount = 0;
            if (isPreventNewRope) return;
            positions[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (positions[1].y > 0)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    positions[0].y / (positions[0].y - positions[1].y));
            }
            else if (positions[1].y < -4.25f)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].y + 4.25f) / (positions[0].y - positions[1].y));
            }
            if (positions[1].x > 2.5f)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x - 2.5f) / (positions[0].x - positions[1].x));
            }
            else if (positions[1].x < -2.5f)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x + 2.5f) / (positions[0].x - positions[1].x));
            }
            var d = Vector2.Distance(positions[0], positions[1]);
            if (d > maxLength)
                positions[1] = Vector2.Lerp(positions[0], positions[1], maxLength / d);
            else if (d < minLength) return;

            pool.FindAll(go => go.activeInHierarchy).ForEach(go => go.GetComponent<RopeScript>().Remove());
            var rope = GetAvailableObject();
            rope.GetComponent<RopeScript>().Initialize(positions[0], positions[1]);
            rope.SetActive(true);
            if (!GameController.instance.IsGameStart()) GameController.instance.GameStart(positions[0], positions[1]);
        }
    }
}
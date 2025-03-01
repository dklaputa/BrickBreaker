using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Rope generator.
/// </summary>
public class RopePoolScript : ObjectPoolBehavior
{
    public static RopePoolScript instance;

    private LineRenderer lineRenderer;

    public float maxLength;
    public float minLength;
    private Vector2[] positions;
    private bool drawStart;
    private bool isPreventNewRope;

    private void Awake()
    {
        instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        positions = new Vector2[2];
    }
    
    private void OnDestroy()
    {
        instance = null;
    }

    // If the ball attaches to a rope, we should not generate a new rope. Otherwise the ball may interact with multiple ropes at the same time.
    public void PreventNewRope(bool preventNewRope)
    {
        isPreventNewRope = preventNewRope;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameControllerScript.instance.IsGameOver() || GameControllerScript.instance.IsShowingTutorial()) return;
        if (Input.GetMouseButtonDown(0))
        {
            positions[0] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // We respond only if the touch point is within the draw area. 
            if (positions[0].y > 0 || positions[0].y < -4.25f || positions[0].x > 2.5f ||
                positions[0].x < -2.5f) return;
            drawStart = true;
            if (!GameControllerScript.instance.IsGameStart()) GameControllerScript.instance.HideIntroductionInfo();
        }
        else if (Input.GetMouseButton(0))
        {
            positions[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!drawStart)
            {
                if (positions[1].y > 0 || positions[1].y < -4.25f || positions[1].x > 2.5f ||
                    positions[1].x < -2.5f) return;
                // The player moves his finger into the draw area so we start the draw mode.
                positions[0] = positions[1];
                drawStart = true;
                if (!GameControllerScript.instance.IsGameStart()) GameControllerScript.instance.HideIntroductionInfo();
                return;
            }

            // The touch point is out of the draw area in the vertical direction. We restrict the rope not to cross the border.
            if (positions[1].y > 0)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    positions[0].y / (positions[0].y - positions[1].y));
            else if (positions[1].y < -4.25f)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].y + 4.25f) / (positions[0].y - positions[1].y));
            // The touch point is out of the draw area in the horizontal direction. We restrict the rope not to cross the border.
            if (positions[1].x > 2.5f)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x - 2.5f) / (positions[0].x - positions[1].x));
            else if (positions[1].x < -2.5f)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x + 2.5f) / (positions[0].x - positions[1].x));
            // We also need to restrict the max length of the rope.
            float d = Vector2.Distance(positions[0], positions[1]);
            if (d > maxLength)
            {
                positions[1] = Vector2.Lerp(positions[0], positions[1], maxLength / d);
                d = maxLength;
            }

            lineRenderer.material.mainTextureScale = new Vector2(d * 5.2f, 1);
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
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    positions[0].y / (positions[0].y - positions[1].y));
            else if (positions[1].y < -4.25f)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].y + 4.25f) / (positions[0].y - positions[1].y));
            if (positions[1].x > 2.5f)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x - 2.5f) / (positions[0].x - positions[1].x));
            else if (positions[1].x < -2.5f)
                positions[1] = Vector2.Lerp(positions[0], positions[1],
                    (positions[0].x + 2.5f) / (positions[0].x - positions[1].x));
            float d = Vector2.Distance(positions[0], positions[1]);
            if (d > maxLength)
                positions[1] = Vector2.Lerp(positions[0], positions[1], maxLength / d);
            else if (d < minLength)
                return;

            pool.FindAll(go => go.activeInHierarchy).ForEach(go => go.GetComponent<RopeScript>().Remove());
            GameObject rope = GetAvailableObject();
            rope.GetComponent<RopeScript>().Initialize(positions[0], positions[1]);
            rope.SetActive(true);
            if (!GameControllerScript.instance.IsGameStart()) GameControllerScript.instance.GameStart(positions[0], positions[1]);
        }
    }
}
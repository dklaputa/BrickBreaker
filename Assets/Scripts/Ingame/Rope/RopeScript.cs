using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    public enum BallTriggerResult
    {
        BallAttach,
        BallDetach,
        BallBounce,
        NotTrigger
    }
    private enum Status
    {
        BeforeTouchBall,
        DuringTouchBall,
        AfterTouchBall
    }

    private enum BallEnterDirection
    {
        UpDown,
        DownUp
    }
    
    private const float BallRange = .175f;
    private const float PerfectRange = .125f;

    public float lengthStall1;
    public float lengthStall2;

    private BallScript ball;
    private EdgeCollider2D edgeCollider2D;
    private GameObject ropeNodeMiddle;
    private Rigidbody2D ropeNodeMiddleRigidBody;
    private SpringJoint2D[] springJointsMiddle;
    private LineRenderer lineRenderer;

    private float ropeLength;
    private Vector2[] endPointPositions;
    private Vector3[] ropePath;
    private Vector2 ropeVector;
    private bool isPerfect;
    private bool isRemoving;
    private BallEnterDirection enterDirection;
    private Status status = Status.BeforeTouchBall;

    public void Initialize(Vector2 position1, Vector2 position2)
    {
        endPointPositions = new[] {position1, position2};
    }

    // Use this for initialization
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ropeNodeMiddle = transform.GetChild(1).gameObject;
        ropeNodeMiddleRigidBody = ropeNodeMiddle.GetComponent<Rigidbody2D>();
        springJointsMiddle = ropeNodeMiddle.GetComponents<SpringJoint2D>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    private void Start()
    {
        ropePath = new Vector3[20];
    }

    private void OnEnable()
    {
        transform.GetChild(0).position = endPointPositions[0];
        transform.GetChild(2).position = endPointPositions[1];
        ropeNodeMiddle.transform.position = (endPointPositions[0] + endPointPositions[1]) / 2;
        ropeVector = endPointPositions[0] - endPointPositions[1];
        ropeLength = ropeVector.magnitude;
        springJointsMiddle[0].distance = ropeLength * .4f;
        springJointsMiddle[1].distance = springJointsMiddle[0].distance;
        edgeCollider2D.points = endPointPositions;
        edgeCollider2D.enabled = true;
        lineRenderer.material.mainTextureScale = new Vector2((int) (ropeLength * 4), 1);
    }

    // Should update after ball updates, so that we can get the accurate ball position.
    private void LateUpdate()
    {
        lineRenderer.positionCount = 20;
        switch (status)
        {
            case Status.BeforeTouchBall:
                RopePathBeforeTouch(endPointPositions[0], endPointPositions[1], ropePath);
                break;
            case Status.DuringTouchBall:
                RopePathDuringTouch(endPointPositions[0], endPointPositions[1], ball.transform.position,
                    BallRange,
                    enterDirection, ropePath);
                break;
            case Status.AfterTouchBall:
                RopePathAfterTouch(endPointPositions[0], ropeNodeMiddleRigidBody.position, endPointPositions[1],
                    ropePath);
                break;
        }

        lineRenderer.SetPositions(ropePath);
    }

    // Rope fade out animation.
    private IEnumerator Fade()
    {
        for (;;)
        {
            Color color1 = lineRenderer.startColor;
            Color color2 = lineRenderer.endColor;
            color1.a -= .12f;
            color2.a -= .12f;
            lineRenderer.startColor = color1;
            lineRenderer.endColor = color2;
            yield return new WaitForSeconds(.03f);
        }
    }

    /// <summary>
    ///     Ball crosses the rope edge.
    /// </summary>
    /// <param name="position">Ball position</param>
    /// <param name="speed">Ball speed</param>
    /// <param name="callback">A callback for the rope</param>
    /// <param name="isNormalCase">Whether we should detect perfect hit and change ball speed level after ball gets detached?</param>
    /// <returns></returns>
    public BallTriggerResult OnBallTrigger(Vector2 position, Vector2 speed, BallScript callback, bool isNormalCase)
    {
        switch (status)
        {
            case Status.DuringTouchBall:
                BallEnterDirection newEnterDirection =
                    Vector3.Cross(speed, ropeVector).z < 0 ? BallEnterDirection.DownUp : BallEnterDirection.UpDown;
                // Ball speed direction is same as the one when the ball attaches to the rope, so we ignore the trigger.
                if (newEnterDirection == enterDirection) return BallTriggerResult.NotTrigger;
                if (isNormalCase)
                {
                    int speedUp;
                    if (ropeLength < lengthStall1) speedUp = 3;
                    else if (ropeLength < lengthStall2)
                        speedUp = 2;
                    else
                        speedUp = 1;
                    if (isPerfect) speedUp += 2;
                    callback.SpeedLevelChange(speedUp);
                }
                else
                {
                    callback.SpeedLevelChange(0);
                }

                ball = null;
                // Start simple harmonic motion.
                ropeNodeMiddle.transform.position = position;
                ropeNodeMiddle.SetActive(true);
                ropeNodeMiddleRigidBody.linearVelocity = speed * .5f;
                status = Status.AfterTouchBall;
                RopePoolScript.instance.PreventNewRope(false);
                Remove();
                return BallTriggerResult.BallDetach;
            case Status.BeforeTouchBall:
                enterDirection = Vector3.Cross(speed, ropeVector).z < 0
                    ? BallEnterDirection.DownUp
                    : BallEnterDirection.UpDown;
                Vector2 accelerationDirection;
                if (enterDirection == BallEnterDirection.UpDown)
                    accelerationDirection = -new Vector2(ropeVector.y, -ropeVector.x).normalized;
                else
                    accelerationDirection = new Vector2(ropeVector.y, -ropeVector.x).normalized;
                callback.SetAccelerationDirection(accelerationDirection);
                if (isNormalCase)
                {
                    Vector2 vLeft = endPointPositions[0] - position;
                    Vector2 vRight = endPointPositions[1] - position;
                    float lLeft = Vector3.Dot(vLeft, ropeVector) / ropeLength;
                    float lRight = -Vector3.Dot(vRight, ropeVector) / ropeLength;
                    if (lLeft < BallRange || lRight < BallRange)
                    {
                        ropeNodeMiddle.SetActive(true);
                        ropeNodeMiddleRigidBody.linearVelocity = speed * .2f;
                        status = Status.AfterTouchBall;
                        Remove();
                        GameControllerScript.instance.Miss();
                        // If the ball closes to one side of the rope, it should not be attached, otherwise problem may occur.
                        return BallTriggerResult.BallBounce;
                    }

                    if (Mathf.Abs(lLeft - ropeLength / 2) < PerfectRange)
                    {
                        isPerfect = true;
                        GameControllerScript.instance.PerfectShoot();
                    }
                    else
                    {
                        GameControllerScript.instance.ResetPerfectCount();
                    }
                }

                ball = callback;
                status = Status.DuringTouchBall;
                RopePoolScript.instance.PreventNewRope(true);
                return BallTriggerResult.BallAttach;
            default:
                return BallTriggerResult.NotTrigger;
        }
    }

    private static void RopePathBeforeTouch(Vector3 pointL, Vector3 pointR, IList<Vector3> ropePath)
    {
        for (int i = 0; i < 20; i++) ropePath[i] = Vector3.Lerp(pointL, pointR, i / 19f);
    }

    private static void RopePathDuringTouch(Vector3 pointL, Vector3 pointR, Vector3 circleCenter, float r,
        BallEnterDirection direction, IList<Vector3> ropePath)
    {
        float dL = (pointL - circleCenter).magnitude;
        float dR = (pointR - circleCenter).magnitude;
        if (r > dL || r > dR)
        {
            Debug.Log("Illegal parameter.");
            // We should avoid this case by detecting the distance of the ball and the rope sides when a ball enters. 
            // If the distance is smaller than r, the ball should be bounced back, rather than attaching to the rope.
            return;
        }

        float fL = Mathf.Acos(r / dL);
        float fR = Mathf.Acos(r / dR);

        Vector3 normalizedL = (pointL - circleCenter).normalized;
        Vector3 normalizedR = (pointR - circleCenter).normalized;

        Vector3[] rotated = new Vector3[2];
        if (direction == BallEnterDirection.DownUp)
        {
            rotated[0] = r * RotateVector(normalizedL, fL);
            rotated[1] = r * RotateVector(normalizedR, -fR);
        }
        else
        {
            rotated[0] = r * RotateVector(normalizedL, -fL);
            rotated[1] = r * RotateVector(normalizedR, fR);
        }

        Vector3[] tangents = {rotated[0] + circleCenter, rotated[1] + circleCenter};
        for (int i = 0; i < 5; i++) ropePath[i] = Vector3.Lerp(pointL, tangents[0], i / 4f);

        for (int i = 5; i < 15; i++) ropePath[i] = Vector3.Slerp(rotated[0], rotated[1], (i - 5) / 9f) + circleCenter;

        for (int i = 15; i < 20; i++) ropePath[i] = Vector3.Lerp(tangents[1], pointR, (i - 10) / 4f);
    }

    private static void RopePathAfterTouch(Vector3 pointL, Vector3 pointM, Vector3 pointR, IList<Vector3> ropePath)
    {
        // Bessel interpolation.
        for (int i = 0; i < 20; i++)
        {
            float t = i / 19f;
            Vector3[] points = {pointL, pointM, pointR};
            for (int m = 2; m > 0; m--)
            for (int n = 0; n < m; n++)
                points[n] = (1 - t) * points[n] + t * points[n + 1];

            ropePath[i] = points[0];
        }
    }

    private static Vector2 RotateVector(Vector2 vector, float angle)
    {
        return new Vector2(vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
            vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle));
    }

    public void Remove()
    {
        if (isRemoving) return;
        isRemoving = true;
        edgeCollider2D.enabled = false;
        StartCoroutine(Fade());
        StartCoroutine(RemoveInternal());
    }

    private IEnumerator RemoveInternal()
    {
        yield return new WaitForSeconds(.25f);
        StopCoroutine(Fade());
        lineRenderer.positionCount = 0;
        Color color1 = lineRenderer.startColor;
        Color color2 = lineRenderer.endColor;
        color1.a = 1;
        color2.a = 1;
        lineRenderer.startColor = color1;
        lineRenderer.endColor = color2;
        status = Status.BeforeTouchBall;
        ropeNodeMiddle.SetActive(false);
        gameObject.SetActive(false);
        isPerfect = false;
        isRemoving = false;
    } 
}
using System.Collections;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
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

    public enum BallTriggerResult
    {
        BallAttach,
        BallDetach,
        BallBounce,
        NotTrigger
    }

    public float lengthStall1;
    public float lengthStall2;

    private const float BallRange = .175f;
    private const float PerfectRange = .125f;

    private Vector2[] endPointPositions;

    private LineRenderer lineRenderer;
    private GameObject ropeNodeMiddle;
    private Rigidbody2D ropeNodeMiddleRigidBody;
    private SpringJoint2D[] springJointsMiddle;
    private EdgeCollider2D edgeCollider2D;

    private Status status = Status.BeforeTouchBall;
    private bool isPerfect;
    private bool isRemoving;
    private BallEnterDirection enterDirection;
    private Vector3[] ropePath;
    private BallScript ball;
    private Vector2 ropeVector;
    private float ropeLength;

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
//        lineRenderer.material.mainTextureScale = new Vector2(ropeLength * 2.5f, 1);
    }

    // Update is called once per frame
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

    private void Update()
    {
        if (!isRemoving) return;
        var color1 = lineRenderer.startColor;
        var color2 = lineRenderer.endColor;
        color1.a -= Time.deltaTime * 4;
        color2.a -= Time.deltaTime * 4;
        lineRenderer.startColor = color1;
        lineRenderer.endColor = color2;
    }

    public BallTriggerResult OnBallTrigger(Vector2 position, Vector2 speed, BallScript callback, bool isNormalCase)
    {
        switch (status)
        {
            case Status.DuringTouchBall:
                var newEnterDirection =
                    Vector3.Cross(speed, ropeVector).z < 0 ? BallEnterDirection.DownUp : BallEnterDirection.UpDown;
                if (newEnterDirection == enterDirection) return BallTriggerResult.NotTrigger;
                if (isNormalCase)
                {
                    int speedUp = 0;
                    if (ropeLength < lengthStall1) speedUp = 2;
                    else if (ropeLength < lengthStall2) speedUp = 1;
                    if (isPerfect) speedUp += 2;
                    callback.SpeedLevelChange(speedUp);
                }
                else callback.SpeedLevelChange(0);
                ball = null;
                ropeNodeMiddle.transform.position = position;
                ropeNodeMiddle.SetActive(true);
                ropeNodeMiddleRigidBody.velocity = speed * .5f;
                status = Status.AfterTouchBall;
                RopeManager.instance.PreventNewRope(false);
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
                    var vLeft = endPointPositions[0] - position;
                    var vRight = endPointPositions[1] - position;
                    var lLeft = Vector3.Dot(vLeft, ropeVector) / ropeLength;
                    var lRight = -Vector3.Dot(vRight, ropeVector) / ropeLength;
                    if (lLeft < BallRange || lRight < BallRange)
                    {
                        ropeNodeMiddle.SetActive(true);
                        ropeNodeMiddleRigidBody.velocity = speed * .2f;
                        status = Status.AfterTouchBall;
                        Remove();
                        GameController.instance.Miss();
                        return BallTriggerResult.BallBounce;
                    }
                    if (Mathf.Abs(lLeft - ropeLength / 2) < PerfectRange)
                    {
                        isPerfect = true;
                        GameController.instance.PerfectShoot();
                    }
                    else GameController.instance.ResetPerfectCount();
                }
                ball = callback;
                status = Status.DuringTouchBall;
                RopeManager.instance.PreventNewRope(true);
                return BallTriggerResult.BallAttach;
            default:
                return BallTriggerResult.NotTrigger;
        }
    }

    private static void RopePathBeforeTouch(Vector3 pointL, Vector3 pointR, Vector3[] ropePath)
    {
        for (var i = 0; i < 20; i++)
        {
            ropePath[i] = Vector3.Lerp(pointL, pointR, i / 19f);
        }
    }

    private static void RopePathDuringTouch(Vector3 pointL, Vector3 pointR, Vector3 circleCenter, float r,
        BallEnterDirection direction, Vector3[] ropePath)
    {
        var dL = (pointL - circleCenter).magnitude;
        var dR = (pointR - circleCenter).magnitude;
        if (r > dL || r > dR)
        {
            Debug.Log("illegal parameter.");
            return;
        }
        var fL = Mathf.Acos(r / dL);
        var fR = Mathf.Acos(r / dR);

        var normalizedL = (pointL - circleCenter).normalized;
        var normalizedR = (pointR - circleCenter).normalized;

        var rotated = new Vector3[2];
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
        for (var i = 0; i < 5; i++)
        {
            ropePath[i] = Vector3.Lerp(pointL, tangents[0], i / 4f);
        }
        for (var i = 5; i < 15; i++)
        {
            ropePath[i] = Vector3.Slerp(rotated[0], rotated[1], (i - 5) / 9f) + circleCenter;
        }
        for (var i = 15; i < 20; i++)
        {
            ropePath[i] = Vector3.Lerp(tangents[1], pointR, (i - 10) / 4f);
        }
    }

    private static void RopePathAfterTouch(Vector3 pointL, Vector3 pointM, Vector3 pointR, Vector3[] ropePath)
    {
        for (var i = 0; i < 20; i++)
        {
            var t = i / 19f;
            Vector3[] points = {pointL, pointM, pointR};
            for (var m = 2; m > 0; m--)
            {
                for (var n = 0; n < m; n++)
                {
                    points[n] = (1 - t) * points[n] + t * points[n + 1];
                }
            }
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
        StartCoroutine("Destroy");
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(.25f);
        lineRenderer.positionCount = 0;
        var color1 = lineRenderer.startColor;
        var color2 = lineRenderer.endColor;
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
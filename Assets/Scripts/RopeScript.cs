﻿using UnityEngine;
using Random = UnityEngine.Random;

public class RopeScript : MonoBehaviour
{
    private enum Status
    {
        BeforeTouchBall,
        DuringTouchBall,
        AfterTouchBall
    }

    private enum BallEnterStatus
    {
        UpDown,
        DownUp,
        NotEnter
    }

    public float lengthStall1 = 2f;
    public float lengthStall2 = 3f;

    private const float endRange = .175f;
//    private const float randomBias = .1f;

    private Vector2[] endPointPositions;
    private LineRenderer lineRenderer;
    private Rigidbody2D ballRigidBody;
    private CircleCollider2D ballCircleCollider;
    private GameObject ropeNodeLeft;
    private GameObject ropeNodeMiddle;
    private GameObject ropeNodeRight;
    private Rigidbody2D ropeNodeMiddleRigidBody;
    private SpringJoint2D[] springJointsMiddle;

    private Status status = Status.BeforeTouchBall;
    private bool isRemovable = true;
    private bool isRemoving;
    private Vector2 AccelerationDirection;
    private BallEnterStatus enterDirection;
    private Vector3[] ropePath = new Vector3[30];

    public void SetEndPointPositions(Vector2 position1, Vector2 position2)
    {
        endPointPositions = new[] {position1, position2};
    }

    // Use this for initialization
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.sortingLayerName = "Rope";
        ropeNodeLeft = transform.GetChild(0).gameObject;
        ropeNodeMiddle = transform.GetChild(1).gameObject;
        ropeNodeRight = transform.GetChild(2).gameObject;
        ropeNodeMiddleRigidBody = ropeNodeMiddle.GetComponent<Rigidbody2D>();
        springJointsMiddle = ropeNodeMiddle.GetComponents<SpringJoint2D>();
    }

    private void Start()
    {
        ballRigidBody = BallScript.instance.GetComponent<Rigidbody2D>();
        ballCircleCollider = BallScript.instance.GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        ropeNodeLeft.transform.position = endPointPositions[0];
        ropeNodeRight.transform.position = endPointPositions[1];
        ropeNodeMiddle.transform.position = (endPointPositions[0] + endPointPositions[1]) / 2;
        var ropeLength = (endPointPositions[0] - endPointPositions[1]).magnitude;
        springJointsMiddle[0].distance = ropeLength * .4f;
        springJointsMiddle[1].distance = springJointsMiddle[0].distance;
    }

    // Update is called once per frame
    private void Update()
    {
        lineRenderer.positionCount = 30;
        switch (status)
        {
            case Status.BeforeTouchBall:
                RopePathBeforeTouch(endPointPositions[0], endPointPositions[1], ropePath);
                break;
            case Status.DuringTouchBall:
                RopePathDuringTouch(endPointPositions[0], endPointPositions[1], ballRigidBody.position, .175f,
                    enterDirection, ropePath);
                break;
            case Status.AfterTouchBall:
                RopePathAfterTouch(endPointPositions[0], ropeNodeMiddle.transform.position, endPointPositions[1],
                    ropePath);
                break;
        }
        lineRenderer.SetPositions(ropePath);
        if (!isRemoving) return;
        var color1 = lineRenderer.startColor;
        var color2 = lineRenderer.endColor;
        color1.a -= Time.deltaTime * 4;
        color2.a -= Time.deltaTime * 4;
        lineRenderer.startColor = color1;
        lineRenderer.endColor = color2;
    }

    private void FixedUpdate()
    {
        if (isRemoving) return;
        if (status == Status.DuringTouchBall)
        {
            var acceleration = AccelerationDirection * (30 + 15 * ballRigidBody.velocity.magnitude);
            var ballStatus = CheckTrigger(endPointPositions[0], endPointPositions[1], ballRigidBody.position,
                (ballRigidBody.velocity + acceleration * Time.deltaTime) * Time.deltaTime);
            if (ballStatus == BallEnterStatus.NotEnter)
                ballRigidBody.velocity += acceleration * Time.deltaTime;
            else if (ballStatus != enterDirection)
            {
                var ropeLength = (endPointPositions[0] - endPointPositions[1]).magnitude;
                if (ropeLength < lengthStall1)
                    BallScript.instance.SpeedUp();
                else if (ropeLength < lengthStall2)
                    BallScript.instance.RefreshBallSpeed();
                else
                    BallScript.instance.SpeedDown();
                ropeNodeMiddle.transform.position = ballRigidBody.position;
                ropeNodeMiddle.SetActive(true);
                ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity * .5f;
                ballCircleCollider.enabled = true;
                status = Status.AfterTouchBall;
                isRemovable = true;
                Remove();
            }
        }
        else if (status == Status.BeforeTouchBall)
        {
            var ballStatus = CheckTrigger(endPointPositions[0], endPointPositions[1], ballRigidBody.position,
                ballRigidBody.velocity * Time.deltaTime);
            if (ballStatus == BallEnterStatus.NotEnter) return;

            var vAB = endPointPositions[0] - endPointPositions[1];

            if (ballStatus == BallEnterStatus.UpDown)
                AccelerationDirection = -new Vector2(vAB.y, -vAB.x).normalized;
            else
                AccelerationDirection = new Vector2(vAB.y, -vAB.x).normalized;
            enterDirection = ballStatus;
            ballRigidBody.velocity = -AccelerationDirection * ballRigidBody.velocity.magnitude;

            var vAC = endPointPositions[0] - ballRigidBody.position;
            var vBC = endPointPositions[1] - ballRigidBody.position;
            if (Mathf.Abs(Vector3.Cross(vAC, ballRigidBody.velocity).z / ballRigidBody.velocity.magnitude) < endRange ||
                Mathf.Abs(Vector3.Cross(vBC, ballRigidBody.velocity).z / ballRigidBody.velocity.magnitude) < endRange)
            {
                ballRigidBody.velocity = -ballRigidBody.velocity;
                ropeNodeMiddle.SetActive(true);
                ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity * .2f;
                status = Status.AfterTouchBall;
                Remove();
                return;
            }
            ballCircleCollider.enabled = false;
            status = Status.DuringTouchBall;
            isRemovable = false;
        }
    }

    private static BallEnterStatus CheckTrigger(Vector2 ropeNodeLeftPosition, Vector2 ropeNodeRightPosition,
        Vector2 ballPosition, Vector2 ballDisplacement)
    {
        var pointD = ballPosition + ballDisplacement;
        var vAB = ropeNodeRightPosition - ropeNodeLeftPosition;
        var vAC = ballPosition - ropeNodeLeftPosition;
        var vAD = pointD - ropeNodeLeftPosition;
        var vBC = ballPosition - ropeNodeRightPosition;
        var sinCAB = Vector3.Cross(vAC, vAB).z;
        if (!(sinCAB * Vector3.Cross(vAD, vAB).z < 0) ||
            !(Vector3.Cross(vAC, ballDisplacement).z * Vector3.Cross(vBC, ballDisplacement).z < 0))
            return BallEnterStatus.NotEnter;
        return sinCAB > 0 ? BallEnterStatus.UpDown : BallEnterStatus.DownUp;
    }

    private static void RopePathBeforeTouch(Vector3 pointL, Vector3 pointR, Vector3[] ropePath)
    {
        for (var i = 0; i < 30; i++)
        {
            ropePath[i] = Vector3.Lerp(pointL, pointR, i / 29f);
        }
    }

    private static void RopePathDuringTouch(Vector3 pointL, Vector3 pointR, Vector3 circleCenter, float r,
        BallEnterStatus direction, Vector3[] ropePath)
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
        if (direction == BallEnterStatus.DownUp)
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
        for (var i = 0; i < 10; i++)
        {
            ropePath[i] = Vector3.Lerp(pointL, tangents[0], i / 9f);
        }
        for (var i = 10; i < 20; i++)
        {
            ropePath[i] = Vector3.Slerp(rotated[0], rotated[1], (i - 10) / 9f) + circleCenter;
        }
        for (var i = 20; i < 30; i++)
        {
            ropePath[i] = Vector3.Lerp(tangents[1], pointR, (i - 20) / 9f);
        }
    }

    private static void RopePathAfterTouch(Vector3 pointL, Vector3 pointM, Vector3 pointR, Vector3[] ropePath)
    {
        for (var i = 0; i < 30; i++)
        {
            var t = i / 29f;
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
        if (!isRemovable || isRemoving) return;
        isRemoving = true;
        Invoke("Destroy", .25f);
    }

    private void Destroy()
    {
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
        isRemoving = false;
    }
}
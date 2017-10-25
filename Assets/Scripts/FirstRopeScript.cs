using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRopeScript : RopeScript
{
    public void Initialize(Vector2 position1, Vector2 position2)
    {
        endPointPositions = new[] {position1, position2};
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        status = Status.DuringTouchBall;
        BallScript.instance.transform.position = (endPointPositions[0] + endPointPositions[1]) / 2;
        var vAB = endPointPositions[0] - endPointPositions[1];
        if (vAB.x > 0)
        {
            AccelerationDirection = new Vector2(-vAB.y, vAB.x).normalized;
            enterDirection = BallEnterStatus.UpDown;
        }
        else
        {
            AccelerationDirection = new Vector2(vAB.y, -vAB.x).normalized;
            enterDirection = BallEnterStatus.DownUp;
        }
        ballRigidBody.velocity = -AccelerationDirection * BallScript.instance.speed[0];
        BallScript.instance.GetComponent<SpriteRenderer>().enabled = true;
        ballCircleCollider.enabled = false;
    }

    protected override void FixedUpdate()
    {
        if (isRemoving || status == Status.AfterTouchBall) return;
        var acceleration = AccelerationDirection * (30 + 15 * ballRigidBody.velocity.magnitude);
        var ballStatus = CheckTrigger(endPointPositions[0], endPointPositions[1], ballRigidBody.position,
            (ballRigidBody.velocity + acceleration * Time.deltaTime) * Time.deltaTime);
        if (ballStatus == BallEnterStatus.NotEnter)
            ballRigidBody.velocity += acceleration * Time.deltaTime;
        else if (ballStatus != enterDirection)
        { 
            BallScript.instance.SpeedLevelChange(0);
            ropeNodeMiddle.transform.position = ballRigidBody.position;
            ropeNodeMiddle.SetActive(true);
            ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity * .5f;
            ballCircleCollider.enabled = true;
            status = Status.AfterTouchBall;
            Remove();
        }
    }
}
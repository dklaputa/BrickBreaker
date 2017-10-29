using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRopeScript : RopeScript
{
//    public void Initialize(Vector2 position1, Vector2 position2)
//    {
//        endPointPositions = new[] {position1, position2};
//    }
//
//    protected override void OnEnable()
//    {
//        base.OnEnable();
//        status = Status.DuringTouchBall;
//        BallScript.instance.transform.position = (endPointPositions[0] + endPointPositions[1]) / 2;
//        var vAB = endPointPositions[0] - endPointPositions[1];
//        if (vAB.x > 0)
//        {
//            AccelerationDirection = new Vector2(-vAB.y, vAB.x).normalized;
//            enterDirection = BallEnterStatus.UpDown;
//        }
//        else
//        {
//            AccelerationDirection = new Vector2(vAB.y, -vAB.x).normalized;
//            enterDirection = BallEnterStatus.DownUp;
//        }
//        BallScript.instance.speed = -AccelerationDirection * BallScript.instance.speedArray[0];
//        BallScript.instance.GetComponent<SpriteRenderer>().enabled = true;
//        BallScript.instance.isAttachedToRope = true;
//    }
//
//    protected override void FixedUpdate()
//    {
//        if (isRemoving || status == Status.AfterTouchBall) return;
//        var acceleration = AccelerationDirection * (30 + 15 * BallScript.instance.speed.magnitude);
//        var ballStatus = CheckTrigger(endPointPositions[0], endPointPositions[1], BallScript.instance.transform.position,
//            (BallScript.instance.speed + acceleration * Time.deltaTime) * Time.deltaTime);
//        if (ballStatus == BallEnterStatus.NotEnter)
//            BallScript.instance.speed += acceleration * Time.deltaTime;
//        else if (ballStatus != enterDirection)
//        { 
//            BallScript.instance.SpeedLevelChange(0);
//            ropeNodeMiddle.transform.position = BallScript.instance.transform.position;
//            ropeNodeMiddle.SetActive(true);
//            ropeNodeMiddleRigidBody.velocity = BallScript.instance.speed * .5f;
//            BallScript.instance.isAttachedToRope = false;
//            status = Status.AfterTouchBall;
//            Remove();
//        }
//    }
}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{
    public float[] speedArray = {8, 9, 10, 11, 12, 13, 14, 15, 16};

    private SpriteRenderer spriteRenderer;
    private Vector2 speed;
    private int speedLvl;
    private int scoreTimes;
    private bool isAttachedToRope;
    private Vector2 accelerationDirection;

    private static readonly Color Blue = new Color(57f / 255, 196f / 255, 215f / 255);
    private static readonly Color Yellow = new Color(215f / 255, 165f / 255, 57f / 255);

    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        speed = Vector2.down * speedArray[speedLvl];
    }

//    // Update is called once per frame
//    private void Update()
//    {
//        if (Mathf.Abs(transform.position.x) > 4 || Mathf.Abs(transform.position.y) > 6)
//            SceneManager.LoadScene("Main");
//    }

    private void Update()
    {
        if (isAttachedToRope)
        {
            speed += accelerationDirection * (20 + 10 * speed.magnitude) * Time.deltaTime;
        }
        var remainTime = Time.deltaTime;
        var hit = Physics2D.CircleCast(transform.position, .125f, speed,
            speed.magnitude * remainTime);
        while (hit.collider != null)
        {
            var o = hit.collider.gameObject;
            if (o.CompareTag("Rope"))
            {
                var rope = o.GetComponent<RopeScript>();
                var result = rope.OnBallTrigger(transform.position, speed, this);
                if (result == RopeScript.BallTriggerResult.BallBounce)
                {
                    speed = -speed;
                }
                else if (result == RopeScript.BallTriggerResult.BallAttach)
                {
                    speed = -accelerationDirection * speed.magnitude;
                    isAttachedToRope = true;
                }
                else if (result == RopeScript.BallTriggerResult.BallDetach)
                {
                    isAttachedToRope = false;
                }
                break;
            }
            if (isAttachedToRope) break;
            if (o.CompareTag("Wall"))
            {
                remainTime -= hit.distance / speed.magnitude;
                transform.position = hit.point + hit.normal * .125f;
                speed = Vector2.Reflect(speed, hit.normal);
            }
            else if (o.CompareTag("Brick"))
            {
                remainTime -= hit.distance / speed.magnitude;
                transform.position = hit.point + hit.normal * .125f;
                var brick = o.GetComponent<BrickScript>();
                if (speedLvl >= brick.level)
                {
                    SpeedLevelChange(-brick.level - 1);
                    Destroy(o);
                }
                if (speedLvl <= brick.level)
                {
                    speed = Vector2.Reflect(speed, hit.normal);
                }
            }
            else break;
            hit = Physics2D.CircleCast(transform.position, .125f, speed, speed.magnitude * remainTime);
        }
        transform.position += (Vector3) speed * remainTime;
    }

    public void SetAccelerationDirection(Vector2 direction)
    {
        accelerationDirection = direction;
    }

    public void SpeedLevelChange(int lvlChange)
    {
        speedLvl = Mathf.Clamp(speedLvl + lvlChange, 0, speedArray.Length - 1);
        OnSpeedLevelChange();
    }

    public void SetScoreTimes(int times)
    {
        scoreTimes = times;
    }
//    public void SpeedLevelToZero()
//    {
//        speedLvl = 0;
//        RefreshBallSpeed();
//    }

    private void OnSpeedLevelChange()
    {
        speed = speed.normalized * speedArray[speedLvl];
        spriteRenderer.color = Color.Lerp(Blue, Yellow, (float) speedLvl / (speedArray.Length - 1));
    }
}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{
    public float[] speedArray = {8, 9, 10, 11, 12, 13, 14, 15, 16};

    private SpriteRenderer spriteRenderer;
    private ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule;
    private ParticleSystem particle;
    private Vector2 speed;
    private int speedLvl;
    private int scoreTimes;
    private bool isAttachedToRope;
    private Vector2 accelerationDirection;
    private bool isBeforeStart = true;
    private bool isRestoreTimeScale;

    public Color Blue = new Color(57f / 255, 196f / 255, 215f / 255);
    public Color Yellow = new Color(215f / 255, 165f / 255, 57f / 255);
    public Color Red = new Color(215f / 255, 57f / 255, 80f / 255);

    public void setInitialSpeedDirection(Vector2 direction)
    {
        speed = direction * speedArray[speedLvl];
    }

    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particle = GetComponent<ParticleSystem>();
        colorOverLifetimeModule = particle.colorOverLifetime;
    }

//    private void Start() 
//    {
//        speed = Vector2.down * speedArray[speedLvl];
//    }

//    // Update is called once per frame
//    private void Update()
//    {
//        if (Mathf.Abs(transform.position.x) > 4 || Mathf.Abs(transform.position.y) > 6)
//            SceneManager.LoadScene("Main");
//    }

    private void Update()
    {
        if (isRestoreTimeScale)
        {
            Time.timeScale += 2 * Time.deltaTime;
            if (Time.timeScale >= 1)
            {
                isRestoreTimeScale = false;
                Time.timeScale = 1;
            }
        }
        if (isAttachedToRope)
        {
            speed += accelerationDirection * (20 + 10 * speed.magnitude) * Time.deltaTime;
        }
        var remainTime = Time.deltaTime;
        var hit = Physics2D.CircleCast(transform.position, .125f, speed,
            speed.magnitude * remainTime);
        var count = 0;
        while (hit.collider != null && count < 5)
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
                    if (isBeforeStart) isBeforeStart = false;
                }
                else if (result == RopeScript.BallTriggerResult.BallDetach)
                {
                    isAttachedToRope = false;
                }
                break;
            }
            if (isAttachedToRope || isBeforeStart) break;
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
                if (speedLvl <= brick.level)
                {
                    speed = Vector2.Reflect(speed, hit.normal);
                }
                if (speedLvl >= brick.level)
                {
                    if (speedLvl > 3) SlowDownTimeScale();
                    SpeedLevelChange(-brick.level - 1);
                    brick.Remove();
                    BrickGenerator.instance.CheckIsBrickAllDead();
                }
            }
            else if (o.CompareTag("GameOverTrigger"))
            {
                SceneManager.LoadScene("Main");
                break;
            }
            else break;
            hit = Physics2D.CircleCast(transform.position, .125f, speed, speed.magnitude * remainTime);
            count++;
        }
        transform.position += (Vector3) speed * remainTime;
    }

    public void SetAccelerationDirection(Vector2 direction)
    {
        accelerationDirection = direction;
    }

    public void SlowDownTimeScale()
    {
        CancelInvoke();
        isRestoreTimeScale = false;
        Time.timeScale = .1f;
        Invoke("RestoreTimeScale", .1f);
    }

    public void RestoreTimeScale()
    {
        isRestoreTimeScale = true;
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
        Color color;
        var half = speedArray.Length / 2;
        if (speedLvl < half)
        {
            color = Color.Lerp(Blue, Yellow, (float) speedLvl / half);
        }
        else
        {
            color = Color.Lerp(Yellow, Red,
                (float) (speedLvl - half) / (speedArray.Length - half - 1));
        }
        spriteRenderer.color = color;
        if (speedLvl > 3)
        {
            if (!particle.isPlaying) particle.Play();
            colorOverLifetimeModule.color = color;
        }
        else
        {
            if (!particle.isStopped) particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
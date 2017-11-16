using System.Collections;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private static readonly float[] SpeedArray = {8, 9, 10, 11, 12, 13, 14, 15, 16};
    private static readonly Color Blue = new Color(57f / 255, 196f / 255, 215f / 255);
    private static readonly Color Yellow = new Color(255f / 255, 197f / 255, 71f / 255);
    private static readonly Color Red = new Color(255f / 255, 73f / 255, 122f / 255);

    private SpriteRenderer spriteRenderer;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem particle;
    private Vector2 speed;
    private Animator animator;
    private int speedLvl;
    private bool isAttachedToRope;
    private Vector2 accelerationDirection;
    private bool isBeforeStart = true;

    public void SetInitialSpeedDirection(Vector2 direction)
    {
        speed = direction * SpeedArray[speedLvl];
    }

    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particle = GetComponent<ParticleSystem>();
        mainModule = particle.main;
        animator = GetComponent<Animator>();
    }

//    private void Start() 
//    {
//        speed = Vector2.down * speedArray[speedLvl];
//    } 

    private void Update()
    {
        var remainTime = Time.deltaTime;
        if (isAttachedToRope)
        {
            speed += accelerationDirection * (20 + 10 * speed.magnitude) * Time.deltaTime;
            var hit = Physics2D.CircleCast(transform.position, .125f, speed, speed.magnitude * Time.deltaTime,
                LayerMask.GetMask("Rope"));
            if (hit.collider != null)
            {
                var rope = hit.collider.gameObject.GetComponent<RopeScript>();
                var result = rope.OnBallTrigger(transform.position, speed, this, !isBeforeStart);
                if (result == RopeScript.BallTriggerResult.BallDetach)
                {
                    isAttachedToRope = false;
                    if (isBeforeStart) isBeforeStart = false;
                    var divisionItem = ItemManager.instance.CheckItemLevel(ItemManager.Division);
                    CloneBallManager.instance.ShowCloneBalls(transform.position, speed, speedLvl, divisionItem);
                }
            }
        }
        else if (isBeforeStart)
        {
            var hit = Physics2D.CircleCast(transform.position, .125f, speed, speed.magnitude * Time.deltaTime,
                LayerMask.GetMask("Rope"));
            if (hit.collider != null)
            {
                var rope = hit.collider.gameObject.GetComponent<RopeScript>();
                rope.OnBallTrigger(transform.position, speed, this, !isBeforeStart);
                isAttachedToRope = true;
            }
        }
        else
        {
            var hit = Physics2D.CircleCast(transform.position, .125f, speed,
                speed.magnitude * remainTime);
            var count = 0;
            while (hit.collider != null && count < 5)
            {
                var o = hit.collider.gameObject;
                if (o.CompareTag("Rope"))
                {
                    var rope = o.GetComponent<RopeScript>();
                    var result = rope.OnBallTrigger(transform.position, speed, this, !isBeforeStart);
                    if (result == RopeScript.BallTriggerResult.BallBounce)
                    {
                        speed = -speed;
                    }
                    else if (result == RopeScript.BallTriggerResult.BallAttach)
                    {
                        speed = -accelerationDirection * speed.magnitude;
                        isAttachedToRope = true;
                        GameController.instance.ResetComboCount();
                    }
                    break;
                }
                if (o.CompareTag("Wall"))
                {
                    remainTime -= hit.distance / speed.magnitude;
                    transform.position = hit.point + hit.normal * .125f;
                    speed = Vector2.Reflect(speed, hit.normal);
                    var blackHoleItem = ItemManager.instance.CheckItemLevel(ItemManager.BlackHole);
                    if (blackHoleItem > 0)
                        BlackHoleManager.instance.ShowBlackHole(transform.position, .1f + blackHoleItem * .1f);
                }
                else if (o.CompareTag("Brick"))
                {
                    remainTime -= hit.distance / speed.magnitude;
                    transform.position = hit.point + hit.normal * .125f;
                    var brick = o.GetComponent<BrickScript>();
                    if (speedLvl <= brick.level)
                    {
                        speed = Vector2.Reflect(speed, hit.normal);
                        var blackHoleItem = ItemManager.instance.CheckItemLevel(ItemManager.BlackHole);
                        if (blackHoleItem > 0)
                            BlackHoleManager.instance.ShowBlackHole(transform.position, .1f + blackHoleItem * .1f);
                    }
                    if (speedLvl >= brick.level)
                    {
                        if (speedLvl > 3) GameController.instance.SlowDownTimeScale();
                        SpeedLevelChange(-brick.level - 1);
                        var score = brick.Break();
                        if (score > 0) PointsTextManager.instance.ShowPointsText(brick.transform.position, score);
                    }
                }
                else if (o.CompareTag("GameOverTrigger") && speed.y < 0)
                {
                    if (!GameController.instance.IsGameOver())
                        GameController.instance.GameOver();
                    animator.SetTrigger("Die");
                    StartCoroutine("Destroy");
                    break;
                }
                hit = Physics2D.CircleCast(transform.position, .125f, speed, speed.magnitude * remainTime);
                count++;
            }
        }
        transform.position += (Vector3) speed * remainTime;
    }

    public void SetAccelerationDirection(Vector2 direction)
    {
        accelerationDirection = direction;
    }

    public void SpeedLevelChange(int lvlChange)
    {
        speedLvl = Mathf.Clamp(speedLvl + lvlChange, 0, SpeedArray.Length - 1);
        OnSpeedLevelChange();
    }

    private void OnSpeedLevelChange()
    {
        speed = speed.normalized * SpeedArray[speedLvl];
        Color color;
        var half = SpeedArray.Length / 2;
        if (speedLvl < half)
        {
            color = Color.Lerp(Blue, Yellow, (float) speedLvl / half);
        }
        else
        {
            color = Color.Lerp(Yellow, Red,
                (float) (speedLvl - half) / (SpeedArray.Length - half - 1));
        }
        spriteRenderer.color = color;
        if (speedLvl > 3)
        {
            mainModule.startColor = color;
            if (!particle.isPlaying) particle.Play();
        }
        else
        {
            if (!particle.isStopped) particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        GameController.instance.SetBallLevel(speedLvl, SpeedArray.Length, color);
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(.1f);
        Destroy(this);
    }
}
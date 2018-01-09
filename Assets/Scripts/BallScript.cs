using System.Collections;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    private const float BallSize = .125f;
    private static readonly float[] SpeedArray = {8, 9, 10, 11, 12, 12.5f, 13, 13.5f, 14};
    private static readonly Color Blue = new Color(57f / 255, 196f / 255, 215f / 255);
    private static readonly Color Yellow = new Color(255f / 255, 197f / 255, 71f / 255);
    private static readonly Color Red = new Color(255f / 255, 73f / 255, 122f / 255);

    private Vector2 accelerationDirection;
    private Vector2 speed;
    private int speedLvl;

    // Whether the game really started.
    // We say a game really starts after the ball is detached from the rope for the first time.
    // Before that, we should ignore all the collisions of the ball (with walls and bricks)
    private bool isBeforeStart = true;
    private bool isAttachedToRope;

    private Animator animator;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem particle;
    private SpriteRenderer spriteRenderer;

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

    // Refresh ball position
    private void Update()
    {
        float remainTime = Time.deltaTime;
        if (isAttachedToRope)
        {
            // Acceleration increases as speed increases.
            speed += accelerationDirection * (20 + 10 * speed.magnitude) * Time.deltaTime;
            // Detect whether the ball is detached from rope.
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, BallSize, speed,
                speed.magnitude * Time.deltaTime,
                LayerMask.GetMask("Rope"));
            if (hit.collider != null)
            {
                RopeScript rope = hit.collider.gameObject.GetComponent<RopeScript>();
                RopeScript.BallTriggerResult result =
                    rope.OnBallTrigger(transform.position, speed, this, !isBeforeStart);
                if (result == RopeScript.BallTriggerResult.BallDetach)
                {
                    isAttachedToRope = false;
                    if (isBeforeStart) isBeforeStart = false; // Game really starts.
                    // Check whether the item Division is playing effects.
                    int divisionLevel = ItemManager.instance.CheckItemActivated(ItemManager.Division);
                    if (divisionLevel >= 0)
                        CloneBallManager.instance.ShowCloneBalls(transform.position, speed, speedLvl, divisionLevel);
                }
            }
        }
        else if (isBeforeStart)
        {
            // Game does not really start. We should ignore all collisions but that with rope.
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, BallSize, speed,
                speed.magnitude * Time.deltaTime,
                LayerMask.GetMask("Rope"));
            if (hit.collider != null)
            {
                RopeScript rope = hit.collider.gameObject.GetComponent<RopeScript>();
                rope.OnBallTrigger(transform.position, speed, this, !isBeforeStart);
                isAttachedToRope = true;
            }
        }
        else
        {
            // Check if there are collisions with rope, walls, and bricks.
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, BallSize, speed,
                speed.magnitude * remainTime);
            int count = 0; // We check collisions iteratively. The max iterations num is 5. 
            while (hit.collider != null && count < 5)
            {
                GameObject o = hit.collider.gameObject;
                if (o.CompareTag("Rope"))
                {
                    RopeScript rope = o.GetComponent<RopeScript>();
                    // Ball reaction determined by the rope.
                    RopeScript.BallTriggerResult result =
                        rope.OnBallTrigger(transform.position, speed, this, !isBeforeStart);
                    if (result == RopeScript.BallTriggerResult.BallBounce)
                    {
                        // Ball hits one of the two sides of the rope, the ball should bounce back.
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
                    transform.position = hit.point + hit.normal * BallSize;
                    speed = Vector2.Reflect(speed, hit.normal);
                    // Check whether the item BlackHole is playing effects.
                    int blackHoleLevel = ItemManager.instance.CheckItemActivated(ItemManager.BlackHole);
                    if (blackHoleLevel >= 0)
                        BlackHoleManager.instance.ShowBlackHole(transform.position, blackHoleLevel);
                }
                else if (o.CompareTag("Brick"))
                {
                    remainTime -= hit.distance / speed.magnitude;
                    transform.position = hit.point + hit.normal * BallSize;
                    BrickScript brick = o.GetComponent<BrickScript>();
                    int brickLevel =
                        brick.level == -1 ? 0 : brick.level; //Item brick can be treated as a level 0 brick.
                    // If speed level is not larger than the brick level, the ball should reflect.
                    if (speedLvl <= brickLevel)
                    {
                        speed = Vector2.Reflect(speed, hit.normal);
                        int blackHoleLevel = ItemManager.instance.CheckItemActivated(ItemManager.BlackHole);
                        if (blackHoleLevel >= 0)
                            BlackHoleManager.instance.ShowBlackHole(transform.position, blackHoleLevel);
                    }

                    // If speed level is not smaller than the brick level, the brick should be destroyed.
                    if (speedLvl >= brickLevel)
                    {
                        if (speedLvl > 3) GameController.instance.SlowDownTimeScale();
                        SpeedLevelChange(-brickLevel - 1); // Speed level decreases.
                        int points = brick.Break();
                        if (points > 0)
                            PointsTextManager.instance.ShowPointsText(brick.transform.position,
                                GameController.instance.AddPointsConsiderCombo(points));
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
                else
                {
                    break;
                }

                hit = Physics2D.CircleCast(transform.position, BallSize, speed, speed.magnitude * remainTime);
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

    // Behaviors on ball speed level changes: 1. Change ball speed. 2. Change ball color. 3. Change spark color if exists.
    private void OnSpeedLevelChange()
    {
        speed = speed.normalized * SpeedArray[speedLvl];
        Color color;
        int half = SpeedArray.Length / 2;
        if (speedLvl < half)
            color = Color.Lerp(Blue, Yellow, (float) speedLvl / half);
        else
            color = Color.Lerp(Yellow, Red,
                (float) (speedLvl - half) / (SpeedArray.Length - half - 1));

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
        yield return new WaitForSeconds(.1f); // Wait for 0.1 second for the destroy animation.
        Destroy(gameObject);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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
    private int speedLvl;
    private bool isAttachedToRope;
    private Vector2 accelerationDirection;
    private bool isBeforeStart = true;
    private bool isRestoreTimeScale;

    private GameController.Item currentItem;

    public void setInitialSpeedDirection(Vector2 direction)
    {
        speed = direction * SpeedArray[speedLvl];
    }

    // Use this for initialization
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particle = GetComponent<ParticleSystem>();
        mainModule = particle.main;
    }

//    private void Start() 
//    {
//        speed = Vector2.down * speedArray[speedLvl];
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
                else if (result == RopeScript.BallTriggerResult.BallDetach)
                {
                    isAttachedToRope = false;
                    if (isBeforeStart) isBeforeStart = false;
//                    if (currentItem == GameController.Item.Division)
                    CloneBallManager.instance.ShowCloneBalls(transform.position, speed, speedLvl);
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
                    brick.Break(this);
                }
            }
            else if (o.CompareTag("GameOverTrigger") && speed.y < 0)
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

    public void SetCurrentItem(GameController.Item item)
    {
        currentItem = item;
    }

    private void SlowDownTimeScale()
    {
        CancelInvoke();
        isRestoreTimeScale = false;
        Time.timeScale = .1f;
        Invoke("RestoreTimeScale", .1f);
    }

    private void RestoreTimeScale()
    {
        isRestoreTimeScale = true;
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
}
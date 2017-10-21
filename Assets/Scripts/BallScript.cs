using UnityEngine;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{
    public static BallScript instance;
    private Rigidbody2D rigid2D;
    private SpriteRenderer spriteRenderer;

    private float[] speed = {8, 10, 12, 14, 16};
    public int speedLvl;
    private Color blue = new Color(57f / 255, 196f / 255, 215f / 255);
    private Color yellow = new Color(215f / 255, 165f / 255, 57f / 255);

    // Use this for initialization
    private void Awake()
    {
        instance = this;
        rigid2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        rigid2D.velocity = -transform.up * speed[speedLvl];
    }

    // Update is called once per frame
    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > 4 || Mathf.Abs(transform.position.y) > 6)
            SceneManager.LoadScene("Main");
    }

    public void SpeedUp()
    {
        if (speedLvl < speed.Length - 1)
        {
            speedLvl++;
        }
        RefreshBallSpeed();
    }

    public void SpeedDown()
    {
        speedLvl = 0;
        RefreshBallSpeed();
    }

    public void RefreshBallSpeed()
    {
        rigid2D.velocity = rigid2D.velocity.normalized * speed[speedLvl];
        spriteRenderer.color = Color.Lerp(blue, yellow, (float) speedLvl / (speed.Length - 1));
    }

    public bool HitBrick(int brickLevel)
    {
        bool result = true;
        if (speedLvl > brickLevel) speedLvl = speedLvl - brickLevel - 1;
        else if (speedLvl == brickLevel) speedLvl = 0;
        else result = false;
        RefreshBallSpeed();
        return result;
    }
}
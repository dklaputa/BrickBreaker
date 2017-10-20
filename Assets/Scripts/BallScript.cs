using UnityEngine;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{
    public static BallScript instance;
    private Rigidbody2D rigid2D;
    private SpriteRenderer spriteRenderer;

    private float[] speed = {10, 11, 12, 13, 14, 15, 16};
    private int speedLvl;
    private Color blue = new Color(17f / 255, 196f / 255, 255f / 255);
    private Color yellow = new Color(255f / 255, 196f / 255, 17f / 255);

    // Use this for initialization
    private void Start()
    {
        instance = this;
        rigid2D = GetComponent<Rigidbody2D>();
        rigid2D.velocity = -transform.up * speed[speedLvl];
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            rigid2D.velocity = rigid2D.velocity.normalized * speed[speedLvl];
        }
        else
            KeepSpeed();
        spriteRenderer.color = Color.Lerp(blue, yellow, (float) speedLvl / (speed.Length - 1));
    }

    public void SpeedDown()
    {
        speedLvl = 0;
        rigid2D.velocity = rigid2D.velocity.normalized * speed[speedLvl];
        spriteRenderer.color = blue;
    }

    public void KeepSpeed()
    {
        rigid2D.velocity = rigid2D.velocity.normalized * speed[speedLvl];
    }
}
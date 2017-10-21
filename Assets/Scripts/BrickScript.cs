using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    public int level;

    private Rigidbody2D ballRigid2D;

    private Color[] colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 152f / 255, 135f / 255),
        new Color(255f / 255, 135f / 255, 172f / 255)
    };

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[level];
        ballRigid2D = BallScript.instance.GetComponent<Rigidbody2D>();
        GetComponent<CapsuleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            if (BallScript.instance.speedLvl <= level)
            {
                ballRigid2D.velocity = Vector3.Reflect(ballRigid2D.velocity, other.contacts[0].normal);
            }
            if (BallScript.instance.HitBrick(level)) Destroy(gameObject);
        }
    }
}
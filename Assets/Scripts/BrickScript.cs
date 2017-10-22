using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickScript : MonoBehaviour
{
    public int level;

    public Color[] colors =
    {
        new Color(255f / 255, 207f / 255, 135f / 255), new Color(255f / 255, 152f / 255, 135f / 255),
        new Color(214f / 255, 135f / 255, 255f / 255)
    };

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[level]; 
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            if (BallScript.instance.HitBrick(level))
            {
                Destroy(gameObject);
            }
        }
    }
}
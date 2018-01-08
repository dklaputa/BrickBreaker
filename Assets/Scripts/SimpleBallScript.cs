using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBallScript : MonoBehaviour
{
    private const float ballSize = .125f;
    private Vector3 speed;

    private void Start()
    {
        speed = 5 * new Vector2(Random.value - .5f, Random.value - .5f).normalized;
    }

    private void Update()
    {
        var remainTime = Time.deltaTime;
        var hit = Physics2D.CircleCast(transform.position, ballSize, speed,
            speed.magnitude * remainTime);
        var count = 0;
        while (hit.collider != null && count < 5)
        {
            var o = hit.collider.gameObject;

            if (o.CompareTag("Wall"))
            {
                remainTime -= hit.distance / speed.magnitude;
                transform.position = hit.point + hit.normal * ballSize;
                speed = Vector2.Reflect(speed, hit.normal);
            }
            else
                break;

            hit = Physics2D.CircleCast(transform.position, ballSize, speed, speed.magnitude * remainTime);
            count++;
        }

        transform.position += speed * remainTime;
    }
}
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     A simple ball that bounces when it hits wall.
/// </summary>
public class SimpleBallScript : MonoBehaviour
{
    private const float BallSize = .125f;
    private Vector3 speed;

    private void Start()
    {
        speed = 5 * new Vector2(Random.value - .5f, Random.value - .5f).normalized;
    }

    private void Update()
    {
        float remainTime = Time.deltaTime;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, BallSize, speed,
            speed.magnitude * remainTime);
        int count = 0;
        while (hit.collider != null && count < 5)
        {
            GameObject o = hit.collider.gameObject;

            if (o.CompareTag("Wall"))
            {
                remainTime -= hit.distance / speed.magnitude;
                transform.position = hit.point + hit.normal * BallSize;
                speed = Vector2.Reflect(speed, hit.normal);
            }
            else
            {
                break;
            }

            hit = Physics2D.CircleCast(transform.position, BallSize, speed, speed.magnitude * remainTime);
            count++;
        }

        transform.position += speed * remainTime;
    }
}
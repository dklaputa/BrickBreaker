using UnityEngine;

public class CloneBallScript : MonoBehaviour
{
    private static readonly float[] SpeedArray = {8, 9, 10, 11, 12, 13, 14, 15, 16};

    private Vector2 speed;
    private int speedLvl;

    public void setInitialSpeed(Vector2 direction, int speedLevel)
    {
        speedLvl = speedLevel;
        speed = direction * SpeedArray[speedLvl];
    }


    private void Update()
    {
        var remainTime = Time.deltaTime;
        var hit = Physics2D.CircleCast(transform.position, .125f, speed,
            speed.magnitude * remainTime);
        var count = 0;
        while (hit.collider != null && count < 5)
        {
            var o = hit.collider.gameObject;

            if (o.CompareTag("Wall"))
            {
                remainTime -= hit.distance / speed.magnitude;
                transform.position = hit.point + hit.normal * .125f;
                speed = Vector2.Reflect(speed, hit.normal);
                var blackHoleItem = ItemManager.instance.checkItem(ItemManager.BlackHole);
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
                    var blackHoleItem = ItemManager.instance.checkItem(ItemManager.BlackHole);
                    if (blackHoleItem > 0)
                        BlackHoleManager.instance.ShowBlackHole(transform.position, .1f + blackHoleItem * .1f);
                }
                if (speedLvl >= brick.level)
                {
                    SpeedLevelChange(-brick.level - 1);
                    PointsTextManager.instance.ShowPointsText(brick.transform.position, brick.Break());
                }
            }
            else if (o.CompareTag("GameOverTrigger") && speed.y < 0)
            {
                gameObject.SetActive(false);
                break;
            }
            else break;
            hit = Physics2D.CircleCast(transform.position, .125f, speed, speed.magnitude * remainTime);
            count++;
        }
        transform.position += (Vector3) speed * remainTime;
    }


    private void SpeedLevelChange(int lvlChange)
    {
        speedLvl = Mathf.Clamp(speedLvl + lvlChange, 0, SpeedArray.Length - 1);
        speed = speed.normalized * SpeedArray[speedLvl];
    }
}
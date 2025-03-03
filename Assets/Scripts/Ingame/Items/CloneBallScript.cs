﻿using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Clone ball generated by Division item.
///     Similar to real ball excepts: 1. No interaction with rope. 2. No color. 3. Not trigger game over.
/// </summary>
public class CloneBallScript : MonoBehaviour
{
    private const float BallSize = .1f;
    private static readonly float[] SpeedArray = {8, 9, 10, 11, 12, 12.5f, 13, 13.5f, 14};

    private Vector2 speed;
    private int speedLvl;

    public void SetInitialSpeed(Vector2 direction, int speedLevel)
    {
        speedLvl = speedLevel;
        speed = direction * SpeedArray[speedLvl];
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
                int blackHoleLevel = ItemManagerScript.instance.CheckItemActivated(ItemManagerScript.BlackHole);
                if (blackHoleLevel >= 0)
                    BlackHolePoolScript.instance.ShowBlackHole(transform.position, blackHoleLevel);
            }
            else if (o.CompareTag("Brick"))
            {
                remainTime -= hit.distance / speed.magnitude;
                transform.position = hit.point + hit.normal * BallSize;
                BrickScript brick = o.GetComponent<BrickScript>();
                int brickLevel =
                    brick.level == -1 ? 0 : brick.level; //Item brick can be treated as a level 0 brick.
                if (speedLvl <= brickLevel)
                {
                    speed = Vector2.Reflect(speed, hit.normal);
                    int blackHoleLevel = ItemManagerScript.instance.CheckItemActivated(ItemManagerScript.BlackHole);
                    if (blackHoleLevel >= 0)
                        BlackHolePoolScript.instance.ShowBlackHole(transform.position, blackHoleLevel);
                }

                if (speedLvl >= brickLevel)
                {
                    SpeedLevelChange(-brickLevel - 1);
                    int points = brick.Break();
                    if (points > 0)
                        PointsTextPoolScript.instance.ShowPointsText(brick.transform.position,
                            GameControllerScript.instance.AddPointsConsiderCombo(points, false));
                }
            }
            else if (o.CompareTag("GameOverTrigger") && speed.y < 0)
            {
                gameObject.SetActive(false);
                break;
            }
            else
            {
                break;
            }

            hit = Physics2D.CircleCast(transform.position, BallSize, speed, speed.magnitude * remainTime);
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
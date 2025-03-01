using System.Collections;
using System.Linq;
using UnityEngine;

public class BlackHoleScript : MonoBehaviour
{
    private const float Radius = 2.56f;
    private float scale;
    private Coroutine checkBricks;

    public void SetRange(float range)
    {
        scale = range;
    }

    private void OnEnable()
    {
        transform.localScale = new Vector2(scale, scale);
        StartCoroutine(Remove());
        checkBricks = StartCoroutine(CheckBricks());
    }

    // Destroy bricks that within the range every 0.5 seconds.
    private IEnumerator CheckBricks()
    {
        for (;;)
        {
            Collider2D[] collider2Ds =
                Physics2D.OverlapCircleAll(transform.position, Radius * scale, LayerMask.GetMask("Brick"));
            int points = collider2Ds.Sum(col => col.gameObject.GetComponent<BrickScript>().Break());
            if (points > 0)
            {
                GameControllerScript.instance.AddPointsIgnoreCombo(points);
                PointsTextPoolScript.instance.ShowPointsText(transform.position, points);
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    // Black hole lasts 1.5 seconds.
    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(1.5f);
        StopCoroutine(checkBricks);
        gameObject.SetActive(false);
    }
}
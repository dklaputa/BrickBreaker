using System.Collections;
using System.Linq;
using UnityEngine;

public class BlackHoleScript : MonoBehaviour
{
    private float scale;
    private const float radius = 2.56f;

    public void SetRange(float range)
    {
        scale = range;
    }

    private void OnEnable()
    {
        transform.localScale = new Vector2(scale, scale);
        StartCoroutine("Destory");
        StartCoroutine("CheckBricks");
    }

    // Destroy bricks that within the range every 0.5 seconds.
    private IEnumerator CheckBricks()
    {
        for (;;)
        {
            var collider2Ds =
                Physics2D.OverlapCircleAll(transform.position, radius * scale, LayerMask.GetMask("Brick"));
            var points = collider2Ds.Sum(col => col.gameObject.GetComponent<BrickScript>().Break());
            if (points > 0)
            {
                GameController.instance.AddPointsIgnoreCombo(points);
                PointsTextManager.instance.ShowPointsText(transform.position, points);
            }

            yield return new WaitForSeconds(.5f);
        }
    }

    // Black hole lasts 1.5 seconds.
    private IEnumerator Destory()
    {
        yield return new WaitForSeconds(1.5f);
        StopCoroutine("CheckBricks");
        gameObject.SetActive(false);
    }
}
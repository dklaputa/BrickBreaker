using System.Collections;
using System.Linq;
using UnityEngine;

public class BlackHoleScript : MonoBehaviour
{
    private float size;

    public void SetRange(float range)
    {
        size = range;
    }

    private void OnEnable()
    {
        transform.localScale = new Vector2(size, size);
        StartCoroutine("Destory");
        StartCoroutine("CheckBricks");
    }

    // Destroy bricks that within the range every 0.5 seconds.
    private IEnumerator CheckBricks()
    {
        for (;;)
        {
            var collider2Ds = Physics2D.OverlapCircleAll(transform.position, 2.56f * size, LayerMask.GetMask("Brick"));
            var score = collider2Ds.Sum(col => col.gameObject.GetComponent<BrickScript>().Break());
            if (score > 0) PointsTextManager.instance.ShowPointsText(transform.position, score);
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
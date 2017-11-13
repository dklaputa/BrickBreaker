using System.Linq;
using UnityEngine;

public class BlackHoleScript : MonoBehaviour
{
    private void OnEnable()
    {
        var collider2Ds = Physics2D.OverlapCircleAll(transform.position, .45f, LayerMask.GetMask("Brick"));
        var score = collider2Ds.Sum(col => col.gameObject.GetComponent<BrickScript>().Break());
        if (score > 0) PointsTextManager.instance.ShowPointsText(transform.position, score);
        Invoke("Destory", 1.5f);
    }

    private void Destory()
    {
        gameObject.SetActive(false);
    }
}
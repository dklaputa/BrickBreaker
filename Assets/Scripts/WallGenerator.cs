using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    public GameObject wallPrefab;

    // Use this for initialization
    private void Awake()
    {
        var top = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height, 0));
        var bottom = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0f, 0));
        var left = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 0));
        var right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0));

        var width = Vector3.Distance(left, right);
        var height = Vector3.Distance(bottom, top);

        var topCol = Instantiate(wallPrefab, transform).GetComponent<BoxCollider2D>();
        topCol.offset = top + new Vector3(0, 0.3f, 0);
        topCol.size = new Vector3(width, 0.6f, 0);
        var leftCol = Instantiate(wallPrefab, transform).GetComponent<BoxCollider2D>();
        leftCol.offset = left - new Vector3(0.3f, 0, 0);
        leftCol.size = new Vector3(0.6f, height, 0);
        var rightCol = Instantiate(wallPrefab, transform).GetComponent<BoxCollider2D>();
        rightCol.offset = right + new Vector3(0.3f, 0, 0);
        rightCol.size = new Vector3(0.6f, height, 0);
        var bottomCol = Instantiate(wallPrefab, transform).GetComponent<BoxCollider2D>();
        bottomCol.offset = bottom - new Vector3(0, 0.3f, 0);
        bottomCol.size = new Vector3(width, 0.6f, 0);
    }
}
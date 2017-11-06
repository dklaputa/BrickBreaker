using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    // Use this for initialization
    private void Awake()
    {
        var left = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 0));
        var right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0));

        var width = Vector3.Distance(left, right);

        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().size = new Vector2(width * 2, 20);
        transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().size = new Vector2(width * 2, 1.5f);
    }
}
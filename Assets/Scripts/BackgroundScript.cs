using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    // Use this for initialization
    private void Awake()
    {
        var left = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 0));
        var right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0));
        var width = Vector3.Distance(left, right);

        // Fit the bacground with screen width.
        var sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.size = new Vector2(width * 2, sprite.size.y);
    }
}
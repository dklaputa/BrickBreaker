using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    // Use this for initialization
    private void Awake()
    {
        Vector3 left = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 0));
        Vector3 right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0));
        float width = Vector3.Distance(left, right);

        // Fit the bacground with screen width.
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        sprite.size = new Vector2(width * 2, sprite.size.y);
    }
}
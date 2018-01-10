using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControllerScript : MonoBehaviour
{
    private void Awake()
    {
        Vector3 top = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height, 0));
        Vector3 bottom = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, 0f, 0));
        Vector3 left = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 0));
        Vector3 right = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, 0));

        float width = Vector3.Distance(left, right);
        float height = Vector3.Distance(bottom, top);

        BoxCollider2D topCol = GameObject.Find("WallUp").GetComponent<BoxCollider2D>();
        topCol.offset = top + new Vector3(0, 0.3f, 0);
        topCol.size = new Vector3(width, 0.6f, 0);
        BoxCollider2D leftCol = GameObject.Find("WallLeft").GetComponent<BoxCollider2D>();
        leftCol.offset = left - new Vector3(0.3f, 0, 0);
        leftCol.size = new Vector3(0.6f, height, 0);
        BoxCollider2D rightCol = GameObject.Find("WallRight").GetComponent<BoxCollider2D>();
        rightCol.offset = right + new Vector3(0.3f, 0, 0);
        rightCol.size = new Vector3(0.6f, height, 0);
        BoxCollider2D bottomCol = GameObject.Find("WallDown").GetComponent<BoxCollider2D>();
        bottomCol.offset = bottom - new Vector3(0, 0.3f, 0);
        bottomCol.size = new Vector3(width, 0.6f, 0);
    }

    // Game end on esc pressed.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void onStartClick()
    {
        SceneManager.LoadScene("Main");
    }

    public void onExitClick()
    {
        Application.Quit();
    }

    public void onStoreClick()
    {
        SceneManager.LoadScene("Store");
    }
}
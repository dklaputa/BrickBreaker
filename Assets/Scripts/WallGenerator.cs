using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
	public GameObject wallPrefab;
	// Use this for initialization
	void Start ()
	{
		Vector3 top = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2f, Screen.height, 0));  
		Vector3 bottom = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width / 2f, 0f, 0));  
		Vector3 left = Camera.main.ScreenToWorldPoint (new Vector3 (0f, Screen.height / 2f, 0));  
		Vector3 right = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height / 2f, 0));  

		float width = Vector3.Distance (left, right);  
		float height = Vector3.Distance (bottom, top);

		BoxCollider2D topCol = ((GameObject)Instantiate (wallPrefab, transform)).GetComponent<BoxCollider2D> ();
		topCol.offset = top + new Vector3 (0, 0.3f, 0);
		topCol.size = new Vector3 (width, 0.6f, 0);
		BoxCollider2D leftCol = ((GameObject)Instantiate (wallPrefab, transform)).GetComponent<BoxCollider2D> ();
		leftCol.offset = left - new Vector3 (0.3f, 0, 0);
		leftCol.size = new Vector3 (0.6f, height, 0);
		BoxCollider2D rightCol = ((GameObject)Instantiate (wallPrefab, transform)).GetComponent<BoxCollider2D> ();
		rightCol.offset = right + new Vector3 (0.3f, 0, 0);
		rightCol.size = new Vector3 (0.6f, height, 0);
	}
}

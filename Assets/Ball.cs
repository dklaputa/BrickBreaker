using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour {

	public float speedDown = 10;
	public float force = 1;
	Rigidbody2D rigid2D;

	// Use this for initialization
	void Start () {
		rigid2D = GetComponent<Rigidbody2D>(); 
		rigid2D.velocity = - transform.up * speedDown;
	}

	// Update is called once per frame
	void Update () {
		//		rigid2D.AddForce (new Vector2 (rigid2D.velocity.y, -rigid2D.velocity.x) * force);
		if (Input.GetMouseButton (1)) {
			rigid2D.AddForce (Vector2.down * force);
		}
		if (Mathf.Abs (transform.position.x) > 4 || Mathf.Abs (transform.position.y) > 6)
			SceneManager.LoadScene ("Main"); 
		if (Physics2D.OverlapCircleAll ((Vector2)transform.position, 0.5f).Length == 1 && rigid2D.velocity.magnitude < speedDown) {
			Debug.Log ("acc");
			rigid2D.velocity = rigid2D.velocity.normalized * speedDown;
		}
	}
}

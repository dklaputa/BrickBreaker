using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
	
	public int initialPoolSize = 2;
	public float minLength = .5f;
	public GameObject ropePrefab;

	List<GameObject> ropePool = new List<GameObject> ();
	Vector2[] endPositions = new Vector2[2];

	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < initialPoolSize; i++) {
			ropePool.Add ((GameObject)Instantiate (ropePrefab, transform));
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			endPositions [0] = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		} else if (Input.GetMouseButtonUp (0)) {
			endPositions [1] = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			if ((endPositions [1] - endPositions [0]).magnitude < minLength)
				return;
			bool done = false;
			foreach (GameObject rope in ropePool) {
				if (rope.activeInHierarchy) {
					rope.GetComponent<RopeScript> ().Remove ();
				}
				else if(!done) {
					rope.GetComponent<RopeScript> ().setEndPointPositions (endPositions[0], endPositions[1]);
					rope.SetActive (true);
					done = true;
				}
			}
			if (!done) {
				GameObject newRope = (GameObject)Instantiate (ropePrefab, transform);
				newRope.GetComponent<RopeScript> ().setEndPointPositions (endPositions [0], endPositions [1]);
				newRope.SetActive (true);
				ropePool.Add (newRope);
			}
		}
	}
}

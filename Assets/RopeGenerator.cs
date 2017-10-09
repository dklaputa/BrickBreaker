using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{

	public GameObject nodePrefab;
	public float minDistance = 0.2f;
	Vector2 startPosition;
	Vector2 endPosition;
	GameObject[] nodes;
	LineRenderer lineRenderer;

	// Use this for initialization
	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			startPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		} else if (Input.GetMouseButtonUp (0)) {
			endPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
			float distance = (startPosition - endPosition).magnitude;
			if (distance < minDistance)
				return;
			int nodeNum = Mathf.FloorToInt (distance / minDistance) + 1;
			Vector2 delta = (endPosition - startPosition) / (nodeNum - 1);

			if (nodes != null && nodes.Length > 0) {
				for (int i = 0; i < nodes.Length; i++) {
					Destroy (nodes [i]);
				}
			}

			nodes = new GameObject[nodeNum];
			nodes [0] = (GameObject)Instantiate (nodePrefab, transform); 
			nodes [0].transform.position = startPosition;
			for (int i = 1; i < nodeNum - 1; i++) {
				nodes [i] = (GameObject)Instantiate (nodePrefab, transform);
				nodes [i].transform.position = (Vector2)nodes [i - 1].transform.position + delta;
				nodes [i].GetComponent<FixedJoint2D> ().connectedBody = nodes [i - 1].GetComponent<Rigidbody2D> (); 
			}
			nodes [nodeNum - 1] = (GameObject)Instantiate (nodePrefab, transform);
			nodes [nodeNum - 1].transform.position = (Vector2)nodes [nodeNum - 2].transform.position + delta;
			nodes [nodeNum - 2].AddComponent<FixedJoint2D> ().connectedBody = nodes [nodeNum - 1].GetComponent<Rigidbody2D> ();
		}

		if (nodes != null && nodes.Length > 0) {
			lineRenderer.enabled = true;
			lineRenderer.SetVertexCount (nodes.Length); 
			for (int i = 0; i < nodes.Length; i++) { 
				lineRenderer.SetPosition (i, nodes [i].transform.position); 
			}
		} else {
			lineRenderer.enabled = false;
		}
	}
}

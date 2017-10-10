using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
	enum Status
	{
		Before,
		On,
		After
	}

	Vector2[] endPointPositions;
	EdgeCollider2D edgeCollider2D;
	LineRenderer lineRenderer;
	GameObject ball;
	Rigidbody2D ballRigidBody;
	GameObject ropeNodeLeft;
	GameObject ropeNodeMiddle;
	GameObject ropeNodeRight;
	Rigidbody2D ropeNodeMiddleRigidBody;
	SpringJoint2D springJointsLeft;
	SpringJoint2D springJointsRight;
	Status status = Status.Before;

	public void setEndPointPositions (Vector2[] positions)
	{
		endPointPositions = positions; 
	}

	// Use this for initialization
	void Start ()
	{
		edgeCollider2D = GetComponent<EdgeCollider2D> ();
		lineRenderer = GetComponent<LineRenderer> ();
		ball = GameObject.Find ("Ball");
		ballRigidBody = ball.GetComponent<Rigidbody2D> ();
		ropeNodeLeft = transform.GetChild (0).gameObject; 
		ropeNodeMiddle = transform.GetChild (1).gameObject; 
		ropeNodeRight = transform.GetChild (2).gameObject; 
		ropeNodeMiddleRigidBody = ropeNodeMiddle.GetComponent<Rigidbody2D> ();
		springJointsLeft = ropeNodeLeft.GetComponent<SpringJoint2D> ();
		springJointsRight = ropeNodeRight.GetComponent<SpringJoint2D> ();

		edgeCollider2D.points = endPointPositions;
		ropeNodeLeft.transform.position = endPointPositions [0];
		ropeNodeRight.transform.position = endPointPositions [1];
	}

	// Update is called once per frame
	void Update ()
	{ 
		switch (status) {
		case Status.Before: 
			lineRenderer.positionCount = 10;
			lineRenderer.SetPositions (Curver.MakeSmoothCurve (new Vector3[] {
				ropeNodeLeft.transform.position,
				ropeNodeRight.transform.position
			}, 5)); 
			break;
		case Status.On: 
			lineRenderer.positionCount = 15;
			lineRenderer.SetPositions (Curver.MakeSmoothCurve (new Vector3[] {
				ropeNodeLeft.transform.position,
				ball.transform.position,
				ropeNodeRight.transform.position
			}, 5)); 
			break;
		case Status.After:  
			lineRenderer.positionCount = 15;
			lineRenderer.SetPositions (Curver.MakeSmoothCurve (new Vector3[] {
				ropeNodeLeft.transform.position,
				ropeNodeMiddle.transform.position,
				ropeNodeRight.transform.position
			}, 5)); 
			break;
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{ 
		if (other.gameObject.tag == "Ball") {
			if (status == Status.Before) {
				springJointsLeft.connectedBody = ballRigidBody;
				springJointsRight.connectedBody = ballRigidBody;
				status = Status.On;
			} else if (status == Status.On) {
				springJointsLeft.connectedBody = null;
				springJointsRight.connectedBody = null;
				ballRigidBody.velocity = ballRigidBody.velocity.normalized * 10;
				ropeNodeMiddle.transform.position = ball.transform.position;
				ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity;
				ropeNodeMiddle.SetActive (true);
				status = Status.After;
			}
		}
	}
}

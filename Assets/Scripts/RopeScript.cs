using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
	enum Status
	{
		BeforeTouch,
		DuringTouch,
		AfterTouch
	}

	enum Direction
	{
		UpDown,
		DownUp
	}

	public float endRange = .2f;

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
	SpringJoint2D[] springJointsMiddle;

	Status status = Status.BeforeTouch;
	Direction enterDirection;
	Vector3[] ropePath = new Vector3[30];

	public void setEndPointPositions (Vector2 position1, Vector2 position2)
	{
		endPointPositions = new Vector2[]{ position1, position2 }; 
	}

	// Use this for initialization
	void Start ()
	{
		edgeCollider2D = GetComponent<EdgeCollider2D> ();
		lineRenderer = GetComponent<LineRenderer> ();
		lineRenderer.sortingLayerName = "Rope";
		ball = GameObject.Find ("Ball");
		ballRigidBody = ball.GetComponent<Rigidbody2D> ();
		ropeNodeLeft = transform.GetChild (0).gameObject; 
		ropeNodeMiddle = transform.GetChild (1).gameObject; 
		ropeNodeRight = transform.GetChild (2).gameObject; 
		ropeNodeMiddleRigidBody = ropeNodeMiddle.GetComponent<Rigidbody2D> ();
		springJointsLeft = ropeNodeLeft.GetComponent<SpringJoint2D> ();
		springJointsRight = ropeNodeRight.GetComponent<SpringJoint2D> ();
		springJointsMiddle = ropeNodeMiddle.GetComponents<SpringJoint2D> ();

		edgeCollider2D.points = endPointPositions;
		ropeNodeLeft.transform.position = endPointPositions [0];
		ropeNodeRight.transform.position = endPointPositions [1];
	}

	// Update is called once per frame
	void Update ()
	{ 
		lineRenderer.positionCount = 30;
		switch (status) {
		case Status.BeforeTouch: 
			ropePathBeforeTouch (ropeNodeLeft.transform.position, ropeNodeRight.transform.position, ropePath); 
			break;
		case Status.DuringTouch: 
			ropePathDuringTouch (ropeNodeLeft.transform.position, ropeNodeRight.transform.position, ball.transform.position, 0.2f, enterDirection, ropePath);
			break;
		case Status.AfterTouch:   
			ropePathAfterTouch (ropeNodeLeft.transform.position, ropeNodeMiddle.transform.position, ropeNodeRight.transform.position, ropePath); 
			break;
		}
		lineRenderer.SetPositions (ropePath);
	}

	void OnTriggerEnter2D (Collider2D other)
	{ 
		if (other.gameObject.tag == "Ball") {
			if (status == Status.BeforeTouch) { 
				Vector3 vAB = ropeNodeLeft.transform.position - ropeNodeRight.transform.position;
				Vector3 vAC = ropeNodeLeft.transform.position - ball.transform.position;
				Vector3 vBC = ropeNodeRight.transform.position - ball.transform.position;
				Vector3 vCD = ballRigidBody.velocity;
				float lAB = vAB.magnitude;
				float lAC = vAC.magnitude;
				float lBC = vBC.magnitude;
				float lCD = vCD.magnitude;
				if (lAC < endRange || lBC < endRange) {
					ballRigidBody.velocity = -10 * Vector3.Reflect (ballRigidBody.velocity, vAB).normalized; 
					ropeNodeMiddle.transform.position = Vector3.Lerp (ropeNodeLeft.transform.position, ropeNodeRight.transform.position, 0.5f);
					springJointsMiddle [0].distance = lAB * .45f;
					springJointsMiddle [1].distance = lAB * .45f;
					ropeNodeMiddle.SetActive (true);
					ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity * .5f;
					status = Status.AfterTouch;
				} else {
					float sinACD = Vector3.Cross (vAC, vCD).z / (lAC * lCD);
					float sinBCD = Vector3.Cross (vBC, vCD).z / (lBC * lCD);
					float sinADC = Vector3.Cross (vAB, vCD).z / (lAB * lCD);
					float lAD = Mathf.Abs (lAC * sinACD / sinADC);
					float lBD = Mathf.Abs (lBC * sinBCD / sinADC);

					springJointsLeft.connectedBody = ballRigidBody;
					springJointsRight.connectedBody = ballRigidBody;
					springJointsLeft.distance = lAD * .9f;
					springJointsRight.distance = lBD * .9f;
					springJointsMiddle [0].distance = lAD * .9f;
					springJointsMiddle [1].distance = lBD * .9f;

					if (sinADC < 0)
						enterDirection = Direction.UpDown;
					else
						enterDirection = Direction.DownUp;
					status = Status.DuringTouch;
				}
			} else if (status == Status.DuringTouch) { 
				springJointsLeft.connectedBody = null;
				springJointsRight.connectedBody = null;
				ballRigidBody.velocity = ballRigidBody.velocity.normalized * 10;
				ropeNodeMiddle.transform.position = ball.transform.position;
				ropeNodeMiddle.SetActive (true);
				ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity;
				status = Status.AfterTouch; 
			}
		}
	}

	void ropePathBeforeTouch (Vector3 pointL, Vector3 pointR, Vector3[] ropePath)
	{
		for (int i = 0; i < 30; i++) {
			ropePath [i] = Vector3.Lerp (pointL, pointR, i / 29f);
		}
	}

	void ropePathDuringTouch (Vector3 pointL, Vector3 pointR, Vector3 circleCenter, float r, Direction direction, Vector3[] ropePath)
	{
		float dL = (pointL - circleCenter).magnitude; 
		float dR = (pointR - circleCenter).magnitude;
		float fL = Mathf.Acos (r / dL); 
		float fR = Mathf.Acos (r / dR); 

		Vector3 normalizedL = (pointL - circleCenter).normalized; 
		Vector3 normalizedR = (pointR - circleCenter).normalized; 

		Vector3[] rotated = new Vector3[2]; 
		if (direction == Direction.DownUp) {
			rotated [0] = r * new Vector3 (normalizedL.x * Mathf.Cos (fL) - normalizedL.y * Mathf.Sin (fL), normalizedL.x * Mathf.Sin (fL) + normalizedL.y * Mathf.Cos (fL));
			rotated [1] = r * new Vector3 (normalizedR.x * Mathf.Cos (fR) + normalizedR.y * Mathf.Sin (fR), -normalizedR.x * Mathf.Sin (fR) + normalizedR.y * Mathf.Cos (fR));
		} else {
			rotated [0] = r * new Vector3 (normalizedL.x * Mathf.Cos (fL) + normalizedL.y * Mathf.Sin (fL), -normalizedL.x * Mathf.Sin (fL) + normalizedL.y * Mathf.Cos (fL));
			rotated [1] = r * new Vector3 (normalizedR.x * Mathf.Cos (fR) - normalizedR.y * Mathf.Sin (fR), normalizedR.x * Mathf.Sin (fR) + normalizedR.y * Mathf.Cos (fR));
		}
		Vector3[] tangents = new Vector3[]{ rotated [0] + circleCenter, rotated [1] + circleCenter  };
		for (int i = 0; i < 10; i++) {
			ropePath [i] = Vector3.Lerp (pointL, tangents [0], i / 9f);
		}
		for (int i = 10; i < 20; i++) {
			ropePath [i] = Vector3.Slerp (rotated [0], rotated [1], (i - 10) / 9f) + circleCenter;
		}
		for (int i = 20; i < 30; i++) {
			ropePath [i] = Vector3.Lerp (tangents [1], pointR, (i - 20) / 9f);
		}
	}

	void ropePathAfterTouch (Vector3 pointL, Vector3 pointM, Vector3 pointR, Vector3[] ropePath)
	{
		for (int i = 0; i < 30; i++) {
			float t = i / 29f;
			Vector3[] points = new Vector3[]{ pointL, pointM, pointR };
			for (int m = 2; m > 0; m--) {
				for (int n = 0; n < m; n++) {
					points [n] = (1 - t) * points [n] + t * points [n + 1];
				}
			}
			ropePath [i] = points [0];
		} 
	}
}

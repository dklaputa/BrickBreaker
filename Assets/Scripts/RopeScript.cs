using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
	enum Status
	{
		BeforeTouchBall,
		DuringTouchBall,
		AfterTouchBall
	}

	enum Direction
	{
		UpDown,
		DownUp
	}

	float endRange = .25f;
	float randomBias = .1f;

	Vector2[] endPointPositions;
	EdgeCollider2D edgeCollider2D;
	LineRenderer lineRenderer;
	GameObject ball;
	Rigidbody2D ballRigidBody;
	GameObject ropeNodeLeft;
	GameObject ropeNodeMiddle;
	GameObject ropeNodeRight;
	Rigidbody2D ropeNodeMiddleRigidBody;
	SpringJoint2D[] springJointsMiddle;

	Status status = Status.BeforeTouchBall;
	bool isRemovable = true;
	bool isRemoving = false;
	Vector2 forceDirection;
	Direction enterDirection;
	Vector3[] ropePath = new Vector3[30];

	public void setEndPointPositions (Vector2 position1, Vector2 position2)
	{
		endPointPositions = new Vector2[]{ position1, position2 }; 
	}

	// Use this for initialization
	void Awake ()
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
		springJointsMiddle = ropeNodeMiddle.GetComponents<SpringJoint2D> ();
	}

	void OnEnable ()
	{
		edgeCollider2D.points = endPointPositions;
		edgeCollider2D.enabled = true;
		ropeNodeLeft.transform.position = endPointPositions [0];
		ropeNodeRight.transform.position = endPointPositions [1];
		ropeNodeMiddle.transform.position = (endPointPositions [0] + endPointPositions [1]) / 2;
		springJointsMiddle [0].distance = (endPointPositions [0] - endPointPositions [1]).magnitude * .4f;
		springJointsMiddle [1].distance = springJointsMiddle [0].distance;
	}

	// Update is called once per frame
	void Update ()
	{ 
		lineRenderer.positionCount = 30;
		switch (status) {
		case Status.BeforeTouchBall: 
			ropePathBeforeTouch (ropeNodeLeft.transform.position, ropeNodeRight.transform.position, ropePath); 
			break;
		case Status.DuringTouchBall: 
			ropePathDuringTouch (ropeNodeLeft.transform.position, ropeNodeRight.transform.position, ball.transform.position, 0.2f, enterDirection, ropePath);
			break;
		case Status.AfterTouchBall:   
			ropePathAfterTouch (ropeNodeLeft.transform.position, ropeNodeMiddle.transform.position, ropeNodeRight.transform.position, ropePath); 
			break;
		}
		lineRenderer.SetPositions (ropePath);
		if (isRemoving) {
			Color color1 = lineRenderer.startColor;
			Color color2 = lineRenderer.endColor;
			color1.a -= Time.deltaTime * 4;
			color2.a -= Time.deltaTime * 4;
			lineRenderer.startColor = color1;
			lineRenderer.endColor = color2;
		}
	}

	void FixedUpdate ()
	{
		if (status == Status.DuringTouchBall) {
			ballRigidBody.AddForce (forceDirection);
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{ 
		if (other.gameObject.tag == "Ball") {
			if (status == Status.BeforeTouchBall) { 
				Vector3 vAB = ropeNodeLeft.transform.position - ropeNodeRight.transform.position;
				Vector3 vAC = ropeNodeLeft.transform.position - ball.transform.position;
				Vector3 vBC = ropeNodeRight.transform.position - ball.transform.position;
				Vector3 vCD = ballRigidBody.velocity;
				float lAC = vAC.magnitude;
				float lBC = vBC.magnitude;
				if (lAC < endRange || lBC < endRange) {
					if ((lAC < endRange && Vector3.Dot (vAB, vCD) < 0) || (lBC < endRange && Vector3.Dot (vAB, vCD) > 0))
						ballRigidBody.velocity = rotateVector (-ballRigidBody.velocity, (Random.value - .5f) * 2 * randomBias);
					else
						ballRigidBody.velocity = rotateVector (-ballRigidBody.velocity.magnitude * Vector3.Reflect (ballRigidBody.velocity, vAB).normalized, (Random.value - .5f) * 2 * randomBias);
					ropeNodeMiddle.SetActive (true);
					ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity * .5f;
					status = Status.AfterTouchBall;
					Remove ();
				} else {
					if (Vector3.Cross (vAB, vCD).z < 0) {
						enterDirection = Direction.UpDown;
						forceDirection = -new Vector2 (vAB.y, -vAB.x).normalized; 
						forceDirection = rotateVector (forceDirection, (Random.value - .5f) * 2 * randomBias);
					} else {
						enterDirection = Direction.DownUp;
						forceDirection = new Vector2 (vAB.y, -vAB.x).normalized;
						forceDirection = rotateVector (forceDirection, (Random.value - .5f) * 2 * randomBias);
					}
					ballRigidBody.velocity = -forceDirection * ballRigidBody.velocity.magnitude;
					status = Status.DuringTouchBall;
					isRemovable = false;
				}
			}
		}
	}

	void OnTriggerExit2D (Collider2D other)
	{ 
		if (other.gameObject.tag == "Ball") {
			if (status == Status.DuringTouchBall && Vector3.Dot (ballRigidBody.velocity, forceDirection) > 0) {
				ballRigidBody.velocity = ballRigidBody.velocity.normalized * 10;
				ropeNodeMiddle.SetActive (true);
				ropeNodeMiddleRigidBody.velocity = ballRigidBody.velocity;
				status = Status.AfterTouchBall; 
				isRemovable = true;
				Remove ();
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
		if (r > dL || r > dR) {
			Debug.Log ("illegal parameter.");
			return;
		}
		float fL = Mathf.Acos (r / dL); 
		float fR = Mathf.Acos (r / dR); 

		Vector3 normalizedL = (pointL - circleCenter).normalized; 
		Vector3 normalizedR = (pointR - circleCenter).normalized; 

		Vector3[] rotated = new Vector3[2]; 
		if (direction == Direction.DownUp) {
			rotated [0] = r * rotateVector (normalizedL, fL); 
			rotated [1] = r * rotateVector (normalizedR, -fR);
		} else {
			rotated [0] = r * rotateVector (normalizedL, -fL);
			rotated [1] = r * rotateVector (normalizedR, fR);
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

	Vector2 rotateVector (Vector2 vector, float angle)
	{
		return new Vector2 (vector.x * Mathf.Cos (angle) - vector.y * Mathf.Sin (angle), vector.x * Mathf.Sin (angle) + vector.y * Mathf.Cos (angle));
	}

	public void Remove ()
	{
		if (isRemovable && !isRemoving) {
			isRemoving = true;
			edgeCollider2D.enabled = false;
			Invoke ("Destroy", .25f);
		}
	}

	void Destroy ()
	{
		lineRenderer.positionCount = 0;
		Color color1 = lineRenderer.startColor;
		Color color2 = lineRenderer.endColor;
		color1.a = 1;
		color2.a = 1;
		lineRenderer.startColor = color1;
		lineRenderer.endColor = color2;
		status = Status.BeforeTouchBall;
		ropeNodeMiddle.SetActive (false);
		gameObject.SetActive (false);
		isRemoving = false;
	}
}

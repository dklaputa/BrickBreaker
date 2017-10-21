using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BricksRangeScript : MonoBehaviour
{
    private Rigidbody2D ballRigid2D;

    // Use this for initialization
    private void Start()
    {
        ballRigid2D = BallScript.instance.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ballRigid2D.isKinematic = true;
            ballRigid2D.useFullKinematicContacts = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            ballRigid2D.isKinematic = false;
        }
    }
}
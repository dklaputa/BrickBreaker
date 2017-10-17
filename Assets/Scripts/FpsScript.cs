using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsScript : MonoBehaviour
{
	Text text;
	// Use this for initialization
	void Start ()
	{
		text = GameObject.Find ("FPS").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		text.text = (1f / Time.deltaTime).ToString();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUIScript : MonoBehaviour {
	private Text comboText;
	private Text comboPoints;
	private Animator comboAnimator;
	private void Awake()
	{
		comboText = gameObject.transform.GetChild(1).GetComponent<Text>();
		comboPoints = gameObject.transform.GetChild(2).GetComponent<Text>();
		comboAnimator = gameObject.GetComponent<Animator>();
	}

	public void SetCombo(int combo)
	{
		comboText.text = "×" + combo;
	}
	
	public void SetPointsAndRemove(int points)
	{
		comboPoints.text = "+" + points;
		comboPoints.gameObject.SetActive(true);
		StartCoroutine("Remove");
	}
	
	private IEnumerator Remove()
	{
		comboAnimator.SetTrigger("Hide");
		yield return new WaitForSeconds(0.8f);
		comboPoints.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
}

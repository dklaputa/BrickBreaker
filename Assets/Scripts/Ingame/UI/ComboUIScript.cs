using System.Collections;
using TMPro;
using UnityEngine;

public class ComboUIScript : MonoBehaviour
{
	private static readonly int Hide = Animator.StringToHash("Hide");
	[SerializeField] private TextMeshProUGUI comboText;
	[SerializeField] private TextMeshProUGUI comboPoints;
	private Animator comboAnimator;

	private void Awake()
	{
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
		StartCoroutine(Remove());
	}

	private IEnumerator Remove()
	{
		comboAnimator.SetTrigger(Hide);
		yield return new WaitForSeconds(0.8f);
		comboPoints.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}
}

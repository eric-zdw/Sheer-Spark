using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour {
	public ScoreManager scoreManager;
	private float scoreDisplay = 1f;

	private UnityEngine.UI.Text scoreText;
	private UnityEngine.UI.Text multiplierText;
	private UnityEngine.UI.Image completionBar;

	public int maxPowerups;
	public int maxPoints;

	private float scorePerPowerup;
	private float scorePerPoint;


	// Use this for initialization
	void Start () {
		scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();

		scoreText = transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
		multiplierText = transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
		completionBar = transform.GetChild(2).GetChild(0).GetComponent<UnityEngine.UI.Image>();

		maxPowerups = 50;
		maxPoints = 20000;

		StartCoroutine(UpdateScore());
	}

	private IEnumerator UpdateScore() {
		while (true) {
			scoreDisplay = Mathf.Lerp(scoreDisplay, ScoreManager.score, 0.1f);
			scoreText.text = Mathf.RoundToInt(scoreDisplay).ToString("D8");
			multiplierText.text = "x" + ((Mathf.Round(ScoreManager.multiplier * 10f)) / 10f).ToString("F1");
			completionBar.fillAmount = Mathf.Lerp(completionBar.fillAmount, ScoreManager.completion, 0.1f);

			yield return new WaitForEndOfFrame();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashIndicatorPanel : MonoBehaviour {

	public UnityEngine.UI.Image dash1;
	public UnityEngine.UI.Image dash2;
	public UnityEngine.UI.Image dash3;
	public UnityEngine.UI.Image dash4;
	public UnityEngine.UI.Image dash5;
	public UnityEngine.UI.Image bg1;
	public UnityEngine.UI.Image bg2;
	public UnityEngine.UI.Image bg3;
	public UnityEngine.UI.Image bg4;
	public UnityEngine.UI.Image bg5;
	private DashBoost dashBoost;
	private TeleBoost teleBoost;

	private float valueWidth, valueHeight;
	private float percent;

	private float red = 0f;
	private float alpha = 0.5f;
	private float fadeDelay = 0.8f; 

	private float totalPixels;
	private float lerpFill = 5f;
	private float lerpVelocity = 0f;

	// Use this for initialization
	void Start () {
		if (GameObject.Find("DashBoost")) {
			dashBoost = GameObject.Find("DashBoost").GetComponent<DashBoost>();
		}
		if (GameObject.Find("TeleBoost")) {
			teleBoost = GameObject.Find("TeleBoost").GetComponent<TeleBoost>();
		}

		percent = dashBoost.uses / 5f;
		lerpFill = Mathf.Lerp(lerpFill, dashBoost.uses, 0.9f);

		dash1.fillAmount = lerpFill;
		dash2.fillAmount = lerpFill - 1f;
		dash3.fillAmount = lerpFill - 2f;
		dash4.fillAmount = lerpFill - 3f;
		dash5.fillAmount = lerpFill - 4f;
	}
	
	// Update is called once per frame
	void Update () {
		percent = dashBoost.uses / 5f;
		
		//if (dashBoost) fill = dashBoost.uses / 4f;
		//if (teleBoost) fill = teleBoost.uses / 4f;

		//lerpFill = Mathf.SmoothDamp(lerpFill, dashBoost.uses, ref lerpVelocity, 0.01f);

		dash1.fillAmount = dashBoost.uses;
		dash2.fillAmount = dashBoost.uses - 1f;
		dash3.fillAmount = dashBoost.uses - 2f;
		dash4.fillAmount = dashBoost.uses - 3f;
		dash5.fillAmount = dashBoost.uses - 4f;

		Color color;
		// non-full dash
		if (percent < 1f) {
			alpha = 0.5f;
			fadeDelay = 0.8f;

			if (percent < 0.20f) {
				if (dashBoost) red = 0.2f;
				if (teleBoost) red = 0.2f;
				color = new Color (1f, red, red, alpha);
			}
			else {
				red = 1f;
			}
			color = new Color (1f, red, red, alpha);
		}
		// full dash
		else {
			red = 1f;
			fadeDelay -= Time.deltaTime;
			if (fadeDelay < 0f && alpha > 0f) {
				alpha -= Time.deltaTime;
			}
		}
		color = new Color(1f, red, red, alpha);

		GetComponent<CanvasGroup>().alpha = alpha;
		dash1.color = color;
		dash2.color = color;
		dash3.color = color;
		dash4.color = color;
		dash5.color = color;
		bg1.color = color * 0.4f;
		bg2.color = color * 0.4f;
		bg3.color = color * 0.4f;
		bg4.color = color * 0.4f;
		bg5.color = color * 0.4f;
	}
}

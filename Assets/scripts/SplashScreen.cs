using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour {

	public GameObject mainMenuScreen;
	public CanvasGroup title;
	public CanvasGroup pressAnyKey;
	public UnityEngine.UI.Image darken;

	private bool splashActive = false;

	// Use this for initialization
	void Start () {
		darken.color = new Color(0f, 0f, 0f, 1f);
		title.alpha = 0f;
		pressAnyKey.alpha = 0f;
		StartCoroutine(SplashRoutine());
	}

	private IEnumerator SplashRoutine()
    {
		/*
		float t = 0f;
		Vector3 rgb = new Vector3(darken.color.r, darken.color.g, darken.color.b);
		Vector3 newrgb;
		while (t < 1f)
        {
			newrgb = Vector3.Lerp(rgb, Vector3.zero, t);
			darken.color = new Color(newrgb.x, newrgb.y, newrgb.z, 1f);
			t += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		*/

		yield return new WaitForSeconds(2f);


		float alpha = 1f;
		bool titleStart = false;
		while (alpha > 0f)
        {
			alpha -= Time.deltaTime * 0.2f;
			darken.color = new Color(0f, 0f, 0f, alpha);

			if (alpha < 0.5f && !titleStart)
            {
				StartCoroutine(TitleFadeIn());
				titleStart = true;
            } 

			yield return new WaitForEndOfFrame();
        }
    }

	private IEnumerator TitleFadeIn()
    {
		while (title.alpha < 1f)
        {
			title.alpha += Time.deltaTime * 0.5f;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(1.5f);
		StartCoroutine(PressAnyKeyFlash());
    }

	private IEnumerator PressAnyKeyFlash()
    {
		splashActive = true;
		int direction = 0;
		while (true)
        {
			if (direction == 0)
            {
				pressAnyKey.alpha += Time.deltaTime * 0.5f;
				if (pressAnyKey.alpha >= 1f)
                {
					direction = 1;
                }
            }
			else
            {
				pressAnyKey.alpha -= Time.deltaTime * 0.5f;
				if (pressAnyKey.alpha <= 0f)
                {
					direction = 0;
                }
            }
			yield return new WaitForEndOfFrame();
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKey && splashActive) {
			MenuCameraRig.ChangePosition(1);
			mainMenuScreen.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}

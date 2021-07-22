using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndUI : MonoBehaviour
{
    public UnityEngine.UI.Image blackBG;
    public UnityEngine.UI.Text levelTitle;
    public UnityEngine.UI.Text completeText;
    public UnityEngine.CanvasGroup statsPanel;
    public UnityEngine.UI.Text[] stats;
    public UnityEngine.CanvasGroup completionPanel;
    public UnityEngine.CanvasGroup buttonPanel;

    public UnityEngine.UI.Image bar;
    public UnityEngine.UI.Text percentText;

    // Start is called before the first frame update
    void Start()
    {
        levelTitle.color = new Color(1f, 1f, 1f, 0f);
        completeText.color = new Color(1f, 1f, 1f, 0f);
        //statsPanel.alpha = 0f;
        foreach (UnityEngine.UI.Text t in stats)
        {
            t.color = new Color(t.color.r, t.color.g, t.color.b, 0f);
        }
        completionPanel.alpha = 0f;
        buttonPanel.alpha = 0f;
        StartCoroutine(LevelCompleteRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator LevelCompleteRoutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        StartCoroutine(FadeInBG(blackBG, 0f, 0.5f, 0.1f));
        yield return new WaitForSecondsRealtime(2.5f);
        Cursor.visible = true;
        StartCoroutine(FadeInText(levelTitle));
        yield return new WaitForSecondsRealtime(0.6f);
        StartCoroutine(FadeInText(completeText));
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(FadeInStats());
        yield return new WaitForSecondsRealtime(2f);
        StartCoroutine(FadeInCanvasGroup(completionPanel));
        yield return new WaitForSecondsRealtime(0.8f);
        StartCoroutine(FillCompletionBar());
        yield return new WaitForSecondsRealtime(2.5f);
        StartCoroutine(FadeInCanvasGroup(buttonPanel));
        StartCoroutine(FadeInBG(blackBG, 0.5f, 1f, 0.1f));
    }

    private IEnumerator FadeInText(UnityEngine.UI.Text t)
    {
        float a = 0f;
        while (a < 1f)
        {
            a += Time.unscaledDeltaTime;
            t.color = new Color(1f, 1f, 1f, a);
            yield return new WaitForEndOfFrame();
        }
        t.color = new Color(1f, 1f, 1f, 1f);
    }

    private IEnumerator FadeInCanvasGroup(UnityEngine.CanvasGroup c)
    {
        float a = 0f;
        while (a < 1f)
        {
            a += Time.unscaledDeltaTime;
            c.alpha = a;
            yield return new WaitForEndOfFrame();
        }
        c.alpha = 1f;
    }

    private IEnumerator FadeInBG(UnityEngine.UI.Image i, float startA, float maxA, float timeScale)
    {
        float a = startA;
        while (a < maxA)
        {
            a += Time.unscaledDeltaTime * timeScale;
            i.color = new Color(0f, 0f, 0f, a);
            yield return new WaitForEndOfFrame();
        }
        i.color = new Color(0f, 0f, 0f, maxA);
    }

    private IEnumerator FillCompletionBar()
    {
        while (true)
        {
            bar.fillAmount = Mathf.Lerp(bar.fillAmount, ScoreManager.completion, 0.04f);
            float percentTextAnchor = Mathf.Clamp(bar.fillAmount, 0.066f, 1f);
            percentText.rectTransform.anchorMax = new Vector2(percentTextAnchor, percentText.rectTransform.anchorMax.y);
            percentText.text = string.Format("{0:P1}", bar.fillAmount);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FadeInStats()
    {
        foreach (UnityEngine.UI.Text t in stats)
        {
            StartCoroutine(FadeInText(t));
            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    public IEnumerator NextLevel()
    {
        yield return new WaitForEndOfFrame();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public CanvasGroup title;
    public UnityEngine.UI.Image rule;
    public CanvasGroup content;

    public UnityEngine.UI.Image darken;

    private bool ruleStart = false;
    private bool contentStart = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeFade();
    }

    void OnEnable()
    {
        InitializeFade();
    }

    void InitializeFade()
    {
        title.alpha = 0f;
        if (rule != null)
        {
            rule.fillAmount = 0f;
            ruleStart = false;
        }
        content.alpha = 0f;
        contentStart = false;

        StartCoroutine(FadeInTitle());
        if (darken != null)
        {
            StartCoroutine(FadeInDarken());
        }
    }

    IEnumerator FadeInTitle()
    {
        while (title.alpha < 1f)
        {
            title.alpha += Time.deltaTime;
            yield return new WaitForEndOfFrame();

            if (title.alpha > 0.5f)
            {
                if (rule != null && !ruleStart) 
                {
                    StartCoroutine(FadeInRule());
                    ruleStart = true;
                }
                else if (rule == null && !contentStart)
                {
                    StartCoroutine(FadeInContent());
                    contentStart = true;
                }
                
            }
        }
        title.alpha = 1f;
    }

    IEnumerator FadeInDarken()
    {
        float alpha = 0f;
        darken.color = new Color(0f, 0f, 0f, alpha);

        while (alpha < 0.75f)
        {
            alpha += Time.deltaTime * 0.4f;
            darken.color = new Color(0f, 0f, 0f, alpha);
            yield return new WaitForEndOfFrame();
        }
        alpha = 0.75f;
        darken.color = new Color(0f, 0f, 0f, alpha);
    }

    IEnumerator FadeInRule()
    {
        while (rule.fillAmount < 1f)
        {
            rule.fillAmount = Mathf.Lerp(rule.fillAmount, 1f, 0.05f);
            if (rule.fillAmount > 0.98f && !contentStart)
            {
                StartCoroutine(FadeInContent());
                contentStart = true;
            }

            yield return new WaitForSecondsRealtime(0.005f);
        }
    }

    IEnumerator FadeInContent()
    {
        while (content.alpha < 1f)
        {
            content.alpha += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        content.alpha = 1f;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

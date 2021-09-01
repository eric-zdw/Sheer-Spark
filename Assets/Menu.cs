using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public CanvasGroup title;
    public UnityEngine.UI.Image rule;
    public CanvasGroup content;

    public UnityEngine.UI.Image darken;

    // Start is called before the first frame update
    void Start()
    {
        title.alpha = 0f;
        if (rule != null)
        {
            rule.fillAmount = 0f;
        }
        content.alpha = 0f;

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
                if (rule != null) 
                {
                    StartCoroutine(FadeInRule());
                }
                else
                {
                    StartCoroutine(FadeInContent());
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
            rule.fillAmount = Mathf.Lerp(rule.fillAmount, 1f, 0.001f);
            if (rule.fillAmount > 0.95f)
            {
                StartCoroutine(FadeInContent());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeInContent()
    {
        while (content.alpha < 1f)
        {
            content.alpha += Time.deltaTime * 0.2f;
            yield return new WaitForEndOfFrame();
        }
        content.alpha = 1f;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

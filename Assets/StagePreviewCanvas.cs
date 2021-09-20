using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePreviewCanvas : MonoBehaviour
{
    private bool isDoneFading = false;
    public UnityEngine.UI.Image fadePanel;
    public float fadeDuration = 3f;
    public StageSelectMenu selectMenu;
    public UnityEngine.UI.Image loadingBar;
    public CanvasGroup content;
    public CanvasGroup title1;
    public CanvasGroup title2;
    public CanvasGroup buttons;

    private int currentSceneIndex;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //LoadStagePreviewAction();
    }

    public void LoadStagePreviewAction(string name)
    {
        foreach (StageEntry entry in selectMenu.stageEntries)
        {
            if (entry.StageName == name)
            {
                StartCoroutine(FadeToBlack());
                StartCoroutine(LoadStagePreview(entry.SceneIndex));
            }
            continue;
        }
    }

    private IEnumerator LoadingBar(AsyncOperation ao)
    {
        UnityEngine.UI.Text text = loadingBar.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        loadingBar.fillAmount = 0f;

        while (loadingBar.fillAmount < 1f)
        {
            if (ao.progress == 1f)
            {
                loadingBar.GetComponent<UnityEngine.UI.Button>().interactable = true;
                text.text = "START";
            }

            loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, ao.progress, 0.1f);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    private IEnumerator LoadStagePreview(int index)
    {
        AsyncOperation loadingStage = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        loadingStage.allowSceneActivation = false;
        StartCoroutine(LoadingBar(loadingStage));

        //wait for fadeout and scene loading to switch scenes.
        while (!isDoneFading)
        {
            yield return new WaitForSeconds(0.2f);
        }

        // done fading out; load rest of scene.
        loadingStage.allowSceneActivation = true;
        
        // wait for new scene to load before fadein
        while (SceneManager.GetActiveScene().buildIndex == currentSceneIndex)
        {
            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeToBlack()
    {
        title1.alpha = 0f;
        title2.alpha = 0f;
        buttons.alpha = 0f;

        isDoneFading = false;
        float timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float a = 1f - (timer / fadeDuration);
            fadePanel.color = new Color(0f, 0f, 0f, a);
            yield return new WaitForEndOfFrame();
        }

        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        StartCoroutine(FadeInTitle());
        //isDoneFading = true;
    }

    private IEnumerator FadeInTitle()
    {
        float timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float a = 1f - (timer / fadeDuration);
            title1.alpha = a;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(0.8f);
        timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float a = 1f - (timer / fadeDuration);
            title2.alpha = a;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(1.2f);
        fadePanel.color = new Color(0f, 0f, 0f, 1f);
        isDoneFading = true;

        timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float a = 1f - (timer / fadeDuration);
            buttons.alpha = a;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FadeIn()
    {
        float timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float a = (timer / fadeDuration);
            fadePanel.color = new Color(0f, 0f, 0f, a);
            yield return new WaitForEndOfFrame();
        }

        fadePanel.color = new Color(0f, 0f, 0f, 0f);
    }

    /*
    public IEnumerator ReturnToMenu()
    {
        StartCoroutine(FadeOut());
        if (isDoneFading)
    }
    */
}

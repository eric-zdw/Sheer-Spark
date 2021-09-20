using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutMenu : MonoBehaviour
{
    public string[] descriptions1;
    public string[] descriptions2;

    public UnityEngine.UI.Image loadoutButton1;
    public UnityEngine.UI.Image loadoutButton2;
    public CanvasGroup buttonPanel;

    public Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        SwitchColors(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchColors(int c)
    {
        StartCoroutine(ButtonPanelFadeIn());

        loadoutButton1.color = colors[c];
        loadoutButton2.color = colors[c];
        loadoutButton1.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = descriptions1[c];

        if (!SaveManager.saveData.weaponsUnlocked[c])
        {
            loadoutButton2.GetComponent<UnityEngine.UI.Button>().interactable = false;
            loadoutButton2.GetComponent<CanvasGroup>().alpha = 0.4f;
            loadoutButton2.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "LOCKED";
            loadoutButton2.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "";
        }
        else
        {
            loadoutButton2.GetComponent<UnityEngine.UI.Button>().interactable = true;
            loadoutButton2.GetComponent<CanvasGroup>().alpha = 1f;
            loadoutButton2.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "2";
            loadoutButton2.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = descriptions2[c];
        }
    }

    public void ChangePowerup1(int c)
    {

    }

    public void ChangePowerup2(int c)
    {

    }

    IEnumerator ButtonPanelFadeIn()
    {
        float alpha = 0f;
        buttonPanel.alpha = alpha;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * 4f;
            buttonPanel.alpha = alpha;

            yield return new WaitForEndOfFrame();
        }
        alpha = 1f;
    }
}

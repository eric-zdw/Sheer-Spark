using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        GameObject.FindGameObjectWithTag("WaveSystem").GetComponent<WaveSystem>().ActivatePlayer();
        transform.parent.parent.gameObject.SetActive(false);
    }
}

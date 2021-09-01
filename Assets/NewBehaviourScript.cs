using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int items = 1000;
    public int choose = 10;
    public int weeks = 10;

    void Start()
    {
        double totalprob = 0d;
        for (int i = 0; i < weeks; i++)
        {
            double div = 0d;
            for (int j = 1; j <= choose; j++)
            {
                div += 1d / (items - j);
            }
            totalprob += (1d - totalprob) * div;
            print("totalprob: " + totalprob);
        }
        print("final prob: " + totalprob);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

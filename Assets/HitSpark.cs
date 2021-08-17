using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSpark : MonoBehaviour
{
    public float gravityForce = 20f;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 localGravity;
        Destroy(this, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

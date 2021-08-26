using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterAlt : MonoBehaviour
{
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
        Destroy(GetComponent<SphereCollider>(), 0.2f);
        Camera.main.GetComponent<CameraFollow>().AddNoise(50f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (Vector3.Distance(transform.position, other.transform.position) <= 5f)
            {
                other.GetComponent<Enemy>().getDamage(600f);
                other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(other.transform.position - transform.position) * 30000f);
            }
            else if (Vector3.Distance(transform.position, other.transform.position) <= 10f)
            {
                other.GetComponent<Enemy>().getDamage(300f);
                other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(other.transform.position - transform.position) * 20000f);
            }
            else
            {
                other.GetComponent<Enemy>().getDamage(150f);
                other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(other.transform.position - transform.position) * 10000f);
            }
            
        }
    }
}

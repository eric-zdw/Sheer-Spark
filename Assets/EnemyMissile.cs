using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject explosionPrefab;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.Normalize(Random.insideUnitCircle) * Random.Range(20f, 40f), ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (WaveSystem.player != null)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.Normalize(WaveSystem.player.transform.position - transform.position) * 1000f * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        transform.GetChild(0).parent = null;
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeProjectileHitbox : MonoBehaviour {

	private float damage;
    private float radius;
    public float distanceFactor;

    public float explosionForce;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 0.04f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setDamage(float d)
    {
        damage = d;
    }

    public void setRadius(float r)
    {
        radius = r;
        GetComponent<SphereCollider>().radius = radius;
    }

    public void printRadius()
    {
        //print("radius: " + radius + ", damage: " + damage);
    }

    private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Enemy") {
            float distance = Vector3.Distance(other.ClosestPointOnBounds(transform.position), transform.position);

            //distance modifier reduces damage up to distfactor%. If hit, an enemy will always take at least (1 - distfactor)% damage.
            other.GetComponent<Enemy>().getDamage(damage * (1 - ((distance / radius) * distanceFactor)));
            if (other.GetComponent<Enemy>() is SmallEnemy)
            {
                Vector3 direction = other.transform.position - transform.position;
                GameObject hitSpark = other.GetComponent<SmallEnemy>().SpawnHitSpark(other.transform.position, Quaternion.identity, damage, 1.5f * damage);
                hitSpark.transform.right = other.transform.position - transform.position;
                hitSpark.transform.rotation *= Quaternion.Euler(0f, 0f, -90f);
            }
			//print("distance: " + distance + ", radius: " + radius + ", percentage: " + (1 - (distance / radius)) + ", damage: " + damage * (1 - ((distance / radius) * 0.5f)));

            other.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, radius, 0f);
        }
	}
}

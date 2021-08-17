using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterProjectile : Projectile {

    public GameObject explosion;
    public GameObject explosion2;

    // Use this for initialization
    void Start() {
        projectileSpeed = 90f;
        lifeTime = 10f;
        damage = 12f;
        GetComponent<Rigidbody>().AddForce(transform.right * 90f, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (lifeTime <= 0)
            Destroy(gameObject);
        else
        {
            //PropogateRigidbody();
            //CheckLinecastCollision();
            lifeTime -= Time.deltaTime;
        }
            
    }

    void CheckLinecastCollision() {
        RaycastHit info;
        if (Physics.Linecast(transform.position, transform.position + transform.right * projectileSpeed * Time.deltaTime, out info, PlayerBehaviour.projectileLayerMask)) {
            transform.position = info.point;
            if (info.collider.gameObject.CompareTag("Enemy")) {
                info.collider.gameObject.GetComponent<Enemy>().getDamage(damage);
            }
            Explode();
        }
        else
            transform.position += transform.right * projectileSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().getDamage(damage);
            if (collision.collider.GetComponent<Enemy>() is SmallEnemy)
                collision.collider.gameObject.GetComponent<SmallEnemy>().SpawnHitSpark(collision.GetContact(0).point, transform.rotation, damage, 15f);
        }
        Explode();
    }

    void Explode()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Instantiate(explosion2, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

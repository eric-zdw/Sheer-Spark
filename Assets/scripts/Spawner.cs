using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    private GameObject spawnTarget;
    public GameObject particles;
    public GameObject particles2;
    public float safeDistance;

    private float timer;
    private float animTime = 0.5f;
    private float animationTimer = 0.5f;
    private bool isSpawning;
    private PlayerBehaviour player;

    public bool isGrounded;
    public bool isBusy;

	// Use this for initialization
	void Start () {
        player = WaveSystem.player;

        if (Physics.Raycast(transform.position, Vector3.down, 4f)) {
            isGrounded = true;
        }
	}

    public bool SafeToSpawn() {
        if (player != null)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < safeDistance) {
                return false;
            }
            else {
                return true;
            }
        }
        else return false;
    }

    public IEnumerator Spawn(GameObject obj) {
        isBusy = true;
        spawnTarget = obj;
        isSpawning = true;
        float timer = 2f;
        Instantiate(particles, transform.position, Quaternion.identity);
        while (timer > 0f) {    
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Instantiate(particles2, transform.position, Quaternion.identity);
        Instantiate(spawnTarget, transform.position, Quaternion.identity);
        isBusy = false;
    }

    public void OnDrawGizmos() {
        if (isGrounded)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}

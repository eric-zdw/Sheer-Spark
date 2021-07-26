using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PurpleBoss : BossEnemy
{

	private GameObject player;
	private Vector3 playerLocation;
	private Rigidbody rb;

	public GameObject explosion1;
	public GameObject explosion2;

    public bool isTethered;
    private float tetheredCheck = 0.05f;
    private float tetheredTimer;

	public GameObject node;
	private List<GameObject> path;

	private List<List<NodeData>> nodeData;
	//private Dictionary<Tuple<int, int>, NodeData> nodeDictionary; 

	public float pathfindingStrictness = 2f;
	public int iterationsPerFrame = 8;
	public float neighbourAvoidanceWeight = 1f;
	public float neighbourAvoidanceRadius = 5f;

	private UnityEngine.UI.Image healthBarLeft;
	private UnityEngine.UI.Image healthBarRight;

    void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody>();

		StartCoroutine(CorrectRotation());
		StartCoroutine(GlowRoutine());
		//ActivateHealthBar();
    }

	void FixedUpdate()
	{
		/*
		if (path.Count > 0) {
			rb.AddForce(Vector3.Normalize(path[0].transform.position - transform.position) * 5000f * Time.deltaTime);
			if (Vector3.Distance(transform.position, path[0].transform.position) < pathfindingStrictness) {
				path.RemoveAt(0);
			}
		}
		*/
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerBehaviour>().takeDamage(1);
            getDamage(100);
        }
    }

    void Explode()
	{
		ScoreManager.IncreaseScore(scoreValue);
		Camera.main.GetComponent<CameraFollow>().AddNoise(10f);
		Instantiate(explosion1, transform.position, transform.rotation);
		Destroy(gameObject);
	}

	void ExplodeWithoutPowerup()
	{
		Camera.main.GetComponent<CameraFollow>().AddNoise(10f);
		Instantiate(explosion1, transform.position, transform.rotation);
		Destroy(gameObject);
	}

	private IEnumerator GlowRoutine() {
		while (true) {
			float emission = Mathf.Sin(Time.time * (Mathf.PI * 2f) * 0.4f) * 3f;
			//glowMatBlock.SetColor("_EmissionColor", new Color(0.75f, 0.25f, 1f) * emission);
			//GetComponent<MeshRenderer>().SetPropertyBlock(glowMatBlock, 1);
			yield return new WaitForEndOfFrame();
		}
	}

	private void ActivateHealthBar() {
		GameObject.Find("BossHealthPanel").GetComponent<UnityEngine.UI.Image>().enabled = true;
		GameObject.Find("BossTitle").GetComponent<UnityEngine.UI.Text>().enabled = true;
		healthBarLeft = GameObject.Find("BossHealthBarLeft").GetComponent<UnityEngine.UI.Image>();
		healthBarLeft.enabled = true;
		healthBarRight = GameObject.Find("BossHealthBarRight").GetComponent<UnityEngine.UI.Image>();
		healthBarRight.enabled = true;
		StartCoroutine(UpdateHealthBar());
	}

	private IEnumerator UpdateHealthBar() {
		while(true) {
			float oldFill = healthBarLeft.fillAmount;
			float newFill = health / maxHealth;
			healthBarLeft.fillAmount = Mathf.Lerp(oldFill, newFill, 0.1f);
			healthBarRight.fillAmount = Mathf.Lerp(oldFill, newFill, 0.1f);
			yield return new WaitForFixedUpdate();
		}
	}

	private IEnumerator CorrectRotation() {
		while (true) {
			if (transform.rotation.z != 0f) {
				rb.AddTorque(Vector3.forward * Time.deltaTime * 500f);
			}

			rb.AddTorque(Vector3.up * 5000f * Time.deltaTime);
			yield return new WaitForFixedUpdate();
		}
	}

	private void EmitLaser() {
		
	}
}


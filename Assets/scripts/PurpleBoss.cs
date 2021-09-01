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

	public GameObject missilePrefab;
	public GameObject beamPrefab;

	public GameObject node;
	private List<GameObject> path;

	private List<List<NodeData>> nodeData;
	//private Dictionary<Tuple<int, int>, NodeData> nodeDictionary; 

	public float pathfindingStrictness = 2f;
	public int iterationsPerFrame = 8;
	public float neighbourAvoidanceWeight = 1f;
	public float neighbourAvoidanceRadius = 5f;

	public GameObject explosionPrefab;
	public GameObject explosionBigPrefab;

	private UnityEngine.UI.Image healthBarLeft;
	private UnityEngine.UI.Image healthBarRight;

    void Start()
	{
		Initialize();
		player = GameObject.FindGameObjectWithTag("Player");
		rb = GetComponent<Rigidbody>();

		StartCoroutine(CorrectRotation());
		StartCoroutine(GlowRoutine());
		StartCoroutine(AttackSchedule());
		ActivateHealthBar();
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

	private IEnumerator AttackSchedule()
    {
        int healthState = 0;
		float attackTimer = 10f;

		while (true)
        {
			if (health >= maxHealth * 0.666f)
            {
				healthState = 0;
            }
			else if (health >= maxHealth * 0.333f)
            {
				healthState = 1;
            }
			else if (health >= 0f)
            {
				healthState = 2;
            }
			else
            {
				healthState = 3;
            }

			attackTimer -= Time.deltaTime;
			if (attackTimer <= 0f)
            {
				if (healthState == 0)
				{
					int choice = UnityEngine.Random.Range(0, 2);
					if (choice == 0)
                    {
						StartCoroutine(MissileAttack());
                    }
					else if (choice == 1)
					{
						StartCoroutine(BigBeamAttack());
					}

					attackTimer += 10f;
				}
				else if (healthState == 1)
                {
					int choice = UnityEngine.Random.Range(0, 2);
					if (choice == 0)
					{
						StartCoroutine(MissileAttack());
					}
					else if (choice == 1)
                    {
						StartCoroutine(BigBeamAttack());
                    }

					attackTimer += 8f;
				}
				else
				{
					int choice = UnityEngine.Random.Range(0, 2);
					if (choice == 0)
					{
						StartCoroutine(MissileAttack());
					}
					else if (choice == 1)
					{
						StartCoroutine(BigBeamAttack());
					}

					attackTimer += 6f;
                }
			}

			yield return new WaitForFixedUpdate();
        }
    }

	IEnumerator MissileAttack()
    {
		//spinup phase
		float timer = 3f;
		while (timer > 0f)
        {
			GetComponent<Rigidbody>().AddTorque(Vector3.up * 80000f * Time.deltaTime);
			timer -= Time.deltaTime;
			yield return new WaitForFixedUpdate();
        }

		//attack phase
		timer = 2f;
		float missileTimer = 0f;
		while (timer > 0f)
        {
			GetComponent<Rigidbody>().AddTorque(Vector3.up * 80000f * Time.deltaTime);
			if (missileTimer <= 0f)
            {
				Instantiate(missilePrefab, transform.position, Quaternion.identity);
				missileTimer += 0.2f;
			}
			timer -= Time.deltaTime;
			missileTimer -= Time.deltaTime;
			yield return new WaitForFixedUpdate();
        }
    }

	IEnumerator BigBeamAttack()
    {
		Instantiate(beamPrefab, transform.position, Quaternion.identity);
		yield return new WaitForFixedUpdate();
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

	protected override IEnumerator BossExplode()
    {
		int explosions = 32;
		float timer = 0f;
		while (explosions > 0)
        {
			timer -= Time.deltaTime;
			if (timer <= 0f)
            {
				if (UnityEngine.Random.Range(0, 5) == 0)
                {
					Instantiate(explosionBigPrefab, transform.position + Vector3.Normalize(UnityEngine.Random.insideUnitSphere) * 5.5f, Quaternion.identity);
					Camera.main.GetComponent<CameraFollow>().AddNoise(40f);
				}
				else
                {
					Instantiate(explosionPrefab, transform.position + Vector3.Normalize(UnityEngine.Random.insideUnitSphere) * 5.5f, Quaternion.identity);
					Camera.main.GetComponent<CameraFollow>().AddNoise(6f);
				}
				StartCoroutine(FlashWhite());
				explosions--;
				timer += 0.25f;
			}
			yield return new WaitForFixedUpdate();
		}
		for (int i = 0; i < 6; i++)
        {
			Instantiate(explosionBigPrefab, transform.position + Vector3.Normalize(UnityEngine.Random.insideUnitSphere) * 4f, Quaternion.identity);
		}
		Camera.main.GetComponent<CameraFollow>().AddNoise(100f);
		Destroy(gameObject);

	}
}


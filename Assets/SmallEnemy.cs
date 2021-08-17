using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class SmallEnemy : Enemy
{
    public SmallEnemyScriptableObject smallEnemyData;

    [SerializeField]
    protected float powerupChance = 0f;

    protected GameObject healthBar;
    protected int powerupRoll;
    
    private Color enemyColor;
    private Color enemyColorHDR;

    public MeshRenderer[] mainMeshes;
    public MeshRenderer[] outlineMeshes;
    public MeshRenderer[] seeThroughMeshes;
    public MeshRenderer[] damageFlashMeshes;

    private MaterialPropertyBlock mainMPB;
    private MaterialPropertyBlock seeThroughMPB;
    private MaterialPropertyBlock damageFlashMPB;

    private Rigidbody rb;

    protected bool isDelayedDeath = false;

    protected override void Initialize()
    {
        base.Initialize();
        //create health bar
        healthBar = Instantiate(smallEnemyData.healthBarPrefab);
        healthBar.GetComponent<HealthBar>().setTarget(gameObject);
        health = maxHealth;

        rb = GetComponent<Rigidbody>();

        powerupRoll = Random.Range(0, 6);
        enemyColor = smallEnemyData.powerupColors[powerupRoll];
        enemyColorHDR = smallEnemyData.powerupColorsHDR[powerupRoll];
        //todo: change MatBlock to color

        mainMPB = new MaterialPropertyBlock();
        mainMPB.SetColor("Color_3238E920", enemyColorHDR);
        foreach (MeshRenderer m in mainMeshes)
        {
            m.SetPropertyBlock(mainMPB);
        }
        
        foreach (MeshRenderer o in outlineMeshes)
        {
            o.material = smallEnemyData.outlines[powerupRoll];
        }
        
        foreach (MeshRenderer s in seeThroughMeshes)
        {
            s.material = smallEnemyData.seeThroughMats[powerupRoll];
        }

        damageFlashMPB = new MaterialPropertyBlock();
    }

    public override void getDamage(float damage) {
        //Check health previous to damage calculation to prevent duplicate deaths
        float oldHealth = health;
        if (oldHealth > 0f && !isDelayedDeath) {
            health -= damage;
            if (health <= 0f) {
                DefeatRoutine();
                //StartCoroutine(DelayedDeathRoutine());
            }
            else {
                StartCoroutine(FlashWhite());
            }
        }
    }

    public GameObject SpawnHitSpark(Vector3 pos, Quaternion rot, float damage, float spread)
    {
        UnityEngine.VFX.VisualEffect hitSpark = Instantiate(smallEnemyData.hitSparkPrefab, pos, rot * Quaternion.Euler(0f, 0f, -90f)).GetComponent< UnityEngine.VFX.VisualEffect>();
        //change properties according to damage and powerup type
        hitSpark.SetInt(Shader.PropertyToID("Number of Particles"), (int)(damage * Random.Range(1.6f, 2.4f)));
        hitSpark.SetGradient(Shader.PropertyToID("Color"), smallEnemyData.hitSparkGradients[powerupRoll]);
        hitSpark.SetFloat(Shader.PropertyToID("Spread"), spread * Random.Range(0.8f, 1.2f));
        hitSpark.SetVector2(Shader.PropertyToID("Magnitude"), new Vector2(damage * 0.05f, 40f + damage * 3f));
        hitSpark.SetVector2(Shader.PropertyToID("Lifetime"), new Vector2(damage * 0.005f, 0.6f + damage * 0.01f));
        return hitSpark.gameObject;
    }

    public override void setHealth(float h)
    {
        health = h;
        if (health <= 0) {
            DefeatRoutine();
        }
    }

    protected virtual void DefeatRoutine() {
        Destroy(healthBar);
        Camera.main.GetComponent<CameraFollow>().AddNoise(5f);
        Instantiate(smallEnemyData.powerups[powerupRoll], transform.position, Quaternion.identity);
		Instantiate(smallEnemyData.deathExplosions[powerupRoll], transform.position, transform.rotation);
        ScoreManager.IncreaseScore(scoreValue);
		Destroy(gameObject);
    }

	private IEnumerator DelayedDeathRoutine() {
        isDelayedDeath = true;
		float duration = 2.5f;
		Instantiate(smallEnemyData.delayedDeathPrefab, transform.position, Quaternion.identity);
		rb.AddForce(Vector3.up * 1000f);
        rb.useGravity = true;
		while (duration > 0f) {
			duration -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
        DefeatRoutine();
	}

    IEnumerator FlashWhite() {
		float colorValue = 3f;
		Color newColor = new Color(colorValue, colorValue, colorValue, 1);
		while (colorValue > 0f) {
			//print("colorValue: " + colorValue);
			colorValue -= 9f * Time.deltaTime;
			newColor = new Color(colorValue, colorValue, colorValue, 1);
			damageFlashMPB.SetColor("_EmissionColor", newColor);
            foreach (MeshRenderer d in damageFlashMeshes)
            {
                d.SetPropertyBlock(damageFlashMPB);
            }
			yield return new WaitForEndOfFrame();
		}
	}
}

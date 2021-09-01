using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BossEnemy : Enemy
{
    //public SmallEnemyScriptableObject smallEnemyData;

    [SerializeField]

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
        health = maxHealth;

        rb = GetComponent<Rigidbody>();
        //todo: change MatBlock to color

        mainMPB = new MaterialPropertyBlock();
        mainMPB.SetColor("Color_3238E920", enemyColorHDR);
        foreach (MeshRenderer m in mainMeshes)
        {
            m.SetPropertyBlock(mainMPB);
        }
        
        foreach (MeshRenderer o in outlineMeshes)
        {
            //o.material = smallEnemyData.outlines[powerupRoll];
        }
        
        foreach (MeshRenderer s in seeThroughMeshes)
        {
            //s.material = smallEnemyData.seeThroughMats[powerupRoll];
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

    public override void setHealth(float h)
    {
        health = h;
        if (health <= 0) {
            DefeatRoutine();
        }
    }

    protected virtual void DefeatRoutine() {
        //Camera.main.GetComponent<CameraFollow>().AddNoise(5f);
        //Instantiate(smallEnemyData.powerups[powerupRoll], transform.position, Quaternion.identity);
		//Instantiate(smallEnemyData.deathExplosions[powerupRoll], transform.position, transform.rotation);
        //ScoreManager.IncreaseScore(scoreValue);
        StartCoroutine(BossExplode());
    }

    protected abstract IEnumerator BossExplode();

	private IEnumerator DelayedDeathRoutine() {
        isDelayedDeath = true;
		float duration = 2.5f;
		//Instantiate(smallEnemyData.delayedDeathPrefab, transform.position, Quaternion.identity);
		rb.AddForce(Vector3.up * 1000f);
        rb.useGravity = true;
		while (duration > 0f) {
			duration -= Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
        DefeatRoutine();
	}

    protected IEnumerator FlashWhite() {
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

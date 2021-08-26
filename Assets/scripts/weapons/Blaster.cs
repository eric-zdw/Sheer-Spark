using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : Weapon {

    public GameObject projectile;
    public GameObject projectile2;
    public float damage = 12f;
    private Camera cam;
    private Vector2 mousePosition;
    private float angle;
    public float bFireRate = 0.18f;
    public float secondaryRate = 0.90f;

    public float maxHeatDamageMulti = 1f;
    public float maxHeatFireRateMulti = 1f;
    //public float maxHeatRadiusMulti = 1f;

    void Start () {
        //SetFireRate(bFireRate);
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if (GetCooldown() > 0)
            DecrementCooldown();

        mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(CameraFollow.CameraDistance)));
        angle = Mathf.Atan2(mousePosition.y - transform.position.y, mousePosition.x - transform.position.x) * Mathf.Rad2Deg;
    }

    public override void Fire1()
    {
        if (GetCooldown() <= 0)
        {
            float realDamage = damage * (1f + (maxHeatDamageMulti * WaveSystem.player.GetHeatFactor(EnergyColor.White)));
            Instantiate(projectile, transform.position + (Vector3.Normalize((Vector3)mousePosition - transform.position) * 0.5f), Quaternion.Euler(0, 0, angle + Random.Range(-1f, 1f)));
            SetCooldown(bFireRate / (1f + (maxHeatFireRateMulti * WaveSystem.player.GetHeatFactor(EnergyColor.White))));
        }
    }

    public override void Fire2()
    {
        bool hasEnergy = true;
        for (int i = 0; i < 6; i++)
        {
            if (WaveSystem.player.powerupEnergy[i] < 2)
            {
                hasEnergy = false;
            }
        }

        if (GetCooldown() <= 0 && hasEnergy)
        {
            float realDamage = damage * (1f + (maxHeatDamageMulti * WaveSystem.player.GetHeatFactor(EnergyColor.White)));
            Instantiate(projectile2, transform.position, Quaternion.identity);
            for (int i = 0; i < 6; i++)
            {
                WaveSystem.player.powerupEnergy[i] -= 3;
                WaveSystem.player.energyPanel.UpdateEnergyMeters();
            }
            StartCoroutine(Freeze());
            SetCooldown(secondaryRate / (1f + (maxHeatFireRateMulti * WaveSystem.player.GetHeatFactor(EnergyColor.White))));
        }
    }

    public IEnumerator Freeze()
    {
        float timer = 0.6f;

        while (timer > 0f)
        {
            WaveSystem.player.GetComponent<Rigidbody>().velocity *= 0f;
            timer -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}

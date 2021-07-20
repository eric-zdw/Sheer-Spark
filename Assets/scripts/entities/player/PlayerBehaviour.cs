using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnergyColor {Red, Orange, Yellow, Green, Blue, Purple, White};
public class PlayerBehaviour : MonoBehaviour {
    public float playerBaseSpeed = 25f;
    public float playerHeatSpeed = 25f;
    public float verticalSpeed;
    private float playerSpeed;

    public float dragForceX = 0.025f;
    public float dragForceY = 0.01f;
    public float maxAngularVelocity = 15f;
    public float torqueStrength = 0.5f;
    // public float jumpStrength = 5f;
    public float maxHP = 3;
    private Rigidbody rb;

    private GameObject weaponSlot;
    private Weapon weapon;
    private Utility util;
    private GameObject utilitySlot;

    public int[] powerupEnergy;
    public int maxPowerupEnergy = 6;
    public static int totalPowerups = 0;

    private MeshRenderer mesh;
    public Material defaultColour;

    public GameObject defaultWeapon;
    public float powerupDuration;
    public float powerupIncrement;
    public float powerupTimer;
    public float powerupHealth;
    public bool hasPowerup;
    public float HP;
    public EnergyPanel energyPanel;

    private AudioSource collisionSound;

    public GameObject deathExplosion;
    public Color[] powerColors;
    private GameObject[] powerups;
    private RectTransform pbSize;

    public GameObject playerUI;
    private PowerupRadial radialBar;
    private PlayerHealthBar healthBar;
    public ParticleSystemRenderer lightTrail;
    public Material[] trailMaterials;

    private MaterialPropertyBlock matBlock;
    public GameObject[] weaponsList;

    private DamageAlert damageAlert;

    private GameObject inside;
    private GameObject inside2;
    private GameObject inside3;

    public float invincible = 0f;
    public bool isDashing = false;

    public AudioSource damageClip;

    public bool localGravity = false;

    public static int projectileLayerMask = ~((1 << 9) | (1 << 13) | (1 << 8) | (1 << 14) | (1 << 18) | (1 << 21));

    public bool isReversed;

    public GameObject playerSpawnPart;

    // Use this for initialization
    void Start()
    {
        playerSpeed = playerBaseSpeed;

        rb = GetComponent<Rigidbody>();
        weaponSlot = GameObject.Find("WeaponSlot");
        utilitySlot = GameObject.Find("UtilitySlot");

        weapon = weaponSlot.GetComponentInChildren<Weapon>();
        util = utilitySlot.GetComponentInChildren<Utility>();
        
        powerupEnergy = new int[6];
        totalPowerups = 0;

        mesh = GetComponent<MeshRenderer>();

        rb.maxAngularVelocity = maxAngularVelocity;
        HP = maxHP;

        collisionSound = GetComponent<AudioSource>();

        radialBar = playerUI.GetComponentInChildren<PowerupRadial>();
        healthBar = playerUI.GetComponentInChildren<PlayerHealthBar>();
        matBlock = new MaterialPropertyBlock();
        damageAlert = GameObject.Find("DamageAlert").GetComponent<DamageAlert>();

        powerups = new GameObject[6];

        inside = GameObject.Find("Inside");
        inside2 = GameObject.Find("Inside2");
        inside3 = GameObject.Find("Inside3");

        transform.position = GameObject.Find("PlayerSpawn").transform.position;

        if (localGravity) {
            rb.useGravity = false;
        }

        Camera.main.GetComponent<CameraFollow>().followTarget = this.gameObject;
        Instantiate(playerSpawnPart, transform.position, Quaternion.identity);
    }

    void Update()
    {
        Fire();

        if (powerupTimer >= 0f)
        {
            powerupTimer -= Time.deltaTime;
            if (pbSize != null)
            {
                pbSize.localScale = new Vector3(powerupTimer / powerupDuration, 1f, 1f);
            }

            if (powerupTimer <= 0f)
            {
                hasPowerup = false;
                Destroy(weapon.gameObject);
                weapon = Instantiate(defaultWeapon, transform.position, Quaternion.identity, weaponSlot.transform).GetComponent<Weapon>();
                mesh.material = defaultColour;
                inside.GetComponent<MeshRenderer>().material = defaultColour;
                inside2.GetComponent<MeshRenderer>().material = defaultColour;
                inside3.GetComponent<MeshRenderer>().material = defaultColour;
                GameObject.FindGameObjectWithTag("PlayerLight").GetComponent<Light>().color = new Color(0.549f, 0.608f, 0.678f);
                //powerupBar.GetComponent<UnityEngine.UI.Image>().material = defaultColour;
                //powerupName.text = "BLASTER";
                lightTrail.material = trailMaterials[0];
            }
        }

        playerSpeed = playerBaseSpeed;

        if (invincible > 0f) {
            invincible -= Time.deltaTime;
        }
    }

    void FixedUpdate () {
        Movement();

        if (!isDashing) {
            rb.AddForce(new Vector3(-rb.velocity.x, 0, 0) * dragForceX * Time.deltaTime);
            rb.AddForce(new Vector3(0, -rb.velocity.y, 0) * dragForceY * Time.deltaTime);
        }
    }

    void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (isReversed) {
            horizontal *= -1f;
        }

        float speedMultiplier = 1f;

        Vector3 movement = new Vector3(horizontal, 0f, 0f);
        if (horizontal < -0.5f && rb.velocity.x > -4f)
            speedMultiplier *= 1.75f;
        else if (horizontal > 0.5f && rb.velocity.x < 4f)
            speedMultiplier *= 1.75f;
        
        rb.AddForce(movement * playerSpeed * speedMultiplier * Time.deltaTime);
        rb.AddTorque(0, 0, -horizontal * torqueStrength * Time.deltaTime);

        float vertical = Input.GetAxis("Vertical");
        Vector3 verticalMovement = new Vector3(0f, vertical, 0f);
        if (vertical < 0f)
            rb.AddForce(verticalMovement * verticalSpeed * Time.deltaTime);
    }

    void Fire()
    {
        if (Input.GetButton("Fire2") && Input.GetButton("Fire1"))
        {
            weapon.Fire2();
        }
        else if (Input.GetButton("Fire1"))
        {
            weapon.Fire1();
        }
        
        if (Input.GetButtonDown("Fire3"))
        {
            util.Activate();
        }
    }

    public void getPowerup(GameObject newWeapon, Material newColour, int index, Material newMaterial, string powerName)
    {
        hasPowerup = true;
        ScoreManager.IncreaseScore(50);
        ScoreManager.IncreaseMultiplier(1f);
        weapon = newWeapon.GetComponent<Weapon>();
        mesh.material = newColour;
        inside.GetComponent<MeshRenderer>().material = newColour;
        inside2.GetComponent<MeshRenderer>().material = newColour;
        inside3.GetComponent<MeshRenderer>().material = newColour;

        powerupTimer = Mathf.Clamp(powerupTimer + powerupIncrement, 0f, powerupDuration);
        GameObject.FindGameObjectWithTag("PlayerLight").GetComponent<Light>().color = powerColors[index];

        powerupEnergy[index] = Mathf.Clamp(powerupEnergy[index] + 1, 0, maxPowerupEnergy);
        energyPanel.UpdateEnergyMeters();
        totalPowerups++;

        Color radialColor = new Color(powerColors[index].r, powerColors[index].g, powerColors[index].b, 0.25f);
        radialBar.GetComponent<UnityEngine.UI.Image>().color = (powerColors[index]);
        //offset for default trail color
        lightTrail.material = trailMaterials[index + 1];

        if (HP < maxHP) {
            StartCoroutine(healthBar.Flash());
        }

        HP = Mathf.Clamp(HP + powerupHealth, 0f, maxHP);
    }

    public void addRecoil(Vector3 direction)
    {
        rb.AddForce(direction);
    }

    public void takeDamage(int damage)
    {
        //death condition: player has no health and takes more damage.
        damageClip.Play();

        if (damage > 0f && HP == 0f) {
            Camera.main.GetComponent<CameraFollow>().AddNoise(80f);
            Instantiate(deathExplosion, transform.position, Quaternion.identity);

            // Don't go to lose state if the game is already finished.
            if (WaveSystem.gameState != GameState.Postgame)
            {
                WaveSystem.gameState = GameState.Defeated;
                WaveSystem.retried = true;
            }

            Destroy(gameObject);
        }
        else {
            HP = Mathf.Max(0f, HP - damage);
		    Camera.main.GetComponent<CameraFollow>().AddNoise(25f);
        }

        invincible = 2f;

        StartCoroutine(healthBar.Flash());
        damageAlert.Flash(new Color(1f, 0f, 0f));
    }

    public void OnCollisionEnter(Collision collision)
    {
        collisionSound.volume = collision.relativeVelocity.magnitude * 0.05f;
        collisionSound.Play();
    }

    public float GetHeatFactor(EnergyColor color) {
        //special case for color 6, which is white
        if (color == EnergyColor.White) {
            int totalEnergy = 0;
            foreach (int p in powerupEnergy) {
                totalEnergy += p;
            }
            return (float)totalEnergy / (float)(maxPowerupEnergy * 6);
        }
        else {
            return (float)powerupEnergy[(int)color] / (float)maxPowerupEnergy;
        }
        
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class IListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

// Preview: In Stage selection menu, with scene as background. Player goes to pregame via menu.
// Pregame: Player character is active, before enemies have spawned. Player goes to Playing via activation powerup.
// Playing: Enemies start spawning. Goes to postgame after clearing last wave.
// Postgame: Level is complete. Goes to Switching after: a) new weapon is collected; b) automatically after period of time.
// Switching: End game effects; In the process of switching scenes.
public enum GameState { Preview, Pregame, Playing, Postgame, Switching, Defeated};

public class WaveSystem : MonoBehaviour {

    public static GameState gameState = GameState.Preview;
    public static bool retried = false;
    public WaveEntry[] waveEntries;
	public int waveNumber;
    public static float enemyPower = 1f;
    public float enemyPowerIncreasePerWave = 0.08f;
    public float easyPowerMultiplier = 0.5f;
    public float normalPowerMultiplier = 1f;
    public float hardPowerMultiplier = 2f;
    public bool hasEndLevelPowerup = false;
    public GameObject endLevelPowerup;
    public GameObject endLevelPowerupMarker;
    bool ppChange = false;
    public int currentStage;

    private List<EnemyEntry> enemyList;
    private List<EnemyEntry> infiniteEnemyList;
    public int enemiesInLimbo = 0;

    bool activeLevel = false;
    bool started = false;

    private List<GameObject> spawners;
    private List<Spawner> groundSpawners;
    private List<Spawner> airSpawners;
    private List<Spawner> spawnerScripts;

    public PlayerBehaviour playerPrefab;
    public static PlayerBehaviour player;
    private bool highIntensity = false;

    public bool gameStarted = false;
    public WaveComplete wc;
    public WaveComplete2 wc2;

    public PPManager ppManager;
    public SaveManager saveManager;
    public MusicManager musicManager;

    public bool gameFinished;
    private bool resetStarted = false;

    public static bool isPaused = false;

    private float savedTimeScale = 1.5f;

    public GameObject pauseMenu;
    public GameObject gameUI;
    public GameObject playerSpaceUI;

    private int activeEnemies = 0;

    // Use this for initialization
    void Start () {
        gameState = GameState.Preview;
        WaveSystem.isPaused = false;

        player = playerPrefab;
        
        waveNumber = 0;
        enemyPower = 1f;

        enemyList = new List<EnemyEntry>();
        infiniteEnemyList = new List<EnemyEntry>();
        InitializeEnemyQueue();

        InitializeSpawners();

        // what is this for again
        //currentStage = PlayerPrefs.GetInt("ActiveStage");

        //TEST
        //ActivatePlayer();

        // Go straight to PreGame if retrying stage.
        if (retried)
        {
            ActivatePlayer();
        }
    }

    private void InitializeEnemyQueue() {
        foreach (EnemyData ed in waveEntries[waveNumber].Enemies) {
            if (ed.EnemyCount >= 0) {
                for (int i = 0; i < ed.EnemyCount; i++) {
                    enemyList.Add(ed.Entry);
                }
            }
            //if EnemyCount < 0, add enemy to infinite spawn queue
            else {
                infiniteEnemyList.Add(ed.Entry);
            }        
        }
        //randomize enemies if needed
        //Fisher-Yates shuffle
        if (!waveEntries[waveNumber].SpawnInOrder) {
            enemyList.Shuffle();
        }
    }

    public void ActivatePlayer()
    {
        gameState = GameState.Pregame;
        player.gameObject.SetActive(true);
        StartCoroutine(GameUIFadeIn());
        playerSpaceUI.SetActive(true);
        StartCoroutine(SpawnRoutine());
    }

    public static IEnumerator ResetSceneRoutine()
    {
        float resetTimer = 5f;

        print("about to reset...");

        while (resetTimer > 0f)
        {
            resetTimer -= Time.deltaTime;
            print(resetTimer);
            yield return new WaitForEndOfFrame();
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        print("WORKING?!?!");
        while (!load.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator SpawnRoutine() {
        //wait for game start
        while (!gameStarted) {
            yield return new WaitForEndOfFrame();
        }

        //play start music
        musicManager.StartMusic();
        StartCoroutine(musicManager.ChangeMusic(0));
        //StartCoroutine(ppManager.SlowDown());
        //StartCoroutine(ppManager.ChangePP());

        while (true) {
            //spawn initial enemies
            for (int i = 0; i < waveEntries[waveNumber].InitialSpawns; i++) {
                Spawn();
            }
            yield return new WaitForSeconds(waveEntries[waveNumber].SpawnRate);

            //main spawn routine
            while (enemyList.Count > 0) {
                //if at maximum allowed active enemies, wait until enemy is destroyed
                while (activeEnemies > waveEntries[waveNumber].MaxActiveEnemies) {
                    yield return new WaitForEndOfFrame();
                }

                Spawn();

                yield return new WaitForSeconds(waveEntries[waveNumber].SpawnRate);
            }

            //stay until enemyList is reinitialized
            while (enemyList.Count == 0) {
                yield return new WaitForEndOfFrame();
            }

            // brief wait before next wave
            yield return new WaitForSeconds(1.5f);
        }
    }

    void Spawn()
    {
        List<Spawner> availableSpawners = new List<Spawner>();
        for (int i = 0; i < spawners.Count; i++)
        {
            print(spawnerScripts[i].SafeToSpawn());
            if (spawnerScripts[i].SafeToSpawn() == true)
            {
                availableSpawners.Add(spawnerScripts[i]);
            }
        }

        print(spawnerScripts.Count);

        int rngSpawner = Random.Range(0, availableSpawners.Count);
        StartCoroutine(availableSpawners[rngSpawner].Spawn(enemyList[0].Object));
        StartCoroutine(EnemyInLimbo());
        enemyList.RemoveAt(0);
    }

    // Enemy is in limbo when enemy is called for spawning but not spawned yet.
    // Counts towards enemy count when deciding if wave is finished.
    private IEnumerator EnemyInLimbo() {
        enemiesInLimbo++;
        yield return new WaitForSeconds(2.5f);
        enemiesInLimbo--;
    }

    void InitializeSpawners()
    {
        spawners = new List<GameObject>();
        spawners.AddRange(GameObject.FindGameObjectsWithTag("Spawner"));
        spawnerScripts = new List<Spawner>();
        groundSpawners = new List<Spawner>();
        airSpawners = new List<Spawner>();
        for (int i = 0; i < spawners.Count; i++)
        {
            spawnerScripts.Add(spawners[i].GetComponent<Spawner>());
        }

        foreach (Spawner s in spawnerScripts) {
            if (s.isGrounded) {
                groundSpawners.Add(s);
            }
            else {
                airSpawners.Add(s);
            }
        }

        print("spawners: " + spawners.Count);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused) PauseGame();
            else UnpauseGame();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ActivatePlayer();
        }

        activeEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        int remainingEnemies = activeEnemies + enemiesInLimbo + enemyList.Count;

        if (gameStarted)
        {
            if (remainingEnemies == 0 && gameFinished == false)
                IncrementWave();
        }

        if (gameState == GameState.Defeated && resetStarted == false)
        {
            resetStarted = true;
            StartCoroutine(ResetSceneRoutine());
        }
    }

    void IncrementWave()
    {
        waveNumber++;
        if (waveNumber == waveEntries.Length)
        {
            StartCoroutine(musicManager.ChangeMusic(5));
            //Game is done. Increment stats and end game.
            StartCoroutine(YouWin());
        }
        else {
            if (waveNumber == 1) {
                StartCoroutine(musicManager.ChangeMusic(1));
            }
            else if (waveNumber == 3) {
                StartCoroutine(musicManager.ChangeMusic(2));
            }
            else if (waveNumber == 4) {
                StartCoroutine(musicManager.ChangeMusic(3));
            }
            else if (waveNumber == 6) {
                StartCoroutine(musicManager.ChangeMusic(4));
            }
            //Start slow down and post-processing effects.
            StartCoroutine(ppManager.SlowDown());
            StartCoroutine(ppManager.ChangePP());
            //TODO: add music sting, or cut music entirely after completion.            

            InitializeEnemyQueue();
            //StartCoroutine(wc.WaveCompleteRoutine());
            wc2.StartRoutine();
        }

        
    }

    public IEnumerator YouWin() {
        gameFinished = true; 
        gameState = GameState.Postgame;
        SaveManager.saveData.levelsClearedOnNormal.Add(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        SaveManager.WriteToFile(SaveManager.saveData);

        if (hasEndLevelPowerup)
        {
            yield return new WaitForSeconds(3f);
            Instantiate(endLevelPowerup, endLevelPowerupMarker.transform.position, Quaternion.identity);
        }
        else
        {
            StartCoroutine(WinPickup());
        }
    }

    public IEnumerator WinPickup()
    {
        gameState = GameState.Switching;
        StartCoroutine(GameUIFadeOut());
        StartCoroutine(ppManager.GameEndEffects());
        //wc.LevelCompleteRoutine();
        yield return new WaitForSecondsRealtime(10f);
        ReturnToMenu();
    }

    public void ReturnToMenu()
    {
        PPManager.ResetTimeScale();
        SceneManager.LoadScene(0);
    }

    public void PauseGame() {
        if (gameState == GameState.Pregame || gameState == GameState.Playing) {
            WaveSystem.isPaused = true;
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            StartCoroutine(PauseFadeIn());
            //pauseMenu.SetActive(true);
            gameUI.SetActive(false);
        }
    }

    public void UnpauseGame() {
        WaveSystem.isPaused = false;
        Time.timeScale = savedTimeScale;
        pauseMenu.SetActive(false);
        StartCoroutine(GameUIFadeIn());
        //gameUI.SetActive(true);
    }

    public IEnumerator PauseFadeIn()
    {
        float a = 0f;
        pauseMenu.GetComponent<UnityEngine.CanvasGroup>().alpha = a;
        pauseMenu.SetActive(true);
        
        while (a < 1f)
        {
            a += Time.unscaledDeltaTime * 10f;
            pauseMenu.GetComponent<UnityEngine.CanvasGroup>().alpha = a;
            yield return new WaitForEndOfFrame();
        }

        pauseMenu.GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;
    }

    public IEnumerator GameUIFadeIn()
    {
        float a = 0f;
        gameUI.GetComponent<UnityEngine.CanvasGroup>().alpha = a;
        gameUI.SetActive(true);

        while (a < 1f)
        {
            a += Time.unscaledDeltaTime * 10f;
            gameUI.GetComponent<UnityEngine.CanvasGroup>().alpha = a;
            yield return new WaitForEndOfFrame();
        }

        gameUI.GetComponent<UnityEngine.CanvasGroup>().alpha = 1f;
    }
    public IEnumerator GameUIFadeOut()
    {
        float a = 1f;
        gameUI.GetComponent<UnityEngine.CanvasGroup>().alpha = a;
        gameUI.SetActive(true);

        while (a > 0f)
        {
            a -= Time.unscaledDeltaTime * 10f;
            gameUI.GetComponent<UnityEngine.CanvasGroup>().alpha = a;
            yield return new WaitForEndOfFrame();
        }

        gameUI.GetComponent<UnityEngine.CanvasGroup>().alpha = 0f;
    }
}

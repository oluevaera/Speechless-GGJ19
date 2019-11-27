using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float delayNextEnemy = 3;
    public float delayNextGroup = 5;
    public float delayNextWave = 2;
    public GameObject enemy;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject nwCorner;
    public GameObject seCorner;

    //Just putting a wave variable here for now, might be getting this from somewhere else though.
    public int wave = 1;
    private float groupDelay = 5;
    private bool spawnerActive = false; // REMEMBER TO KEEP THIS FALSE WHEN COMMITTING!
    private int groupsPerWave = 2;
    private GameObject spawnThisEnemy;
    private int waveSpawnBuffer = 1;
    private int spawnBufferCounter = 3;

    //Put in the co-ordinates of the boundaries of where enemies will spawn.
    private Vector3 nwBoundary;
    private Vector3 seBoundary;
    private float xSpawnLine, zSpawnLine;

    //Lights that dim as the player loses words.
    [SerializeField]
	private Light[] windowLights;

    private void Start() 
    {
        nwBoundary = nwCorner.transform.position;
        seBoundary = seCorner.transform.position;
        spawnThisEnemy = enemy;

        EventManager.AddListener("START_GAME", OnStartGame);
        EventManager.AddListener("END_GAME", OnEndGame);
        EventManager.AddListener("PAUSE_GAME", OnPauseGame);
        EventManager.AddListener("RESUME_GAME", OnResumeGame);
    }

    private void OnStartGame()
    {
        SpawnerOn();
    }

    private void OnEndGame()
    {
        // Reset everything
        StopAllCoroutines();
        SpawnerOff();
        wave = 1;
        groupsPerWave = 2;
        groupDelay = 5;
    }

    private void OnPauseGame()
    {

    }

    private void OnResumeGame()
    {

    }

    void SpawnerOn() { spawnerActive = true; }
    void SpawnerOff() { spawnerActive = false; }
    void SpawnEnemies(){ StartCoroutine("SpawnWave"); }

    IEnumerator SpawnGroup()
    {
        int r = Random.Range(1, 5);

        for (int i = 0; i < wave * waveSpawnBuffer; i++)
        {
            // Pause spawning whilst game is paused
            if (GameManager.Instance.GetState() == GameManager.GameState.Paused)
            {
                yield return new WaitForResumeGame();
            }

            switch (r)
            {
                case 1:
                    xSpawnLine = Random.Range(nwBoundary.x, nwBoundary.x * -1);
                    zSpawnLine = nwBoundary.z;
                    break;

                case 2:
                    xSpawnLine = seBoundary.x;
                    zSpawnLine = Random.Range(seBoundary.z, seBoundary.z * -1);
                    break;

                case 3:
                    xSpawnLine = Random.Range(seBoundary.x, seBoundary.x * -1);
                    zSpawnLine = seBoundary.z;
                    break;

                case 4:
                    xSpawnLine = nwBoundary.x;
                    zSpawnLine = Random.Range(nwBoundary.z, nwBoundary.z * -1);
                    break;

                default:
                    print("ERROR: Random Range in EnemySpawner is not between 1-4!");
                    break;
            }

            if (wave > 6)
            {
                r = Random.Range(1, 4);
                switch (r)
                {
                    case 1:
                        spawnThisEnemy = enemy;
                        break;
                    case 2:
                        spawnThisEnemy = enemy2;
                        break;
                    case 3:
                        spawnThisEnemy = enemy3;
                        break;
                    default:
                        spawnThisEnemy = enemy;
                        break;
                }
            }
            else if (wave > 3)
            {
                r = Random.Range(1, 3);
                switch (r)
                {
                    case 1:
                        spawnThisEnemy = enemy;
                        break;
                    case 2:
                        spawnThisEnemy = enemy2;
                        break;
                    default:
                        spawnThisEnemy = enemy;
                        break;
                }
            }

            GameObject e = Instantiate(spawnThisEnemy, new Vector3(xSpawnLine, 0, zSpawnLine), Quaternion.identity);
            Enemy.enemies.Add(e.GetComponent<Enemy>());

            // Pause spawning whilst game is paused
            if (GameManager.Instance.GetState() == GameManager.GameState.Paused)
            {
                yield return new WaitForResumeGame();
            }

            yield return new WaitForSeconds(delayNextEnemy);
        }
    }

    IEnumerator SpawnWave()
    {
        while (Enemy.enemies.Count != 0)
        {
            yield return new WaitForSeconds(delayNextWave);
        }

        // If wave is greater then one and we're here, it means we just completed last wave
        if (wave > 1) 
        {
            EventManager.PostEvent("WAVE_COMPLETE");
        }

        groupDelay = delayNextGroup - (float)wave / 20;
        if (groupDelay < 1) { groupDelay = 1; }

        for (int i = 0; i < groupsPerWave; i++)
        {
            StartCoroutine("SpawnGroup");
            // Pause spawning whilst game is paused
            if (GameManager.Instance.GetState() == GameManager.GameState.Paused)
            {
                yield return new WaitForResumeGame();
            }

            yield return new WaitForSeconds(groupDelay);
        }
        //Increase Wave Count and groups etc.
        wave++;
        if (wave % 5 == 0)
        {
            if (groupsPerWave < 3) { groupsPerWave++; }
            waveSpawnBuffer =  spawnBufferCounter / 2;
            spawnBufferCounter++;
            if (wave % 10 != 0)
            {
                if (delayNextEnemy >= 1.5f && delayNextEnemy <= 2) { delayNextEnemy -= 0.2f; }
                else if (delayNextEnemy >= 2) { delayNextEnemy -= 0.5f; }
                else if (delayNextGroup >= 1) { delayNextGroup -= 0.2f; }
            }
        }
        SpawnerOn();
    }


    void Awake()
    {
        Enemy.enemies = new List<Enemy>();
    }

    void Update()
    {
        //If spawnerActive is turned on, we want to turn it off so it only spawns a group of enemies once, before turning on again later.
        if (spawnerActive)
        {
            SpawnerOff();
            SpawnEnemies();
        }

        //Refresh all the lights' brightnesses.
        int intensity = GameObject.FindGameObjectsWithTag("Word").Length;
		foreach(Light l in windowLights) {
			l.intensity = (intensity == 0 ? 0 : 1 + ((float)intensity * 2.0f / 3.0f));
		}
    }
}

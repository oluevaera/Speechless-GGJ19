using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	/* Enemy CLASS
	 * Contains a static object pool
	 * of all the enemies in the game.
	 *
	 * Also handles damage dealing etc.
	 *
	 * -Kieran
	 * */

	static public List<Enemy> enemies;

	[SerializeField]
	private float maxHP = 10;

    public ParticleSystem hitParticles;

    public GameObject particles;
    public bool hasWord;
    bool reachHouse;
    public bool isWord;
    bool closeWall;
    bool playedStolen;
    bool isBaked;
    public GameObject[] words;
    public GameObject[] walls;
    public GameObject[] wordsHeld;
    GameObject[] wordCount;
    GameObject Player;
    GameObject house;
    public NavMeshAgent enemy;
    //NavMeshSurface surface;
    NavMeshObstacle houseO;
    GameObject surface1;
    private float[] values;
    float min;
    int minIndex;
    float minW;
    int minIndexW;
    float minH;
    int minIndexH;
    Vector3 spawnPoint;
    Vector3 txtOffset;
    static public int wordsLeft;
    float oriDist;
    AudioSource lostWordS;//ADDED NOW
    public AudioClip lostWord;//ADDED NOW
    public AudioClip stolenWord;//ADDED NOW
    private bool shouldUpdate = true; // Whether the enemy should do processing on it's update

    private float _hp;
	private float hp {
		get { return _hp; }
		set {
			_hp = value;

            if (!hitParticles.isPlaying)
            {
                hitParticles.Play();
            }

			//If the enemy is taken
			//to 0 HP, despawn it.
			if(_hp <= 0 && hasWord == true) {
                hasWord = false; // enemy stops holding the word
                txtOffset = new Vector3(transform.position.x, 1, transform.position.z); // text drops on the floor
                words[minIndex].transform.position = txtOffset;
                words[minIndex].tag = "Word"; // Tag becomes Word so that other enemies can search for it
                _hp = 0;

				Despawn();
			}
            else if (_hp <= 0 && hasWord == false)
            {
                _hp = 0;
                Despawn();
            }
        }
	}

    void Start()
    {
        lostWordS = GetComponent<AudioSource>();
        playedStolen = false;
        isBaked = false;
        closeWall = false;
        isWord = true;
        Player = GameObject.FindGameObjectWithTag("Player");
        house = GameObject.FindGameObjectWithTag("House");
        //surface = FindObjectOfType<NavMeshSurface>();
        houseO = FindObjectOfType<NavMeshObstacle>();
        Respawn();
        
    }

    private void OnEnable() {
        EventManager.AddListener("PAUSE_GAME", OnPauseGame);
        EventManager.AddListener("RESUME_GAME", OnResumeGame);
    }

    private void OnDisable() {
        EventManager.RemoveListener("PAUSE_GAME", OnPauseGame);
        EventManager.RemoveListener("RESUME_GAME", OnResumeGame);
    }

    private void OnPauseGame()
    {
        // todo should just set speed to 0 to prevent little issues but works for now
        shouldUpdate = false;
        enemy.enabled = false;
    }

    private void OnResumeGame()
    {
        shouldUpdate = true;
        enemy.enabled = true;
    }

    //Resets the Enemy to its default values.
    //(This could be used by the spawning script?)
    public void Respawn() {
		hp = maxHP;
	}

	//Public access to the Enemy's HP.
	public void Damage(float damage) {
		hp -= damage;
	}

	//Despawns the Enemy
	//(this will probably need to be a coroutine
	//using fade-out at some point)
	void Despawn() {
        particles = Instantiate(particles, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(particles, 2.0f);

        // TODO this will also trigger when enemies leave or are despawned for other reasons
        EventManager.PostEvent("SPIRIT_DESTROYED");

        enemies.Remove(this);
		Destroy(gameObject);
	}

    private void Update()
    {
        if (!shouldUpdate) return;
        if (wordsLeft == 0)
        {
            GameManager.Instance.EndGame();
        }
        if (hasWord == false) // if enemy is not holding a word, then search for the closest word
        {
            closestWord();
        }
        Move();
        wordCount = GameObject.FindGameObjectsWithTag("Word"); // Array with all word
        if (wordCount.Length == 0) // If there are no words
        {
            isWord = false; // no words exist
            //enemy.SetDestination(Player.transform.position); // Destination for the enemy is set to the player's position
        }
    }

    void closestWord()
    //funtion to find the closest word to the enemy
    {
        words = GameObject.FindGameObjectsWithTag("Word"); // find game objects with tag Wall

        min = 100000; // set the minimum to a very high number

        for (int i = 0; i < words.Length; i++) // go through all the words
        {
            if (Vector3.Distance(words[i].transform.position, transform.position) < min) // if the distance to a certain word is less than the minimum distance we currently have
            {
                min = Vector3.Distance(words[i].transform.position, transform.position); // sets the new min
                minIndex = i; // index of the word with the min distance
            }
        }
    }

    void closestHeldWord()
    //funtion to find the closest word to the enemy
    {
        wordsHeld = GameObject.FindGameObjectsWithTag("WordBeingHeld"); // find game objects with tag Wall

        minH = 100000; // set the minimum to a very high number

        for (int i = 0; i < wordsHeld.Length; i++) // go through all the words
        {
            if (Vector3.Distance(wordsHeld[i].transform.position, transform.position) < minH) // if the distance to a certain word is less than the minimum distance we currently have
            {
                minH = Vector3.Distance(wordsHeld[i].transform.position, transform.position); // sets the new min
                minIndexH = i; // index of the word with the min distance
            }
        }
    }

    void closestWall()
    //funtion to find the closest wall to the enemy
    {
        walls = GameObject.FindGameObjectsWithTag("Wall"); // find game objects with tag Wall

        minW = 100000; // set the minimum to a very high number

        for (int i = 0; i < walls.Length; i++) // go through all the walls
        {
            if (Vector3.Distance(walls[i].transform.position, transform.position) < minW) // if the distance to a certain wall is less than the minimum distance we currently have
            {
                minW = Vector3.Distance(walls[i].transform.position, transform.position); // sets the new min
                minIndexW = i; // index of the wall with the min distance
            }
        }
        closeWall = true; // calculated the closest wall
    }

    // Method that actually makes Enemy walk
    private void Move()
    {
        if (hasWord == true) // if enemy is holding the word
        {
            closestWall(); // calculate the closest wall

            if (playedStolen == false) //ADDED NOW
            {//ADDED NOW
                lostWordS.clip = stolenWord;//ADDED NOW
                lostWordS.Play();//ADDED NOW
                Debug.Log("PLAYING STOLEN WORD");//ADDED NOW
                playedStolen = true;//ADDED NOW
            }//ADDED NOW

            if (minW < 8)
            {
                // Remove word and despawn ourselves
                closestHeldWord();
                lostWordS.clip = lostWord;
                wordsLeft--;
                Destroy(wordsHeld[minIndexH]);
                lostWordS.Play();
                Debug.Log("I AM PLAYING THE SOUND FOR A MILISECOND");
                Despawn();
                EventManager.PostEvent("WORD_LOST");
            }

            if (words.Length < 1 && isBaked == false)
            {
                houseO.enabled = true;
                isBaked = true;
            }

            if (walls[minIndexW].transform.position.z == 0) // If I want the enemy to only move in the x axis towards the spawn area
            {
                spawnPoint = new Vector3(walls[minIndexW].transform.position.x, transform.position.y, transform.position.z);
                enemy.SetDestination(spawnPoint);
            }

            if (walls[minIndexW].transform.position.x == 0) // If I want the enemy to only move in the z axis towards the spawn area
            {
                spawnPoint = new Vector3(transform.position.x, transform.position.y, walls[minIndexW].transform.position.z);
                enemy.SetDestination(spawnPoint);
            }
            txtOffset = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            words[minIndex].transform.position = txtOffset; // Make text above the enemy's head
        }

        else if (min < 2)// if enemy found word, now enemy is holding the word and the word's tag is changed to being held
        {
            hasWord = true;
            words[minIndex].tag = "WordBeingHeld";
        }

        else if (hasWord == false) // if enemy is not holding a word
        {
            if (words.Length > 0) // if there are still words
            {
                enemy.SetDestination(words[minIndex].transform.position); // walk to the closest word
                if (isBaked == true)
                {
                    houseO.enabled = false;
                    isBaked = false;
                }

            }
            else // if there are no more words
            {
                enemy.SetDestination(Player.transform.position); // walk to the player
                if (isBaked == false)
                {
                    houseO.enabled = true;
                    isBaked = true;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveTowards : MonoBehaviour
{

    public bool hasWord;
    bool reachHouse;
    public bool isWord;
    bool closeWall;
    public GameObject[] words;
    public GameObject[] walls;
    public GameObject[] wordsHeld;
    GameObject[] wordCount;
    GameObject Player;
    public NavMeshAgent enemy;
    private float[] values;
    float min;
    int minIndex;
    public float HP;
    float minW;
    int minIndexW;
    float minH;
    int minIndexH;
    Vector3 spawnPoint;
    Vector3 txtOffset;
    public int wordsLeft;


    private void Start()
    {
        closeWall = false;
        isWord = true;
        Player = GameObject.FindGameObjectWithTag("Player");
    }


    private void Update()
    {
        if (hasWord == false) // if enemy is not holding a word, then search for the closest word
        {
            closestWord();
        }
        Move();
        if (HP <= 0) // if enemy dies
        {
            hasWord = false; // enemy stops holding the word
            txtOffset = new Vector3(transform.position.x, 1, transform.position.z); // text drops on the floor
            words[minIndex].transform.position = txtOffset;
            words[minIndex].tag = "Word"; // Tag becomes Word so that other enemies can search for it
            Destroy(gameObject); // Destroy
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) // Placeholder to kill Enemy
        {
            HP = HP - 100;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) // Placeholder to debug
        {
            hasWord = true;
        }
        wordCount = GameObject.FindGameObjectsWithTag("Word"); // Array with all word
        if (wordCount.Length == 0) // If there are no words
        {
            isWord = false; // no words exist
            //enemy.SetDestination(Player.transform.position); // Destination for the enemy is set to the player's position
        }
        Debug.Log(hasWord);
    }



    void closestWord()
    //funtion to find the closest word to the enemy
    {
        words = GameObject.FindGameObjectsWithTag("Word"); // find game objects with tag Wall

        min = 100000; // set the minimum to a very high number

        for (int i = 0; i < words.Length; i++) // go through all the words
        {
            if (Vector3.Distance(words[i].transform.position,transform.position) < min) // if the distance to a certain word is less than the minimum distance we currently have
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

            if (minW < 2)
            {
                closestHeldWord();
                wordsLeft--;
                Destroy(wordsHeld[minIndexH]);
                Destroy(gameObject);
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
            txtOffset = new Vector3(transform.position.x,transform.position.y + 1, transform.position.z); 
            words[minIndex].transform.position = txtOffset; // Make text above the enemy's head
        }

        else if (min <2)// if enemy found word, now enemy is holding the word and the word's tag is changed to being held
        {
            hasWord = true;
            words[minIndex].tag = "WordBeingHeld";
        }

        else if (hasWord == false) // if enemy is not holding a word
        {
            if (words.Length > 0) // if there are still words
            {
                enemy.SetDestination(words[minIndex].transform.position); // walk to the closest word
            }
            else // if there are no more words
            {
                enemy.SetDestination(Player.transform.position); // walk to the player
            }
        }  
    }
}

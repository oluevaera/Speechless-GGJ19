using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    NavMeshObstacle house;
    AudioSource lostWordS;//ADDED NOW
    public AudioClip lostWord;//ADDED NOW
    public static GameManager Instance 
    {
        get 
        {
            if (instance == null)
            {
                var go = new GameObject();
                instance = go.AddComponent<GameManager>();
                Debug.LogWarning("Automatically created a GameManager, you should create one yourself!");
            }
            return instance;
        }
    }

    private GameState state = GameState.Stopped;
    private float startTime;

    private void Awake() 
    {
        instance = this;
    }

    void playClip()
    {
        lostWordS.Play();
    }

    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GameState.Running) PauseGame();
            else if (state == GameState.Paused) ResumeGame();
        }
    }

    // Call to start the game
    public void StartGame()
    {
        if (state == GameState.Running)
        {
            Debug.LogWarning("Attempted to start the game whilst it's already running");
            return;
        }
        house = FindObjectOfType<NavMeshObstacle>();
        house.enabled = false;
        lostWordS = GetComponent<AudioSource>();
        Enemy.wordsLeft = 4;
        startTime = Time.time;
        state = GameState.Running;
        EventManager.PostEvent("START_GAME");
    }

    // Call to pause the game
    public void PauseGame()
    {
        if (state == GameState.Paused) 
        {
            Debug.LogWarning("Attempted to pause the game whilst it's already paused");
            return;
        }

        state = GameState.Paused;
        EventManager.PostEvent("PAUSE_GAME");
    }

    // Call to resume the game
    public void ResumeGame()
    {
        if (state != GameState.Paused)
        {
            Debug.LogWarning("Attempted to resume the game whilst it's not paused");
            return;
        }

        state = GameState.Running;
        EventManager.PostEvent("RESUME_GAME");
    }

    // Call to end the game
    public void EndGame()
    {
        if (state == GameState.Stopped)
        {
            Debug.LogWarning("Attempted to end the game whilst it's not running");
            return;
        }

        state = GameState.Stopped;
        EventManager.PostEvent("END_GAME");
    }

    public float GetStartTime()
    {
        return startTime;
    }

    public GameState GetState()
    {
        return state;
    }

    public enum GameState 
    {
        Running,
        Stopped,
        Paused
    }

    [System.Serializable]
    private class PauseResumeGameEvent : UnityEvent<bool> {}
}

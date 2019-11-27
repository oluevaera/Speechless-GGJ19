using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI wavesSurvivedText;
    [SerializeField]
    private TextMeshProUGUI spiritsDestroyedText;
    [SerializeField]
    private TextMeshProUGUI word1LostText;
    [SerializeField]
    private TextMeshProUGUI word2LostText;
    [SerializeField]
    private TextMeshProUGUI word3LostText;
    [SerializeField]
    private TextMeshProUGUI word4LostText;
    [SerializeField]
    private TextMeshProUGUI wordsRecoveredText;
    [SerializeField]
    private TextMeshProUGUI playerKnockedBackText;
    [SerializeField]
    private TMP_InputField playerNameText;
    [SerializeField]
    private GameObject mainMenuButton;
    [SerializeField] 
    private GameObject submitScoreButton;

    [SerializeField] private string scoreBoardHost = "https://localhost:3000/";

    private int wavesSurvived = 0;
    private int spiritsDestroyed = 0;
    private List<float> wordLost = new List<float>();
    private int wordsRecovered = 0;
    private int playerKnockedBack = 0;

    private void Start() {
        EventManager.AddListener("START_GAME", OnStartGame);
        EventManager.AddListener("END_GAME", OnEndGame);
        EventManager.AddListener("WAVE_COMPLETE", OnWaveComplete);
        EventManager.AddListener("SPIRIT_DESTROYED", OnSpiritDestroyed);
        EventManager.AddListener("WORD_LOST", OnWordLost);
        EventManager.AddListener("WORD_RECOVERED", OnWordRecovered);
        EventManager.AddListener("PLAYER_KNOCKED_BACK", OnPlayerKnockedBack);
    }

    private void UpdateDisplay()
    {
        // Add blank values for now
        while (wordLost.Count < 4)
        {
            wordLost.Add(0);
        }
        
        wavesSurvivedText.text = wavesSurvived.ToString();
        spiritsDestroyedText.text = spiritsDestroyed.ToString();
        word1LostText.text = string.Format("{0}m {1:00}s", (int)wordLost[0] / 60, (int)wordLost[0] % 60);
        word2LostText.text = string.Format("{0}m {1:00}s", (int)wordLost[1] / 60, (int)wordLost[1] % 60);
        word3LostText.text = string.Format("{0}m {1:00}s", (int)wordLost[2] / 60, (int)wordLost[2] % 60);
        word4LostText.text = string.Format("{0}m {1:00}s", (int)wordLost[3] / 60, (int)wordLost[3] % 60);
        wordsRecoveredText.text = wordsRecovered.ToString();
        playerKnockedBackText.text = playerKnockedBack.ToString();
    }

    public void OnStartGame()
    {
        // Reset stats
        wavesSurvived = 0;
        spiritsDestroyed = 0;
        wordLost = new List<float>();
        wordsRecovered = 0;
        playerKnockedBack = 0;
        playerNameText.text = "";

        mainMenuButton.SetActive(false);
        submitScoreButton.SetActive(false);
    }

    public void OnEndGame() 
    {
        mainMenuButton.SetActive(true);
        submitScoreButton.SetActive(true);
        UpdateDisplay();
    }

    public void OnWaveComplete()
    {
        wavesSurvived++;
    }

    public void OnSpiritDestroyed()
    {
        spiritsDestroyed++;
    }

    public void OnWordLost()
    {
        wordLost.Add(Time.time - GameManager.Instance.GetStartTime());
    }

    public void OnWordRecovered()
    {
        wordsRecovered++;
    }

    public void OnPlayerKnockedBack()
    {
        playerKnockedBack++;
    }

    public void OnClickSubmitScore()
    {
        if (wordLost.Count > 3 && playerNameText.text.Length > 0)
        {
            StartCoroutine(nameof(SendScore));
        }
    }

    private IEnumerator SendScore()
    {
        var score = new Score {name = playerNameText.text, time = wordLost[3]};
        var request = UnityWebRequest.Put(scoreBoardHost + "addScore", JsonUtility.ToJson(score));
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        
        if(request.isNetworkError || request.isHttpError) {
            Debug.LogError($"Failed to upload score: {request.error}");
        }
        else {
            Debug.Log("Successfully uploaded scored");
        }
    }

    [Serializable]
    public struct Score
    {
        public string name;
        public float time;
    }
}

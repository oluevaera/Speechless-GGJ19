using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuScreen;
    [SerializeField]
    private GameObject mainMenuScreen;
    [SerializeField]
    private Image fadeScreen;

    [Header("Transitions")]
    [SerializeField]
    private float startGameTransitionTime = 1f;
    [SerializeField]
    private float scoreTransitionTime = 2f;
    // How long to hold at a full black screen
    private float fadeTransitionHoldTime = 0.25f;

    [Header("Menu Objects")]
    [SerializeField]
    private Image startGameButton;
    [SerializeField]
    private TextMeshProUGUI titleText;

    void Start()
    {
        // Register event listeners
        EventManager.AddListener("START_GAME", OnStartGame);
        EventManager.AddListener("END_GAME", OnEndGame);
        EventManager.AddListener("PAUSE_GAME", OnPauseGame);
        EventManager.AddListener("RESUME_GAME", OnResumeGame);

        // Ensure correct screen is displayed
        pauseMenuScreen.SetActive(false);
        mainMenuScreen.SetActive(true);
    }

    private void OnStartGame()
    {
        StartCoroutine("FadeOutMainMenu");
    }

    private void OnEndGame()
    {
        pauseMenuScreen.SetActive(false);

        StartCoroutine("TransitionFadeScreen");
    }

    private void OnPauseGame()
    {
        pauseMenuScreen.SetActive(true);
    }

    private void OnResumeGame()
    {
        pauseMenuScreen.SetActive(false);
    }

    public void OnClickScoreScreenMainMenu()
    {
        StartCoroutine("TransitionFadeScreen");
        StartCoroutine("EnableMainMenu");
        SceneManager.LoadScene("MainGame");
    }

    public void OnClickStartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void OnClickPauseScreenResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }

    public void OnClickPauseScreenEndGame()
    {
        GameManager.Instance.EndGame();
    }

    private IEnumerator EnableMainMenu() {
        yield return new WaitForSeconds(scoreTransitionTime * 2 + fadeTransitionHoldTime);
        StartCoroutine("FadeInMainMenu");
    }

    private IEnumerator TransitionFadeScreen()
    {
        for (float t = 0f; t < scoreTransitionTime; t += Time.deltaTime)
        {
            fadeScreen.color = Color.Lerp(Color.clear, Color.black, t);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(fadeTransitionHoldTime);

        for (float t = 0f; t < scoreTransitionTime; t += Time.deltaTime)
        {
            fadeScreen.color = Color.Lerp(Color.black, Color.clear, t);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FadeOutMainMenu()
    {
        var text = startGameButton.GetComponentInChildren<TextMeshProUGUI>();
        var clearWhite = new Color(1, 1, 1, 0);
        for (float t = 0; t < startGameTransitionTime; t += Time.deltaTime)
        {
            var color = Color.Lerp(Color.white, clearWhite, t);
            text.color = color;
            startGameButton.color = color;
            titleText.color = color;

            yield return new WaitForEndOfFrame();
        }
        mainMenuScreen.SetActive(false);
    }

    private IEnumerator FadeInMainMenu()
    {
        mainMenuScreen.SetActive(true);

        var text = startGameButton.GetComponentInChildren<TextMeshProUGUI>();
        var clearWhite = new Color(1, 1, 1, 0);
        for (float t = 0; t <= startGameTransitionTime; t += Time.deltaTime)
        {
            var color = Color.Lerp(clearWhite, Color.white, t);
            text.color = color;
            startGameButton.color = color;
            titleText.color = color;

            yield return new WaitForEndOfFrame();
        }
    }
}

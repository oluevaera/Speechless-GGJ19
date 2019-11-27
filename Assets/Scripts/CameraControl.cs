using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private View currentView = View.Menu;
    [SerializeField]
    private GameObject pivot;
    [SerializeField]
    private Camera gameCamera;
    [SerializeField]
    private Camera scoreCamera;

    [Header("Menu Settings")]
    [SerializeField]
    private float rotationSpeed = 1f;
    [SerializeField]
    private float menuZoom = 10f;
    [SerializeField]
    private Vector3 menuPivotPosition = Vector3.zero;

    [Header("Game Settings")]
    [SerializeField]
    private float gameZoom = 5f;
    [SerializeField]
    private Vector3 gamePivotPosition = Vector3.zero;
    [SerializeField]
    private float gameRotation = 0f;

    [Header("Transition")]
    [SerializeField]
    private float transitionTime = 2f;
    [SerializeField]
    private AnimationCurve transitionCurve;
    
    private void Start()
    {
        // Register ourself for events
        EventManager.AddListener("START_GAME", OnStartGame);
        EventManager.AddListener("END_GAME", OnEndGame);

        StartCoroutine("MenuView");
    }

    // Called when the game starts
    private void OnStartGame()
    {
        currentView = View.Game;
    }

    private void OnEndGame()
    {
        currentView = View.Score;
    }

    public void ExitScoreScreen()
    {
        currentView = View.Menu;
    }

    // Handles the camera in the menu view
    IEnumerator MenuView()
    {
        gameCamera.gameObject.SetActive(true);
        scoreCamera.gameObject.SetActive(false);

        pivot.transform.position = Vector3.zero;
        gameCamera.orthographicSize = menuZoom;
        
        while (currentView == View.Menu)
        {
            // Do a fancy rotating camera
            pivot.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine("MenuToGameView");
        yield break;
    }

    // Transitions the camera from the menu view to the ingame view
    IEnumerator MenuToGameView() 
    {
        var pivotRotation = pivot.transform.rotation.eulerAngles;
        for (float t = 0f; t < transitionTime; t += Time.deltaTime)
        {
            float tNormalised = transitionCurve.Evaluate(t / transitionTime);

            gameCamera.orthographicSize = Mathf.Lerp(menuZoom, gameZoom, tNormalised);
            pivot.transform.rotation = Quaternion.Euler(pivotRotation.x, Mathf.LerpAngle(pivotRotation.y, gameRotation, tNormalised), pivotRotation.z);
            pivot.transform.position = Vector3.Lerp(menuPivotPosition, gamePivotPosition, tNormalised);

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine("GameView");
        yield break;
    }

    IEnumerator GameView()
    {
        gameCamera.orthographicSize = gameZoom;
        while (currentView == View.Game) 
        {
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine("GameToScoreView");
        yield break;
    }

    IEnumerator GameToScoreView()
    {
        yield return new WaitForSeconds(transitionTime);

        gameCamera.gameObject.SetActive(false);
        scoreCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(transitionTime);

        StartCoroutine("ScoreView");
        yield break;
    }

    IEnumerator ScoreView()
    {
        while (currentView == View.Score) 
        {
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine("ScoreToMenuView");
        yield break;
    }

    IEnumerator ScoreToMenuView()
    {
        yield return new WaitForSeconds(transitionTime);

        gameCamera.gameObject.SetActive(true);
        scoreCamera.gameObject.SetActive(false);

        yield return new WaitForSeconds(transitionTime / 2);

        for (float t = 0f; t < transitionTime; t += Time.deltaTime)
        {
            float tNormalised = transitionCurve.Evaluate(t / transitionTime);

            gameCamera.orthographicSize = Mathf.Lerp(gameZoom, menuZoom, tNormalised);
            pivot.transform.position = Vector3.Lerp(gamePivotPosition, menuPivotPosition, tNormalised);

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine("MenuView");
        yield break;
    }

    private enum View {
        Menu,
        Game,
        Score
    }
}

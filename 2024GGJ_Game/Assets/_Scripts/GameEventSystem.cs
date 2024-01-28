using UnityEngine;
using TMPro;
using System.Collections;

public class GameEventSystem : MonoBehaviour
{
    public static GameEventSystem Instance;
    [SerializeField] private TextMeshProUGUI gameStartText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private float gameTime = 180.0f; 
    public static bool gameStarted = false;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
    }
    void Start()
    {
       // gameStartText.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        gameStartText.text = "Press Enter to start game";
    }

    void Update()
    {

        if (gameStarted)
        {
            UpdateCountdown();
        }

        /*if (!gameStarted && Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }

        if (gameStarted)
        {
            UpdateCountdown();
        }     */
    }

    public void StartGame()
    {
        gameStarted = true;
        gameStartText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(true);
        StartCoroutine(CountdownRoutine());
    }

    void UpdateCountdown()
    {
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            countdownText.text = "Time: " + FormatTime(gameTime);
        }
    }

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(gameTime);
        GameOver();
    }

    void GameOver()
    {
        gameStarted = false;
        gameOverText.gameObject.SetActive(true);
        gameOverText.text = "Game Over";
        countdownText.gameObject.SetActive(false);
    }

    string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60);
        int seconds = (int)(timeInSeconds % 60);
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void ReduceTime(float amount)
    {
        gameTime = Mathf.Max(0, gameTime - amount);

    }
}

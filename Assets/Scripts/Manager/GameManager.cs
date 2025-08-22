using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;

    public static GameManager Instance { get; private set; }

    [SerializeField] TMP_Text speedText;

    [SerializeField] ChunkManager chunkManager;

    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    private bool isGamePaused = false;
    public bool IsGamePaused => isGamePaused;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        DisplaySpeed();
    }

    private void DisplaySpeed()
    {
        if (chunkManager != null)
        {
            float kmph = chunkManager.chunkMoveSpeed * 3.6f;
            speedText.text = kmph.ToString("F1") + " km/h";
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        scoreManager.NewHighScore();
    }

    public void SetPause(bool pause)
    {
        isGamePaused = pause;
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

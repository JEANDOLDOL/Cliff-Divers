using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private float scoreTimer = 0f;
    [SerializeField] float scoreInterval = 0.25f;  // 점수 추가 간격
    float nextScoreTime = 0f;    // 다음 점수 추가 시점

    private string Keyname = "BestScore";

    [SerializeField] TMP_Text highScoreText;
    [SerializeField] TMP_Text currentScoreText;
    [SerializeField] float lowAltitudeDistance = 30f;
    [SerializeField] GameObject ScorePopUp;
    private float bestScore = 0f;
    private float currentScore = 0f;

    [SerializeField] private Rigidbody playerRigidbody;

    private void Awake()
    {
        bestScore = PlayerPrefs.GetFloat(Keyname, 0);
        highScoreText.text = $"Your high score : {bestScore.ToString()}";
        currentScore = 0f;
    }

    private void Update()
    {
        CheckLowAltitudeScore();
        currentScoreText.text = $"Current score : {currentScore.ToString()}";
    }

    private void CheckLowAltitudeScore()
    {
        // check if gameover
        if (GameManager.Instance.IsGameOver)
        {
            return;
        }

        Ray ray = new Ray(playerRigidbody.position, Vector3.down);
        RaycastHit hit; // Object hit by raycasy

        if (Physics.Raycast(ray, out hit, 500f))
        {
            if (hit.distance <= lowAltitudeDistance)
            {
                scoreTimer += Time.deltaTime;

                int maxScore = 30;
                int minScore = 10;
                int scoreToAdd = Mathf.RoundToInt(Mathf.Lerp(maxScore, minScore, hit.distance / lowAltitudeDistance));

                if (scoreTimer >= 1f && Time.time >= nextScoreTime)
                {
                    AddScore(scoreToAdd);
                    nextScoreTime = Time.time + scoreInterval;
                    if (ScorePopUp)
                    {
                        ShowScorePopUp(scoreToAdd);
                    }
                }
            }
            else
            {
                scoreTimer = 0f;
            }

        }

    }

    private void ShowScorePopUp(int score)
    {
        var go = Instantiate(ScorePopUp, playerRigidbody.position, Quaternion.identity, playerRigidbody.transform);
        go.GetComponent<TextMeshPro>().text = score.ToString();
    }

    private void AddScore(float score)
    {
        currentScore += score;
    }

    public void NewHighScore()
    {
        if (currentScore > bestScore)
        {
            PlayerPrefs.SetFloat(Keyname, currentScore);
        }
    }
}

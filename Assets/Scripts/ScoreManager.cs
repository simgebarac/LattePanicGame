using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;

    private int totalScore = 0;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddScore(int amount)
    {
        totalScore += amount;
        if (totalScore < 0) totalScore = 0;
        UpdateUI();
    }

    public int GetScore() => totalScore;

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Puan: " + totalScore;
    }
}
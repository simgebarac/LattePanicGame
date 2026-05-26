using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI continueButtonText;

    private void Start()
    {
        Time.timeScale = 1f;

        int score = PlayerPrefs.GetInt("LevelScore", 0);
        int levelIndex = PlayerPrefs.GetInt("CompletedLevel", 1);
        int nextIndex = levelIndex + 1;

        if (titleText != null)
            titleText.text = $"Bölüm {levelIndex} Tamamlandı! ☕";

        if (scoreText != null)
            scoreText.text = "Puan: " + score;

        if (starText != null)
            starText.text = GetStars(score);

        if (continueButtonText != null)
            continueButtonText.text = nextIndex <= 3 ?
                $"Bölüm {nextIndex}'e Geç →" : "Finali Gör →";
    }

    private string GetStars(int score)
    {
        if (score >= 250) return "⭐⭐⭐";
        if (score >= 150) return "⭐⭐";
        return "⭐";
    }

    public void ContinueToNext()
    {
        int nextScene = PlayerPrefs.GetInt("CompletedLevel", 1) + 1;
        if (nextScene <= 3)
            SceneManager.LoadScene("Level" + nextScene);
        else
            SceneManager.LoadScene("ThankYou");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
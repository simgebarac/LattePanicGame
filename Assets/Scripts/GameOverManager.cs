using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        Time.timeScale = 1f;
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        bool isWin = PlayerPrefs.GetInt("IsWin", 0) == 1;

        if (titleText != null)
            titleText.text = isWin ? "☕ Level Tamamlandı!" : "😞 Game Over";

        if (scoreText != null)
            scoreText.text = "Puan: " + score;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
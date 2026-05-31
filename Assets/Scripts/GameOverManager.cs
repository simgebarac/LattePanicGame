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
        int score = PlayerPrefs.GetInt("LevelScore", 0);

        if (scoreText != null)
            scoreText.text = "Puan: " + score;

        // 🔊 İŞTE BURADA KANKA: Ekran açıldığı an kaybetme sesini çaldırıyoruz!
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayGameOverSound();

            // 🚀 VE BU SATIR: Paneldeki tüm butonları anında otomatik bağlar!
            SoundManager.Instance.YenidenBaglaButonlar();
        }
    }

    public void RestartLevel()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        SceneManager.LoadScene(currentLevel);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
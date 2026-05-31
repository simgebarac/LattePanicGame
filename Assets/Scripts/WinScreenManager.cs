using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        Time.timeScale = 1f;

        int score = PlayerPrefs.GetInt("LevelScore", 0);

        if (scoreText != null)
            scoreText.text = score.ToString();

        // 🔊 ZAFER ANI: Ekran açıldığı an zafer sesini çaldırıyoruz!
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayWinSound();

            // 🚀 OTOMATİK BAĞLANTI: Paneldeki butonları hemen hafızaya alıyoruz kanka!
            SoundManager.Instance.YenidenBaglaButonlar();
        }
    }

    public void ContinueToNext()
    {
        int completedLevel = PlayerPrefs.GetInt("CompletedLevel", 1);
        int nextScene = completedLevel + 1;

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
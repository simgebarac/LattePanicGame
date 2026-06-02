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
        // Hangi seviyeden geldiğimizi manuel olarak alıyoruz
        // Level1'i bitirdiysen CurrentLevel 1 olmalı
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        // Eğer Level 1'deysen ve 2'ye geçmen gerekiyorsa:
        if (currentLevel == 1)
        {
            PlayerPrefs.SetInt("CurrentLevel", 2);
            SceneManager.LoadScene("Level2");
        }
        // Eğer Level 2'deysen ve 3'e geçmen gerekiyorsa:
        else if (currentLevel == 2)
        {
            PlayerPrefs.SetInt("CurrentLevel", 3);
            SceneManager.LoadScene("Level3");
        }
        else
        {
            SceneManager.LoadScene("ThankYou");
        }

        PlayerPrefs.Save();
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
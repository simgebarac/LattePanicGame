using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WalkoutManager : MonoBehaviour
{
    public static WalkoutManager Instance { get; private set; }

    [SerializeField] private int maxWalkouts = 3;
    [SerializeField] private TextMeshProUGUI walkoutText;

    private int walkoutCount = 0;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // LevelSettings varsa oradan al
        if (LevelSettings.Instance != null)
            maxWalkouts = LevelSettings.Instance.maxWalkouts;

        UpdateUI();
    }

    public void RegisterWalkout()
    {
        walkoutCount++;
        UpdateUI();
        Debug.Log($"Walkout: {walkoutCount}/{maxWalkouts}");
        if (walkoutCount >= maxWalkouts)
            TriggerResult(isWin: false);
    }

    public void ShowLevelComplete()
    {
        TriggerResult(isWin: true);
    }

    private void TriggerResult(bool isWin)
    {
        int levelScore = DeliveryManager.Instance?.GetScore() ?? 0;
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        // Hangi levelde olduğunu kaydet
        PlayerPrefs.SetInt("CurrentLevel", currentLevel); // YENİ
        PlayerPrefs.SetInt("LevelScore", levelScore);
        PlayerPrefs.SetInt("CompletedLevel", currentLevel);
        PlayerPrefs.SetInt("IsWin", isWin ? 1 : 0);

        int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        PlayerPrefs.SetInt("TotalScore", totalScore + levelScore);
        PlayerPrefs.Save();

        Time.timeScale = 1f;

        if (isWin)
            SceneManager.LoadScene("WinScreen");
        else
            SceneManager.LoadScene("GameOver");
    }

    private void UpdateUI()
    {
        if (walkoutText != null)
            walkoutText.text = $"❌ {walkoutCount}/{maxWalkouts}";
    }
}
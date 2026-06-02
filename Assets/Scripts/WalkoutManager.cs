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

        // Sahne ADINDAN level numarasını güvenli şekilde al
        string sceneName = SceneManager.GetActiveScene().name;
        int levelNum = 1;
        if (sceneName == "Level1") levelNum = 1;
        else if (sceneName == "Level2") levelNum = 2;
        else if (sceneName == "Level3") levelNum = 3;

        // WinScreenManager bu değeri okuyacak
        PlayerPrefs.SetInt("CurrentLevel", levelNum);
        PlayerPrefs.SetInt("LevelScore", levelScore);
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
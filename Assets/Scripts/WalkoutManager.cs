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
        UpdateUI();
    }

    public void RegisterWalkout()
    {
        walkoutCount++;
        UpdateUI();
        if (walkoutCount >= maxWalkouts)
            TriggerResult(isWin: false);
    }

    public void ShowLevelComplete()
    {
        TriggerResult(isWin: true);
    }

    private void TriggerResult(bool isWin)
    {
        PlayerPrefs.SetInt("FinalScore", ScoreManager.Instance?.GetScore() ?? 0);
        PlayerPrefs.SetInt("IsWin", isWin ? 1 : 0);
        PlayerPrefs.Save();
        Time.timeScale = 1f;

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (isWin)
        {
            int nextScene = currentScene + 1;
            // GameOver sahnesi index 4 — ondan önce level var mı?
            if (nextScene <= 3) // Level3 index=3
            {
                SceneManager.LoadScene(nextScene); // Sonraki levela geç
            }
            else
            {
                SceneManager.LoadScene("GameOver"); // Tüm levellar bitti
            }
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void UpdateUI()
    {
        if (walkoutText != null)
            walkoutText.text = $"❌ {walkoutCount}/{maxWalkouts}";
    }
}
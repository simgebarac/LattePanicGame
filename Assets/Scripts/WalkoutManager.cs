using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class WalkoutManager : MonoBehaviour
{
    public static WalkoutManager Instance { get; private set; }

    [SerializeField] private int maxWalkouts = 3;
    [SerializeField] private TextMeshProUGUI walkoutText;
    [SerializeField] private string gameOverSceneName = "ResultsScreen";

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
            TriggerGameOver();
    }

    private void UpdateUI()
    {
        if (walkoutText != null)
            walkoutText.text = $"Kaçan: {walkoutCount}/{maxWalkouts}";
    }

    private void TriggerGameOver()
    {
        // Skoru kaydet ve sonuç ekranýna git
        PlayerPrefs.SetInt("FinalScore", ScoreManager.Instance?.GetScore() ?? 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameOverSceneName);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThankYouManager : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;
        // Toplam skoru sýfýrla
        PlayerPrefs.SetInt("TotalScore", 0);
        PlayerPrefs.Save();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
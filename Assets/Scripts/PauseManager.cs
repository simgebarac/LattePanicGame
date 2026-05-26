using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // Unity'den paneli buraya baðlayacaðýz
    private bool isPaused = false;

    void Update()
    {
        // ESC tuþuna basýldýðýnda tetiklenir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);  // Menüyü ekrana açar
        Time.timeScale = 0f;         // Oyunu tamamen dondurur (Müþteriler, hareketler durur)
        isPaused = true;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false); // Menüyü kapatýr
        Time.timeScale = 1f;         // Zamaný normale döndürür, oyun devam eder
        isPaused = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;         // Zamaný sýfýrlamazsak donmuþ olarak baþlar!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Mevcut seviyeyi yeniden yükler
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;         // Zamaný sýfýrla
        SceneManager.LoadScene("MainMenu"); // Ana menü sahnesine döner
    }
}
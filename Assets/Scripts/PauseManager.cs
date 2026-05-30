using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Panelleri")]
    [SerializeField] private GameObject pausePanel;   // Duraklatma menü paneli
    [SerializeField] private GameObject settingsPanel; // Bizim o pikselli yeni Ayarlar paneli

    private bool isPaused = false;

    void Update()
    {
        // ESC tuþuna basýldýðýnda tetiklenir
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Eðer o sýrada Ayarlar paneli açýksa, ESC'ye basýnca önce ayarlarý kapatsýn
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettingsInPause();
            }
            else
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
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
        if (settingsPanel != null) settingsPanel.SetActive(false); // Açýk kaldýysa ayarlarý da kapatýr
        Time.timeScale = 1f;         // Zamaný normale döndürür, oyun devam eder
        isPaused = false;
    }

    // --- PAUSE EKRANINDA AYARLARI AÇIP KAPATMA FONKSÝYONLARI ---
    public void OpenSettingsInPause()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // Ayarlar panelini açar
        }
    }

    public void CloseSettingsInPause()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false); // Ayarlar panelini kapatýr
        }
    }
    // ----------------------------------------------------------

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
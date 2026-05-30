using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Ayarlar UI Paneli")]
    [SerializeField] private GameObject settingsPanel; // Unity'den ayarlar panelini buraya sürükleyeceðiz

    // Baþla butonuna baðlanacak fonksiyon
    public void StartGame()
    {
        Time.timeScale = 1f; // Zamaný sýfýrla (her ihtimale karþý)
        SceneManager.LoadScene("Level1"); // Seni direkt 1. bölüme fýrlatýr
    }

    // Ayarlar Panelini Açma Fonksiyonu (Ayarlar Butonuna baðlanacak)
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // Paneli görünür yapar
        }
    }

    // Ayarlar Panelini Kapatma Fonksiyonu (Panelin içindeki X butonuna baðlanacak)
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false); // Paneli gizler
        }
    }

    // Çýkýþ butonuna baðlanacak fonksiyon
    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýþ yapýldý! (Bu yazý editörde görünür, Build'de oyun tamamen kapanýr)");
        Application.Quit(); // Gerçek oyunda masaüstüne döndüren komut
    }
}
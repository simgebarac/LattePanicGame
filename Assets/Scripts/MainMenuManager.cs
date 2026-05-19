using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Baþla butonuna baðlanacak fonksiyon
    public void StartGame()
    {
        Time.timeScale = 1f; // Zamaný sýfýrla (her ihtimale karþý)
        SceneManager.LoadScene("Level1"); // Seni direkt 1. bölüme fýrlatýr
    }

    // Çýkýþ butonuna baðlanacak fonksiyon
    public void QuitGame()
    {
        Debug.Log("Oyundan çýkýþ yapýldý!"); // Unity editöründe çalýþtýðýný görmek için
        Application.Quit(); // Gerçek build alýnca oyunu kapatýr
    }
}
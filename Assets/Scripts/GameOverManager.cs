using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Yeniden Dene butonuna bağlanacak fonksiyon
    public void YenidenDene()
    {
        // Zaman ölçeğini sıfırlama ihtimaline karşı 1 yapıyoruz (oyun donuk kalmasın)
        Time.timeScale = 1f;

        // Oyuncuyu tekrar ilk bölüme gönderir
        SceneManager.LoadScene("Level1");
    }

    // Ana Menüye Dön butonuna bağlanacak fonksiyon
    public void AnaMenuyeDon()
    {
        Time.timeScale = 1f;

        // Oyuncuyu ana menü sahnesine fırlatır
        SceneManager.LoadScene("MainMenu");
    }
}
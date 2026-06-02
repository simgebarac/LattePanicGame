using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; // Ayarlar_Panel'ini buraya bağlayacağız

    public void StartGame()
    {
        SceneManager.LoadScene("IntroScene"); 
       
    }
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            // Sahnede SoundManager'ı bulup slider'ları hafızadaki yerlerine göre kilitler:
            FindObjectOfType<SoundManager>()?.BaglaSliderlar();
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        // Oyundan tamamen çıkış yapar
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; // Ayarlar_Panel'ini buraya baðlayacaðýz

    public void StartGame()
    {
        // Oyunu normal hýzýna getiriyoruz
        Time.timeScale = 1f;

        // Direkt Level1 yüklemek yerine önce bizim efsane introyu açýyoruz kanka!
        SceneManager.LoadScene("IntroScene");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            // Sahnede SoundManager'ý bulup slider'larý hafýzadaki yerlerine göre kilitler:
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
        // Oyundan tamamen çýkýþ yapar
        Application.Quit();
    }
}
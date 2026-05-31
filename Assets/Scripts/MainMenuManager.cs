using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; // Ayarlar_Panel'ini buraya baðlayacaðýz

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
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
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
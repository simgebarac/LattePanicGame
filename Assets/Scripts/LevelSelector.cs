using UnityEngine;
using UnityEngine.SceneManagement; // Sahneleri deðiþtirebilmek için bu kütüphane þart!

public class LevelSelector : MonoBehaviour
{
    // Bu fonksiyonu Level 1 butonu için çaðýracaðýz
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1"); // Kanka buraya senin Seviye 1 sahnenin adý neyse týrnak içine tam olarak aynýsýný yaz!
    }

    // Bu fonksiyonu Level 2 butonu için çaðýracaðýz
    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2"); // Buraya Seviye 2 sahnenin tam adýný yaz
    }

    // Bu fonksiyonu Level 3 butonu için çaðýracaðýz
    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3"); // Buraya Seviye 3 sahnenin tam adýný yaz
    }
}
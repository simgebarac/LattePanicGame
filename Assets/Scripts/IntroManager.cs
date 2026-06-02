using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video; // 🚨 Bunu eklemeyi unutma!

public class IntroManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        // 🚨 ÖNEMLİ KONTROL: VideoPlayer sahne açılınca hazır mı?
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Eğer video yolu boşsa veya video yoksa direkt oyuna geç, çökmesin!
        if (videoPlayer != null && videoPlayer.clip != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Play();
        }
        else
        {
            Debug.Log("Video bulunamadı, direkt oyuna geçiliyor...");
            Invoke("LoadGameScene", 1f); // Hata vermesin diye 1 saniye sonra geç
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadGameScene();
    }

    public void IntroyuAtla()
    {
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("Level1");
    }
}
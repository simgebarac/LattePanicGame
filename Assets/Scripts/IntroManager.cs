using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class IntroManager : MonoBehaviour
{
    [Header("Video Ayarlarý")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private List<VideoClip> introVideolari; // 4 videoyu buraya sýrayla koyacaðýz kanka

    private int mevcutVideoIndex = 0;

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Videolar bittiðinde tetiklenecek fonksiyonu Unity'ye tanýtýyoruz kanka
        videoPlayer.loopPointReached += VideoBittiðindeTetikle;

        // Ýlk videoyu oynatmaya hazýrla (Donmayý önlemek için Preload yapýyoruz kanka)
        VideoyuHazýrlaVeOynat(mevcutVideoIndex);
    }

    void VideoyuHazýrlaVeOynat(int index)
    {
        if (introVideolari != null && index < introVideolari.Count && introVideolari[index] != null)
        {
            videoPlayer.clip = introVideolari[index];
            videoPlayer.Prepare(); // Videoyu RAM'e önceden yükle, donmayý keser kanka

            // Video tamamen belleðe yüklenince oynat kanka
            videoPlayer.prepareCompleted += OynatmayýBaþlat;
        }
        else
        {
            // Videolar bittiyse veya liste boþsa direkt Level 1 kanka!
            IntroBittiOyunaGec();
        }
    }

    void OynatmayýBaþlat(VideoPlayer vp)
    {
        videoPlayer.prepareCompleted -= OynatmayýBaþlat;
        videoPlayer.Play();
    }

    void VideoBittiðindeTetikle(VideoPlayer vp)
    {
        mevcutVideoIndex++;
        VideoyuHazýrlaVeOynat(mevcutVideoIndex);
    }

    // Oyuncu sað üstteki butona basarsa introyu tamamen atlasýn kanka (Jüri sever bunu)
    public void IntroyuAtla()
    {
        // Videoyu hemen durdur kanka
        if (videoPlayer != null) videoPlayer.Stop();

        // Level 1'e direkt geçiþ
        SceneManager.LoadScene("Level1");
    }

    void IntroBittiOyunaGec()
    {
        // Temizlik yapýyoruz kanka hafýza dolmasýn
        videoPlayer.loopPointReached -= VideoBittiðindeTetikle;

        // Tam senin hiyerarþideki isme göre Level 1 sahnesini çaðýrýyoruz
        SceneManager.LoadScene("Level1");
    }
}
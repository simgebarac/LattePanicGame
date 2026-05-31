using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // 🚨 Butonları bulabilmek için bu kütüphane şart kanka!
using UnityEngine.SceneManagement; // 🚨 Sahne geçişlerini dinlemek için bu şart kanka!

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer ve Kanallar")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource; // Efektleri çalacak kaynak

    [Header("Ses Klipleri (SFX)")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip pickUpSFX;
    [SerializeField] private AudioClip coffeeMachineSFX;
    [SerializeField] private AudioClip winSFX;
    [SerializeField] private AudioClip gameOverSFX;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Her oyun açılışında sesi tam aç, PlayerPrefs'i sıfırla
            PlayerPrefs.SetFloat("MuzikHacim", 1f);
            PlayerPrefs.SetFloat("EfektHacim", 1f);
            PlayerPrefs.Save();

            SetMuzikSes(1f);
            SetEfektSes(1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Hafıza sızıntısı (Memory Leak) olmaması için etkinlik bağlantısını koparıyoruz kanka
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Sahne her yüklendiğinde (veya GameOver paneli gibi yapılar tetiklendiğinde) otomatik çalışan motor kanka
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        YenidenBaglaButonlar();

        // Dökümandaki slider bağlama işini de sahne açılınca otomatik tetikliyoruz kanka
        BaglaSliderlar();
    }

    // 🎯 AMELENİN DOSTU FONKSİYON: Sahnede ne kadar gizli/açık buton varsa ses atar
    public void YenidenBaglaButonlar()
    {
        // Sahnede pasif duran GameOver veya Win panellerinin içindeki butonları da bulabilmek için Resources kullanıyoruz
        Button[] sahnedeNeKadarButonVarsa = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button btn in sahnedeNeKadarButonVarsa)
        {
            // Önce butonun eski ses dinleyicilerini temizliyoruz (üst üste binip çift ses çıkmasın diye)
            btn.onClick.RemoveListener(PlayButtonClick);

            // Ve şak! Butona basıldığında bizim PlayButtonClick fonksiyonunu çalıştır diyoruz kanka
            btn.onClick.AddListener(PlayButtonClick);
        }
    }

    // Arayüz butonlarına tıklandığında otomatik veya elle çalacak fonksiyon kanka
    public void PlayButtonClick()
    {
        if (buttonClickSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(buttonClickSFX);
        }
    }

    // "E" tuşuyla tezgahtan nesne alındığında tetiklenecek fonksiyon kanka
    public void PlayPickUp()
    {
        if (pickUpSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(pickUpSFX);
        }
    }

    // Kahve makinesi sesi uzun ve döngülü (loop) olacağı için özel fonksiyon kanka
    public void PlayCoffeeMachine()
    {
        if (sfxSource != null && coffeeMachineSFX != null)
        {
            // Eğer zaten kahve makinesi sesi çalmıyorsa başlat kanka
            if (!sfxSource.isPlaying || sfxSource.clip != coffeeMachineSFX)
            {
                sfxSource.clip = coffeeMachineSFX;
                sfxSource.loop = true; // Basılı tutulduğu sürece dönecek
                sfxSource.Play();
            }
        }
    }

    // Oyuncu elini "E" tuşundan çektiğinde veya pişirme bittiğinde sesi pürüzsüzce durdurur kanka
    public void StopCoffeeMachine()
    {
        if (sfxSource != null && sfxSource.clip == coffeeMachineSFX)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }

    // Oyuncu seviyeyi kazandığında tetiklenecek fonksiyon kanka
    public void PlayWinSound()
    {
        if (winSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(winSFX);
        }
    }

    // Müşterilerin sabrı tükenip oyun bittiğinde tetiklenecek fonksiyon kanka
    public void PlayGameOverSound()
    {
        if (gameOverSFX != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(gameOverSFX);
        }
    }

    // Sahneler yüklenince slider'ları RAM'den bulan o meşhur fonksiyon kanka
    public void BaglaSliderlar()
    {
        UnityEngine.UI.Slider[] sahnedeNeKadarSliderVarsa = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Slider>();
        foreach (UnityEngine.UI.Slider s in sahnedeNeKadarSliderVarsa)
        {
            if (s.gameObject.name == "Muzik_Slider")
            {
                s.onValueChanged.RemoveAllListeners();
                s.onValueChanged.AddListener(SetMuzikSes);
                s.value = PlayerPrefs.GetFloat("MuzikHacim", 1f);
            }
            else if (s.gameObject.name == "Efekt_Slider")
            {
                s.onValueChanged.RemoveAllListeners();
                s.onValueChanged.AddListener(SetEfektSes);
                s.value = PlayerPrefs.GetFloat("EfektHacim", 1f);
            }
        }
    }

    public void SetMuzikSes(float value)
    {
        float db = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("MuzikVol", db);
        PlayerPrefs.SetFloat("MuzikHacim", value);
    }

    public void SetEfektSes(float value)
    {
        float db = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("EfektVol", db);
        PlayerPrefs.SetFloat("EfektHacim", value);
    }
}
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [Header("Audio Mixer Baðlantýsý")]
    [SerializeField] private AudioMixer audioMixer;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Oyun ilk açýldýðýnda, hafýzada kayýtlý ses seviyelerini yükle (Yoksa 1 yani full aç)
        float muzikHacim = PlayerPrefs.GetFloat("MuzikHacim", 1f);
        float efektHacim = PlayerPrefs.GetFloat("EfektHacim", 1f);

        SetMuzikSes(muzikHacim);
        SetEfektSes(efektHacim);
    }

    // --- SÝHÝRLÝ BAÐLANTI FONKSÝYONU ---
    // Panel her açýldýðýnda bu fonksiyon çaðrýlacak ve gizli olan slider'larý bile bulup baðlayacak!
    public void BaglaSliderlar()
    {
        Slider[] sahnedeNeKadarSliderVarsa = Resources.FindObjectsOfTypeAll<Slider>();

        foreach (Slider s in sahnedeNeKadarSliderVarsa)
        {
            if (s.gameObject.name == "Muzik_Slider")
            {
                s.onValueChanged.RemoveAllListeners();
                s.onValueChanged.AddListener(SetMuzikSes);
                s.value = PlayerPrefs.GetFloat("MuzikHacim", 1f); // Hafýzadaki yeri slider'a yansýt
            }
            else if (s.gameObject.name == "SesEfekt_Slider")
            {
                s.onValueChanged.RemoveAllListeners();
                s.onValueChanged.AddListener(SetEfektSes);
                s.value = PlayerPrefs.GetFloat("EfektHacim", 1f);
            }
        }
    }

    public void SetMuzikSes(float value)
    {
        audioMixer.SetFloat("MuzikVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MuzikHacim", value); // Sesi hafýzaya kaydet
    }

    public void SetEfektSes(float value)
    {
        audioMixer.SetFloat("EfektVol", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("EfektHacim", value); // Efekti hafýzaya kaydet
    }
}
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public static LevelSettings Instance { get; private set; }

    [Header("Spawn Ayarlarý")]
    public float spawnInterval = 12f;
    public int maxCustomers = 10;

    [Header("Sabýr Ayarlarý")]
    public float patienceAtTable = 90f;
    public float patienceOutside = 40f;

    [Header("Game Over Ayarý")]
    public int maxWalkouts = 4;

    [Header("Tatlý Þansý (0=yok, 1=hep tatlý)")]
    [Range(0f, 1f)]
    public float dessertChance = 0f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }
}
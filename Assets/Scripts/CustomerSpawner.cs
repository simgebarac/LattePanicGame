using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefab & Noktalar")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] outsideWaitPoints;

    [Header("Level Ayarlarý")]
    [SerializeField] private float spawnInterval = 8f;
    [SerializeField] private int maxCustomersForLevel = 10;

    private List<MasaControl> allTables = new List<MasaControl>();
    private List<CustomerAI> waitingQueue = new List<CustomerAI>();
    private float timer;
    private int spawnedCount = 0;
    private float queueCheckCooldown = 0f;
    private int servedCount = 0;

    private void Start()
    {
        allTables.AddRange(FindObjectsByType<MasaControl>(FindObjectsSortMode.None));
        timer = spawnInterval;
    }

    private void Update()
    {
        // Spawn timer
        if (spawnedCount < maxCustomersForLevel)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnNewCustomer();
            }
        }

        // Queue kontrolü — Cooldown ile üst üste binmeleri kesin engelleme
        queueCheckCooldown -= Time.deltaTime;
        if (queueCheckCooldown <= 0f && waitingQueue.Count > 0)
        {
            waitingQueue.RemoveAll(c => c == null);
            MasaControl freeTable = GetFreeTable();
            if (freeTable != null)
            {
                freeTable.ReserveTable(); // MASAYI ANINDA KÝLÝTLE!
                queueCheckCooldown = 2.0f; // Güvenlik süresini 2 saniyeye çýkardýk yolda yürüme süresi için

                CustomerAI next = waitingQueue[0];
                waitingQueue.RemoveAt(0);
                next.AssignTable(freeTable);
                next.AllowEntry();
                UpdateQueuePositions();
            }
        }
    }

    private void SpawnNewCustomer()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject obj = Instantiate(customerPrefab, pos, Quaternion.identity);
        CustomerAI ai = obj.GetComponent<CustomerAI>();
        if (ai == null) return;

        spawnedCount++;
        MasaControl freeTable = GetFreeTable();

        // Eðer boþ masa varsa VE kuyrukta hiç bekleyen yoksa direkt içeri al
        if (freeTable != null && waitingQueue.Count == 0)
        {
            freeTable.ReserveTable(); // ANINDA KÝLÝTLE
            ai.SetupCustomer(freeTable, GetQueuePosition(0));
            ai.AllowEntry();
        }
        else
        {
            // Masa yoksa veya öncelik sýrasý baþkasýndaysa kuyruða ekle
            waitingQueue.Add(ai);
            int idx = waitingQueue.Count - 1;
            ai.SetupCustomer(null, GetQueuePosition(idx));
        }
    }

    public void RemoveFromQueue(CustomerAI customer)
    {
        if (waitingQueue.Contains(customer))
        {
            waitingQueue.Remove(customer);
            UpdateQueuePositions();
        }
    }

    private void UpdateQueuePositions()
    {
        for (int i = 0; i < waitingQueue.Count; i++)
        {
            if (waitingQueue[i] != null)
                waitingQueue[i].MoveToQueuePosition(GetQueuePosition(i));
        }
    }

    private Transform GetQueuePosition(int index)
    {
        if (outsideWaitPoints == null || outsideWaitPoints.Length == 0)
            return spawnPoint;
        return outsideWaitPoints[Mathf.Clamp(index, 0, outsideWaitPoints.Length - 1)];
    }

    private MasaControl GetFreeTable()
    {
        foreach (var t in allTables)
        {
            if (!t.IsTableOccupied()) return t;
        }
        return null;
    }

    public void RegisterServed()
    {
        servedCount++;
        Debug.Log($"Müþteri Doyuruldu! Toplam: {servedCount}/{maxCustomersForLevel}");
        if (servedCount >= maxCustomersForLevel)
            LevelComplete();
    }

    private void LevelComplete()
    {
        PlayerPrefs.SetInt("FinalScore", ScoreManager.Instance?.GetScore() ?? 0);
        PlayerPrefs.Save();

        // WalkoutManager'daki paneli tetikle
        if (WalkoutManager.Instance != null)
        {
            WalkoutManager.Instance.ShowLevelComplete();
        }
        else
        {
            Debug.Log("LEVEL TAMAMLANDI! Puan: " + ScoreManager.Instance?.GetScore());
            Time.timeScale = 0f;
        }
    }
}
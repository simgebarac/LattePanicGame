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
        if (LevelSettings.Instance != null)
        {
            spawnInterval = LevelSettings.Instance.spawnInterval;
            maxCustomersForLevel = LevelSettings.Instance.maxCustomers;
        }

        allTables.AddRange(FindObjectsByType<MasaControl>(FindObjectsSortMode.None));
        timer = spawnInterval;
    }

    private void Update()
    {
        if (spawnedCount < maxCustomersForLevel)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnNewCustomer();
            }
        }

        queueCheckCooldown -= Time.deltaTime;
        if (queueCheckCooldown <= 0f && waitingQueue.Count > 0)
        {
            waitingQueue.RemoveAll(c => c == null);
            if (waitingQueue.Count == 0) return;

            MasaControl freeTable = GetFreeTable();
            if (freeTable != null)
            {
                // Masayý HEMEN kilitle, sonra müþteriyi gönder
                freeTable.ReserveTable();
                queueCheckCooldown = 3.0f; // Yürüme süresi için 3 sn

                CustomerAI next = waitingQueue[0];
                waitingQueue.RemoveAt(0);
                next.AssignTable(freeTable);
                next.AllowEntry();
                UpdateQueuePositions();
            }
            else
            {
                // Boþ masa yok, 1 sn sonra tekrar kontrol et
                queueCheckCooldown = 1f;
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

        // Kuyruk boþsa ve boþ masa varsa direkt içeri
        if (waitingQueue.Count == 0)
        {
            MasaControl freeTable = GetFreeTable();
            if (freeTable != null)
            {
                freeTable.ReserveTable();
                ai.SetupCustomer(freeTable, GetQueuePosition(0));
                ai.AllowEntry();
                return;
            }
        }

        // Kuyruða ekle
        int idx = waitingQueue.Count;
        ai.SetupCustomer(null, GetQueuePosition(idx));
        waitingQueue.Add(ai);
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
            if (waitingQueue[i] != null)
                waitingQueue[i].MoveToQueuePosition(GetQueuePosition(i));
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
            if (!t.IsTableOccupied())
            {
                // Ekstra kontrol: bu masaya atanmýþ aktif müþteri var mý?
                CustomerAI[] active = FindObjectsByType<CustomerAI>(FindObjectsSortMode.None);
                bool hasCustomer = false;
                foreach (var c in active)
                {
                    if (c.GetAssignedTable() == t &&
                        c.State != CustomerAI.CustomerState.Leaving)
                    {
                        hasCustomer = true;
                        break;
                    }
                }
                if (!hasCustomer) return t;
            }
        }
        return null;
    }

    public void RegisterServed()
    {
        servedCount++;
        Debug.Log($"Servis: {servedCount}/{maxCustomersForLevel}");

        if (servedCount >= maxCustomersForLevel)
            LevelComplete();
    }

    private void LevelComplete()
    {
        // ScoreManager deðil DeliveryManager kullan
        int score = DeliveryManager.Instance?.GetScore() ?? 0;
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.SetInt("LevelScore", score);
        PlayerPrefs.SetInt("CompletedLevel",
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        if (WalkoutManager.Instance != null)
            WalkoutManager.Instance.ShowLevelComplete();
        else
        {
            Debug.Log("LEVEL TAMAMLANDI! Puan: " + score);
            Time.timeScale = 0f;
        }
    }
}
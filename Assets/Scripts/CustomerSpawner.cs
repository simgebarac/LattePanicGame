using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Prefab & Noktalar")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform outsideWaitPoint; // Kapý önü bekleme noktasý

    [Header("Level Ayarlarý")]
    [SerializeField] private float spawnInterval = 8f;
    [SerializeField] private int maxCustomersForLevel = 10;

    private List<MasaControl> allTables = new List<MasaControl>();
    private Queue<CustomerAI> waitingQueue = new Queue<CustomerAI>(); // Kapýdaki sýra
    private float timer;
    private int spawnedCount = 0;

    private void Start()
    {
        MasaControl[] tables = FindObjectsByType<MasaControl>(FindObjectsSortMode.None);
        allTables.AddRange(tables);
    }

    private void Update()
    {
        // Yeni müþteri spawn et
        if (spawnedCount < maxCustomersForLevel)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                timer = 0f;
                SpawnNewCustomer();
            }
        }

        // Sýradaki müþteriyi boþ masaya gönder
        ProcessQueue();
    }

    private void SpawnNewCustomer()
    {
        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject obj = Instantiate(customerPrefab, pos, Quaternion.identity);
        CustomerAI ai = obj.GetComponent<CustomerAI>();
        if (ai == null) return;

        // Boþ masa var mý?
        MasaControl freeTable = GetFreeTable();
        if (freeTable != null)
        {
            freeTable.ReserveTable();
            ai.SetupCustomer(freeTable, outsideWaitPoint);
            ai.AllowEntry(); // Direkt içeri gönder
        }
        else
        {
            // Masa yok — kapýda beklet (dummy table, gerçek masa sonra verilecek)
            ai.SetupCustomer(null, outsideWaitPoint);
            waitingQueue.Enqueue(ai);
        }

        spawnedCount++;
    }

    private void ProcessQueue()
    {
        if (waitingQueue.Count == 0) return;

        MasaControl freeTable = GetFreeTable();
        if (freeTable == null) return;

        CustomerAI next = waitingQueue.Dequeue();
        if (next == null) return; // Sabýrsýzlanýp gitti olabilir

        freeTable.ReserveTable();
        next.AssignTable(freeTable); // Masayý ata
        next.AllowEntry();           // Ýçeri gönder
    }

    private MasaControl GetFreeTable()
    {
        foreach (var t in allTables)
            if (!t.IsTableOccupied()) return t;
        return null;
    }
}
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxCustomersForLevel = 10;

    private float timer = 0f;
    private int spawnedCount = 0;

    void Update()
    {
        int activeCount = GameObject.FindGameObjectsWithTag("Customer").Length;

        // Sahnedeki aktif müþteri 2'den azsa ve toplam kota dolmadýysa
        if (activeCount < 2 && spawnedCount < maxCustomersForLevel)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                spawnedCount++;
                timer = 0;
            }
        }
    }
}
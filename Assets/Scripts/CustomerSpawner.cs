using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 5f;

    private float timer = 0f;

    void Update()
    {
        int customerCount = GameObject.FindGameObjectsWithTag("Customer").Length;

        if (customerCount < 2)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                timer = 0;
            }
        }
    }
}
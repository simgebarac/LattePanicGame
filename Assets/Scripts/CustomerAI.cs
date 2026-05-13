using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private KitchenObjectSO myOrder;
    private MasaControl myTable;
    private bool hasArrived = false;

    [Header("UI Referanslarý")]
    public GameObject bubbleCanvas;
    public TextMeshProUGUI bubbleOrderText;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (bubbleCanvas != null) bubbleCanvas.SetActive(false);
    }

    public void SetupCustomer(MasaControl table)
    {
        myTable = table;
        agent = GetComponent<NavMeshAgent>();

        // 1. Agent'ý kapatýp karakteri zemine hizalayalým
        agent.enabled = false;

        // En yakýn NavMesh noktasýný bul ve karakteri oraya ýþýnla
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }

        // 2. Agent'ý tekrar aç ve hedefi ver
        agent.enabled = true;
        if (myTable != null)
        {
            agent.SetDestination(myTable.GetWaitPoint().position);
            Debug.Log("Müþteri zemine oturtuldu ve hedefe yönlendirildi!");
        }
    }

    private void Update()
    {
        if (!hasArrived && agent.hasPath && agent.remainingDistance < 0.5f)
        {
            ArrivedAtTable();
        }
    }

    private void ArrivedAtTable()
    {
        hasArrived = true;
        Debug.Log("Masaya ulaþýldý.");

        if (DeliveryManager.Instance != null)
        {
            myOrder = DeliveryManager.Instance.SpawnNewOrder();
            if (myOrder != null && bubbleCanvas != null)
            {
                bubbleOrderText.text = myOrder.objectName;
                bubbleCanvas.SetActive(true);
            }
            if (myTable != null) myTable.SetOrder(myOrder);
        }
    }

    public void ReceiveOrderAndLeave()
    {
        if (bubbleCanvas != null) bubbleCanvas.SetActive(false);
        hasArrived = false;
        // Çýkýþ noktasý (Burayý sahnendeki bir boþlukla deðiþtir)
        agent.SetDestination(new Vector3(0, 0, -10f));
        Destroy(gameObject, 5f);
    }
}
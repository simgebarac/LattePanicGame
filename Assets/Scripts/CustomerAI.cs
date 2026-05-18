using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class CustomerAI : MonoBehaviour
{
    public float GetPatienceRatio() => patienceTimer / patienceAtTable;
    public void AssignTable(MasaControl table)
    {
        myTable = table;
    }

    // ============ STATE ============
    public enum CustomerState
    {
        WaitingOutside,   // Kapıda bekliyor
        WalkingToTable,   // Masaya yürüyor
        WaitingForOrder,  // "Sipariş almayı bekliyorum"
        OrderTaken,       // Sipariş verildi, kahve bekleniyor
        Drinking,         // Kahveyi içiyor
        Leaving           // Çıkıyor
    }

    public CustomerState State { get; private set; } = CustomerState.WaitingOutside;

    // ============ REFS ============
    private NavMeshAgent agent;
    private MasaControl myTable;
    private KitchenObjectSO myOrder;

    [Header("UI")]
    public GameObject bubbleCanvas;
    public TextMeshProUGUI bubbleOrderText;
    public GameObject patienceBarObject;      // Sabır barı root objesi
    public UnityEngine.UI.Image patienceFill; // Sabır barı doluluk image'ı

    [Header("Ayarlar")]
    public float patienceOutside = 30f;  // Kapıda bekleme süresi
    public float patienceAtTable = 60f;  // Masada bekleme süresi (sipariş sonrası)
    public float drinkDuration = 3f;     // Kahveyi içme süresi

    private float patienceTimer;
    private bool hasArrived = false;

    // ============ INIT ============
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (bubbleCanvas != null) bubbleCanvas.SetActive(false);
        if (patienceBarObject != null) patienceBarObject.SetActive(false);
    }

    public MasaControl GetAssignedTable() => myTable;

    // Spawner tarafından çağrılır
    public void SetupCustomer(MasaControl table, Transform outsideWaitPoint)
    {
        myTable = table;

        // NavMesh warp
        agent.enabled = false;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            transform.position = hit.position;
        agent.enabled = true;
        agent.Warp(transform.position);

        // Önce kapıda bekle
        SetState(CustomerState.WaitingOutside);
        patienceTimer = patienceOutside;

        if (outsideWaitPoint != null)
            agent.SetDestination(outsideWaitPoint.position);

        if (patienceBarObject != null) patienceBarObject.SetActive(true);
    }

    // Spawner "gel içeri" dediğinde çağrılır
    public void AllowEntry()
    {
        if (State != CustomerState.WaitingOutside) return;
        SetState(CustomerState.WalkingToTable);
        patienceTimer = patienceAtTable;

        if (myTable != null && myTable.GetWaitPoint() != null)
        {
            agent.SetDestination(myTable.GetWaitPoint().position);
            StartCoroutine(ArrivalWatchdog(15f));
        }
    }

    // ============ UPDATE ============
    private void Update()
    {
        UpdatePatienceBar();

        switch (State)
        {
            case CustomerState.WaitingOutside:
                // Sabır azalır — çok beklerse gider
                patienceTimer -= Time.deltaTime;
                if (patienceTimer <= 0f)
                    CustomerWalkout();
                break;

            case CustomerState.WalkingToTable:
                CheckArrival();
                break;

            case CustomerState.WaitingForOrder:
                // Sipariş alınmayı bekliyor — sabır azalır
                patienceTimer -= Time.deltaTime;
                if (patienceTimer <= 0f)
                    CustomerWalkout();
                break;

            case CustomerState.OrderTaken:
                // Kahve bekleniyor — sabır azalır
                patienceTimer -= Time.deltaTime;
                if (patienceTimer <= 0f)
                    CustomerWalkout();
                break;
        }
    }

    private void CheckArrival()
    {
        if (hasArrived || !agent.enabled || agent.pathPending) return;
        float dist = Vector3.Distance(transform.position, agent.destination);
        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f || dist < 0.8f)
            ArrivedAtTable();
    }

    // ============ ARRIVAL ============
    private void ArrivedAtTable()
    {
        if (hasArrived) return;
        hasArrived = true;

        agent.ResetPath();
        agent.velocity = Vector3.zero;

        // Masaya dön
        if (myTable != null)
        {
            Vector3 dir = myTable.transform.position - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        SetState(CustomerState.WaitingForOrder);

        // "Sipariş bekliyorum" yazısını göster
        ShowBubble("Sipariş\nbekliyorum...");
    }

    // ============ ORDER ============
    // Oyuncu masaya (E) bastığında MasaControl bu fonksiyonu çağırır
    public void TakeOrder()
    {
        if (State != CustomerState.WaitingForOrder) return;

        if (DeliveryManager.Instance == null) return;
        myOrder = DeliveryManager.Instance.SpawnNewOrder();
        if (myOrder == null) return;

        myTable.SetOrder(myOrder);
        SetState(CustomerState.OrderTaken);
        ShowBubble(myOrder.objectName); // Sipariş adını göster
    }

    // ============ DELIVERY ============
    public void ReceiveOrderAndLeave(float patiencePercent)
    {
        if (State == CustomerState.Leaving) return;
        SetState(CustomerState.Drinking);

        HideBubble();
        if (patienceBarObject != null) patienceBarObject.SetActive(false);

        // Puanı hesapla ve ver
        int score = CalculateScore(patiencePercent);
        ScoreManager.Instance?.AddScore(score);

        // Birkaç saniye içme animasyonu, sonra çık
        StartCoroutine(DrinkAndLeave());
    }

    private IEnumerator DrinkAndLeave()
    {
        // İçiyor efekti — balonu kapat, bekle
        yield return new WaitForSeconds(drinkDuration);
        Leave();
    }

    // ============ WALKOUT ============
    private void CustomerWalkout()
    {
        if (State == CustomerState.Leaving) return;
        SetState(CustomerState.Leaving);

        HideBubble();
        if (patienceBarObject != null) patienceBarObject.SetActive(false);

        ScoreManager.Instance?.AddScore(-5); // Ceza puanı
        WalkoutManager.Instance?.RegisterWalkout(); // Walkout sayacı

        myTable?.ResetTable();
        Leave();
    }

    private void Leave()
    {
        SetState(CustomerState.Leaving);
        myTable?.ResetTable();

        // Çıkış noktasına git ve yok ol
        agent.SetDestination(new Vector3(8.8f, 1f, -8.6f));
        Destroy(gameObject, 5f);
    }

    // ============ HELPERS ============
    private void SetState(CustomerState newState)
    {
        State = newState;
    }

    private void ShowBubble(string text)
    {
        if (bubbleCanvas == null || bubbleOrderText == null) return;
        bubbleOrderText.text = text;
        bubbleCanvas.SetActive(true);
    }

    private void HideBubble()
    {
        if (bubbleCanvas != null) bubbleCanvas.SetActive(false);
    }

    private void UpdatePatienceBar()
    {
        if (patienceFill == null) return;
        float maxPatience = (State == CustomerState.WaitingOutside) ? patienceOutside : patienceAtTable;
        patienceFill.fillAmount = Mathf.Clamp01(patienceTimer / maxPatience);

        // Renk: yeşil → sarı → kırmızı
        patienceFill.color = Color.Lerp(Color.red, Color.green, patienceFill.fillAmount);
    }

    private int CalculateScore(float patiencePercent)
    {
        if (patiencePercent >= 0.75f) return 30;
        if (patiencePercent >= 0.50f) return 20;
        if (patiencePercent >= 0.25f) return 10;
        return 5;
    }

    private IEnumerator ArrivalWatchdog(float timeout)
    {
        float elapsed = 0f;
        while (!hasArrived && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (!hasArrived)
        {
            if (myTable?.GetWaitPoint() != null)
                transform.position = myTable.GetWaitPoint().position;
            ArrivedAtTable();
        }
    }

    private void LateUpdate()
    {
        if (bubbleCanvas != null && bubbleCanvas.activeSelf && Camera.main != null)
        {
            bubbleCanvas.transform.LookAt(
                bubbleCanvas.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
            );
        }
    }
}
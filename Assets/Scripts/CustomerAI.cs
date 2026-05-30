using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

public class CustomerAI : MonoBehaviour
{
    public float GetPatienceRatio() => patienceTimer / patienceAtTable;

    public void AssignTable(MasaControl table)
    {
        myTable = table;
    }

    public void UpdateBubble(string text)
    {
        ShowBubble(text);
    }

    // ============ STATE ============
    public enum CustomerState
    {
        WaitingOutside,
        WalkingToTable,
        WaitingForOrder,
        OrderTaken,
        Drinking,
        Leaving
    }

    public CustomerState State { get; private set; } = CustomerState.WaitingOutside;

    // ============ REFS ============
    private NavMeshAgent agent;
    private MasaControl myTable;
    private OrderData myOrder;
    private Transform outsideWaitPoint;

    [Header("UI")]
    public GameObject bubbleCanvas;
    public TextMeshProUGUI bubbleOrderText;
    public GameObject patienceBarObject;
    public UnityEngine.UI.Image patienceFill;

    [Header("Ayarlar")]
    public float patienceOutside = 30f;
    public float patienceAtTable = 60f;
    public float drinkDuration = 3f;

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

    public void SetupCustomer(MasaControl table, Transform waitPoint)
    {
        myTable = table;
        outsideWaitPoint = waitPoint;

        if (LevelSettings.Instance != null)
        {
            patienceAtTable = LevelSettings.Instance.patienceAtTable;
            patienceOutside = LevelSettings.Instance.patienceOutside;
        }

        agent.enabled = false;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            transform.position = hit.position;
        agent.enabled = true;
        agent.Warp(transform.position);

        SetState(CustomerState.WaitingOutside);
        patienceTimer = patienceOutside;

        if (outsideWaitPoint != null)
            agent.SetDestination(outsideWaitPoint.position);

        if (patienceBarObject != null) patienceBarObject.SetActive(true);
    }

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

    public void MoveToQueuePosition(Transform target)
    {
        outsideWaitPoint = target;
        if (target != null && agent.enabled)
            agent.SetDestination(target.position);
    }

    // ============ UPDATE ============
    private void Update()
    {
        UpdatePatienceBar();

        switch (State)
        {
            case CustomerState.WaitingOutside:
                patienceTimer -= Time.deltaTime;
                if (patienceTimer <= 0f) CustomerWalkout();
                break;

            case CustomerState.WalkingToTable:
                CheckArrival();
                break;

            case CustomerState.WaitingForOrder:
                patienceTimer -= Time.deltaTime;
                if (patienceTimer <= 0f) CustomerWalkout();
                break;

            case CustomerState.OrderTaken:
                patienceTimer -= Time.deltaTime;
                if (patienceTimer <= 0f) CustomerWalkout();
                break;
        }
    }

    private void CheckArrival()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        if (hasArrived || agent.pathPending) return;
        float dist = Vector3.Distance(transform.position, agent.destination);
        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f || dist < 0.8f)
            ArrivedAtTable();
    }

    // ============ ARRIVAL ============
    private void ArrivedAtTable()
    {
        if (hasArrived) return;
        hasArrived = true;

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        if (myTable != null)
        {
            Vector3 dir = myTable.transform.position - transform.position;
            dir.y = 0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }

        SetState(CustomerState.WaitingForOrder);
        ShowBubble("Sipariş\nbekliyorum...");
    }

    // ============ ORDER ============
    public void TakeOrder()
    {
        if (State != CustomerState.WaitingForOrder) return;
        if (DeliveryManager.Instance == null) return;

        myOrder = DeliveryManager.Instance.SpawnNewOrder();
        if (myOrder == null) return;

        myTable.SetOrderData(myOrder);
        SetState(CustomerState.OrderTaken);
        ShowBubble(myOrder.GetOrderText());
    }

    // ============ DELIVERY ============
    public void ReceiveOrderAndLeave(float patiencePercent)
    {
        if (State == CustomerState.Leaving) return;
        SetState(CustomerState.Drinking);
        HideBubble();
        if (patienceBarObject != null) patienceBarObject.SetActive(false);

        int score = CalculateScore(patiencePercent);
        DeliveryManager.Instance?.AddScore(score);

        StartCoroutine(DrinkAndLeave());
    }

    private IEnumerator DrinkAndLeave()
    {
        yield return new WaitForSeconds(drinkDuration);

        // Masa ANCAK müşteri kalkarken boşalsın
        myTable?.ResetTable();

        FindFirstObjectByType<CustomerSpawner>()?.RegisterServed();
        FindFirstObjectByType<CustomerSpawner>()?.RemoveFromQueue(this);

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.SetDestination(new Vector3(8.8f, 1f, -8.6f));

        Destroy(gameObject, 5f);
    }

    private void CustomerWalkout()
    {
        if (State == CustomerState.Leaving) return;
        SetState(CustomerState.Leaving);

        HideBubble();
        if (patienceBarObject != null) patienceBarObject.SetActive(false);

        DeliveryManager.Instance?.AddScore(-5);
        WalkoutManager.Instance?.RegisterWalkout();

        myTable?.ResetTable();
        FindFirstObjectByType<CustomerSpawner>()?.RemoveFromQueue(this);
        FindFirstObjectByType<CustomerSpawner>()?.RegisterCustomerGone(); // YENİ

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.SetDestination(new Vector3(8.8f, 1f, -8.6f));
        Destroy(gameObject, 5f);
    }

    // Leave() artık kullanılmıyor — DrinkAndLeave içine taşındı
    private void Leave()
    {
        SetState(CustomerState.Leaving);
        myTable?.ResetTable();

        FindFirstObjectByType<CustomerSpawner>()?.RegisterServed();
        FindFirstObjectByType<CustomerSpawner>()?.RemoveFromQueue(this);

        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.SetDestination(new Vector3(8.8f, 1f, -8.6f));
        Destroy(gameObject, 5f);
    }

    // ============ HELPERS ============
    private void SetState(CustomerState newState) => State = newState;

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

        if (patienceBarObject != null && patienceBarObject.activeSelf && Camera.main != null)
        {
            patienceBarObject.transform.LookAt(
                patienceBarObject.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up
            );
        }
    }
}
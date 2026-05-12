using UnityEngine;
using UnityEngine.AI;

public class CustomerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private MasaControl targetTable;
    private bool hasOrderGiven = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        FindEmptyTable();
    }

    void FindEmptyTable()
    {
        MasaControl[] allTables = GameObject.FindObjectsOfType<MasaControl>();

        foreach (MasaControl table in allTables)
        {
            if (!table.isReserved)
            {
                table.isReserved = true;
                targetTable = table;
                agent.SetDestination(targetTable.waitPoint.position);
                return;
            }
        }
        Debug.Log("Boþ masa yok, kapýda bekliyorum...");
    }

    void Update()
    {
        if (targetTable != null && !hasOrderGiven)
        {
            // NavMeshAgent'ýn hedefe ulaþýp ulaþmadýðýný kontrol et
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                hasOrderGiven = true;
                // Masayý ebeveyn yap (Müþteriyi masanýn çocuðu yapýyoruz ki bulmasý kolay olsun)
                transform.parent = targetTable.transform;
                // Masaya haber ver
                targetTable.CustomerArrived();
            }
        }
    }

    public void FinishAndLeave()
    {
        if (targetTable != null) targetTable.ResetTable();
        Destroy(gameObject);
    }
}
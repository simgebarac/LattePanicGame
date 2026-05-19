using UnityEngine;

public class MasaControl : BaseCounter
{
    [SerializeField] private Transform waitPoint;
    private KitchenObjectSO currentOrder;
    private bool isOccupied = false;

    public Transform GetWaitPoint() => waitPoint;
    public bool IsTableOccupied() => isOccupied;

    // Spawner veya müþteri masayý rezerve ettiðinde kilitlenir
    public void ReserveTable()
    {
        isOccupied = true;
    }

    public void SetOrder(KitchenObjectSO order)
    {
        currentOrder = order;
        isOccupied = true; // Sipariþ alýnsa bile masa HÂLÂ DOLUDUR!
    }

    public override void Interact(Player player)
    {
        if (!isOccupied) return;
        CustomerAI customer = GetCustomerAtThisTable();

        // 1. AÞAMA: Sipariþ al
        if (customer != null && customer.State == CustomerAI.CustomerState.WaitingForOrder)
        {
            customer.TakeOrder();
            return;
        }

        // 2. AÞAMA: Kahve teslim et
        if (currentOrder != null && player.HasKitchenObject())
        {
            KitchenObjectSO playerItem = player.GetKitchenObject().GetKitchenObjectSO();
            if (playerItem == currentOrder)
            {
                float patiencePercent = customer != null ? Mathf.Clamp01(customer.GetPatienceRatio()) : 1f;

                if (DeliveryManager.Instance != null)
                    DeliveryManager.Instance.DeliverOrder(currentOrder);

                player.GetKitchenObject().DestroySelf();
                currentOrder = null; // Sipariþi temizle ama masayý DOLU tut

                // ResetTable() BURADAN KALDIRILDI
                // Masa müþteri Leave() çaðýrýnca CustomerAI üzerinden sýfýrlanacak
                customer?.ReceiveOrderAndLeave(patiencePercent);
            }
            else
            {
                Debug.LogWarning($"Yanlýþ ürün! Beklenen: {currentOrder.objectName}");
            }
        }
    }

    private CustomerAI GetCustomerAtThisTable()
    {
        CustomerAI[] all = FindObjectsByType<CustomerAI>(FindObjectsSortMode.None);
        foreach (var c in all)
        {
            if (c.GetAssignedTable() == this && c.State != CustomerAI.CustomerState.Leaving)
                return c;
        }
        return null;
    }

    public void ResetTable()
    {
        currentOrder = null;
        isOccupied = false; // Koltuk ancak müþteri kalkýnca boþa düþer
    }
}
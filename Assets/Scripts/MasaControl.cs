using UnityEngine;
using static CustomerAI;

public class MasaControl : BaseCounter
{
    [SerializeField] private Transform waitPoint;
    private KitchenObjectSO currentOrder;
    private bool isOccupied = false;

    public Transform GetWaitPoint() => waitPoint;
    public bool IsTableOccupied() => isOccupied;

    public void ReserveTable()
    {
        isOccupied = true;
    }

    public void SetOrder(KitchenObjectSO order)
    {
        currentOrder = order;
        isOccupied = true;
    }

    public override void Interact(Player player)
    {
        if (!isOccupied) return;

        // Müþteri sipariþ bekliyorsa — oyuncu E'ye basýnca sipariþ al
        CustomerAI customer = GetCustomerAtThisTable();
        if (customer != null && customer.State == CustomerState.WaitingForOrder)
        {
            customer.TakeOrder();
            return;
        }

        // Müþteri sipariþ verdiyse — doðru kahveyi teslim et
        if (currentOrder != null && player.HasKitchenObject())
        {
            KitchenObjectSO playerItem = player.GetKitchenObject().GetKitchenObjectSO();

            if (playerItem == currentOrder)
            {
                float patiencePercent = customer != null ?
                    (customer.State == CustomerAI.CustomerState.OrderTaken ?
                     Mathf.Clamp01(customer.GetPatienceRatio()) : 1f) : 1f;

                if (DeliveryManager.Instance != null)
                    DeliveryManager.Instance.DeliverOrder(currentOrder);

                player.GetKitchenObject().DestroySelf();
                customer?.ReceiveOrderAndLeave(patiencePercent);
                ResetTable();
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
            if (c.GetAssignedTable() == this) return c;
        return null;
    }

    public void ResetTable()
    {
        currentOrder = null;
        isOccupied = false;
    }
}
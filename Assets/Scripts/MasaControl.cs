using UnityEngine;

public class MasaControl : BaseCounter
{
    [SerializeField] private Transform waitPoint;
    private OrderData currentOrder;
    private bool isOccupied = false;

    public Transform GetWaitPoint() => waitPoint;
    public bool IsTableOccupied() => isOccupied;
    public void ReserveTable() => isOccupied = true;

    public void SetOrderData(OrderData order)
    {
        currentOrder = order;
        isOccupied = true;
    }

    public override void Interact(Player player)
    {
        if (!isOccupied) return;

        CustomerAI customer = GetCustomerAtThisTable();

        // 1. Sipariþ al
        if (customer != null && customer.State == CustomerAI.CustomerState.WaitingForOrder)
        {
            customer.TakeOrder();
            return;
        }

        if (currentOrder == null || !player.HasKitchenObject()) return;

        KitchenObjectSO playerItem = player.GetKitchenObject().GetKitchenObjectSO();

        // ÝÇECEK TESLÝMÝ
        if (playerItem == currentOrder.drink && !currentOrder.drinkDelivered)
        {
            currentOrder.drinkDelivered = true;
            player.GetKitchenObject().DestroySelf();

            if (currentOrder.HasDessert && !currentOrder.dessertDelivered)
            {
                // Tatlý hala bekleniyor
                customer?.UpdateBubble("+" + currentOrder.dessert.objectName);
                Debug.Log("Ýçecek tamam, tatlý bekleniyor.");
            }
            else
            {
                // Tatlý yoktu veya zaten verildi — bitir
                FinishOrder(customer);
            }
            return;
        }

        // TATLI TESLÝMÝ — sýra zorunluluðu YOK
        if (currentOrder.HasDessert &&
            playerItem == currentOrder.dessert &&
            !currentOrder.dessertDelivered)
        {
            currentOrder.dessertDelivered = true;
            player.GetKitchenObject().DestroySelf();

            if (currentOrder.drink != null && !currentOrder.drinkDelivered)
            {
                // Ýçecek hala bekleniyor
                customer?.UpdateBubble(currentOrder.drink.objectName + "\n(içecek bekleniyor)");
                Debug.Log("Tatlý tamam, içecek bekleniyor.");
            }
            else
            {
                // Ýçecek yoktu veya zaten verildi — bitir
                FinishOrder(customer);
            }
            return;
        }

        Debug.LogWarning($"Yanlýþ ürün! Beklenen: {currentOrder.GetOrderText()}");
    }

    private void FinishOrder(CustomerAI customer)
    {
        float patiencePercent = customer != null ?
            Mathf.Clamp01(customer.GetPatienceRatio()) : 1f;
        customer?.ReceiveOrderAndLeave(patiencePercent);
        ResetTable();
    }

    private CustomerAI GetCustomerAtThisTable()
    {
        CustomerAI[] all = FindObjectsByType<CustomerAI>(FindObjectsSortMode.None);
        foreach (var c in all)
            if (c.GetAssignedTable() == this &&
                c.State != CustomerAI.CustomerState.Leaving)
                return c;
        return null;
    }

    public void ResetTable()
    {
        currentOrder = null;
        isOccupied = false;
    }
}
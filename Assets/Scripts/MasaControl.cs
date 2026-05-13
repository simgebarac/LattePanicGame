using UnityEngine;

public class MasaControl : BaseCounter
{
    [SerializeField] private Transform waitPoint;
    private KitchenObjectSO currentOrder;
    private bool isOccupied = false;

    public Transform GetWaitPoint() => waitPoint;

    public void SetOrder(KitchenObjectSO order)
    {
        currentOrder = order;
        isOccupied = true;
    }

    public override void Interact(Player player)
    {
        if (isOccupied && currentOrder != null && player.HasKitchenObject())
        {
            if (player.GetKitchenObject().GetKitchenObjectSO() == currentOrder)
            {
                // Doðru Ürün!
                DeliveryManager.Instance.DeliverOrder(currentOrder);
                player.GetKitchenObject().DestroySelf();

                CustomerAI customer = GetComponentInChildren<CustomerAI>();
                if (customer != null) customer.ReceiveOrderAndLeave();

                ResetTable();
            }
        }
    }

    private void ResetTable()
    {
        currentOrder = null;
        isOccupied = false;
    }
}
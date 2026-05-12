using UnityEngine;

public class MasaControl : BaseCounter // Eðer BaseCounter hata verirse MonoBehavour yapabilirsin
{
    // HATA VEREN SATIRI SÝLDÝK VEYA YORUMA ALDIK
    // [SerializeField] private MasaBubbleUI bubbleUI; 

    private KitchenObjectSO currentOrder;
    public bool isReserved = false;
    public Transform waitPoint;
    public bool isOccupied = false;

    public void SetMasaOrder(KitchenObjectSO order)
    {
        currentOrder = order;
        isOccupied = true;
    }

    public override void Interact(Player player)
    {
        // Profesyonel Etkileþim: Oyuncu masaya kahve koyuyor
        if (!HasKitchenObject() && isOccupied)
        {
            if (player.HasKitchenObject() && player.GetKitchenObject().GetKitchenObjectSO() == currentOrder)
            {

                // Doðru kahve! Masaya býrak
                player.GetKitchenObject().SetKitchenObjectParent(this);

                // Puan ver ve UI güncelle
                DeliveryManager.Instance.DeliverCorrectOrder();

                // Müþteriyi gönder
                CustomerAI customer = GetComponentInChildren<CustomerAI>();
                if (customer != null) customer.FinishAndLeave();

                ResetTable();
            }
        }
    }

    public void ResetTable()
    {
        isReserved = false;
        isOccupied = false;
        currentOrder = null;
    }
}
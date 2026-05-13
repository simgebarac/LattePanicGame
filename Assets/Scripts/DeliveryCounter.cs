using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            // Oyuncunun elindeki objenin ne olduðunu alýyoruz
            KitchenObjectSO deliveredObjectSO = player.GetKitchenObject().GetKitchenObjectSO();

            // Yeni sistemdeki DeliverOrder fonksiyonunu çaðýrýyoruz
            // Bu fonksiyon listenin içinde bu kahve var mý diye kontrol eder
            DeliveryManager.Instance.DeliverOrder(deliveredObjectSO);

            // Kahveyi teslim ettik, elindekini yok et
            player.GetKitchenObject().DestroySelf();
        }
    }
}
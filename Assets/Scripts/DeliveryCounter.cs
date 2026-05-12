using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObjectSO deliveredObject = player.GetKitchenObject().GetKitchenObjectSO();

            if (deliveredObject == DeliveryManager.Instance.GetCurrentOrder())
            {
                Debug.Log("Doðru Teslimat!");
                // Doðru kullaným budur:
                DeliveryManager.Instance.DeliverCorrectOrder();

                player.GetKitchenObject().DestroySelf();

                // Müþteriyi bul ve dükkandan gönder (yok et)
                // Bu sayede Spawner yeni birini üretebilir
                GameObject customer = GameObject.FindGameObjectWithTag("Customer");
                if (customer != null)
                {
                    Destroy(customer);
                }
            }
            else
            {
                Debug.Log("Yanlýþ Kahve!");
            }
        }
    }
}
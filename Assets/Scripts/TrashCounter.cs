using UnityEngine;

public class TrashCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        // Eðer oyuncunun elinde bir eþya varsa
        if (player.HasKitchenObject())
        {
            // Elindeki eþyayý dükkandan tamamen sil/yok et!
            player.GetKitchenObject().DestroySelf();

            // Ýstersen buraya çöp atma ses efekti de ekleyebiliriz kanka
            Debug.Log("Çöp fýrlatýldý, dükkan temizlendi!");
        }
    }
}
using UnityEngine;

public class ContainerCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO; // Elimize gelecek olan nesne

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // 1. KONTROL: SO kutusu boþ mu?
            if (kitchenObjectSO == null)
            {
                Debug.LogError("DÝKKAT: Counter üzerindeki Kitchen Object SO kutusu boþ!");
                return;
            }

            // 2. OLUÞTURMA: Prefab'ý oluþtur
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

            // 3. ATAMA: Oyuncunun eline ver
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);
        }
    }
}
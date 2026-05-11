using UnityEngine;

public class ContainerCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO; // Elimize gelecek olan nesne

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            // Eðer oyuncunun eli boþsa, yeni bir tane oluþtur ve direkt eline ver
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
            kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

            Debug.Log("Masadan bir kahve aldýn!");
        }
    }
}
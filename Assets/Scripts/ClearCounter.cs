using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectParent
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private KitchenObject kitchenObject;

    public override void Interact(Player player)
    {
        if (kitchenObject == null)
        {
            // MASA BOÞSA: Eðer Player'ýn elinde bir þey yoksa masada yeni bir tane oluþtur
            if (!player.HasKitchenObject())
            {
                Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
                kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
            }
        }
        else
        {
            // MASA DOLUYSA:
            if (player.HasKitchenObject())
            {
                // Player'ýn eli de doluysa þimdilik bir þey yapma (veya yer deðiþtir)
            }
            else
            {
                // Player'ýn eli boþsa masadakini Player'a ver
                kitchenObject.SetKitchenObjectParent(player);
            }
        }
    }

    // IKitchenObjectParent Fonksiyonlarý (Burasý ayný kalýyor)
    public Transform GetKitchenObjectFollowTransform() => counterTopPoint;
    public void SetKitchenObject(KitchenObject kitchenObject) => this.kitchenObject = kitchenObject;
    public KitchenObject GetKitchenObject() => kitchenObject;
    public void ClearKitchenObject() => kitchenObject = null;
    public bool HasKitchenObject() => kitchenObject != null;
}
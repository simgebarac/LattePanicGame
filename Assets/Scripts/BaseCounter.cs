using UnityEngine;

// IKitchenObjectParent arayüzünü ekleyerek tüm masalarýn ortak bir dilde 
// nesne taþýyabilmesini saðlýyoruz.
public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{
    // Üzerinde duran nesneyi tutan deðiþken (Private kalarak güvenliði saðlar)
    private KitchenObject kitchenObject;

    // Çocuk sýnýflar (ClearCounter vb.) bu fonksiyonun içine kendi mantýklarýný yazabilir.
    public virtual void Interact(Player player)
    {
        Debug.Log("BaseCounter.Interact();");
    }

    // KRÝTÝK DÜZELTME: Baþýna 'virtual' eklendi.
    // Bu sayede çocuk sýnýflar 'override' ederek nesnenin duracaðý noktayý (CounterTopPoint) deðiþtirebilir.
    public virtual Transform GetKitchenObjectFollowTransform()
    {
        return transform;
    }

    // --- IKitchenObjectParent Arayüzü Fonksiyonlarý ---

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
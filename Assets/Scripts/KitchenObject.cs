using UnityEngine;

public class KitchenObject : MonoBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        // Eski ebeveynden kendini temizle
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        // Yeni ebeveyni ata
        this.kitchenObjectParent = kitchenObjectParent;

        // Yeni ebeveyne "senin üzerinde ben varým" de
        kitchenObjectParent.SetKitchenObject(this);

        // Görsel olarak yeni tutma noktasýna git ve sýfýrlan
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }
    // KitchenObject.cs içindeki mevcut kodlarýn altýna ekle:
    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }
}
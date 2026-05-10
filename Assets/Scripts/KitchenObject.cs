using UnityEngine;

public class KitchenObject : MonoBehaviour
{

    [Header("Ayarlar")]
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    // Bu nesnenin kimlik bilgilerini (ad, model, ikon) döndürür
    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    // Nesnenin sahibini (Masa veya Player) deðiþtiren ana fonksiyon
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        // 1. Eðer zaten bir ebeveyni (sahibi) varsa, eski sahibine "ben gidiyorum" de
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        // 2. Yeni sahibini ata
        this.kitchenObjectParent = kitchenObjectParent;

        // 3. Yeni sahibine "artýk ben senin üzerindeyim" de
        kitchenObjectParent.SetKitchenObject(this);

        // 4. Görsel olarak objeyi yeni sahibinin tutma noktasýna (HoldPoint/TopPoint) baðla
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();

        // 5. Yerel pozisyonu sýfýrla ki tam tutma noktasýnýn merkezine otursun
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }
}
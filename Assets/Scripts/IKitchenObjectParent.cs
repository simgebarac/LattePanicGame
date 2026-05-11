using UnityEngine;

public interface IKitchenObjectParent
{
    // Nesnenin fiziksel olarak nereye yapýþacaðýný söyler (HoldPoint veya TopPoint)
    Transform GetKitchenObjectFollowTransform();

    // Sahibi olduðu nesneyi kaydeder
    void SetKitchenObject(KitchenObject kitchenObject);

    // Üzerindeki nesneyi geri verir
    KitchenObject GetKitchenObject();

    // Üzerindeki nesneyi siler (boþaltýr)
    void ClearKitchenObject();

    // Üzerinde bir þey var mý yok mu kontrol eder
    bool HasKitchenObject();
}
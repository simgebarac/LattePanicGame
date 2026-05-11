using UnityEngine;

public class ClearCounter : BaseCounter
{
    [Header("Ayarlar")]
    [SerializeField] private KitchenObjectSO kitchenObjectSO; // Test için (isteðe baðlý)
    [SerializeField] private Transform counterTopPoint;    // Eþyanýn duracaðý nokta

    // DÝKKAT: 'kitchenObject' deðiþkenini buradan sildik çünkü BaseCounter içinde zaten var.
    // DÝKKAT: GetKitchenObject, SetKitchenObject gibi fonksiyonlarý sildik çünkü BaseCounter'da varlar.

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            // --- DURUM 1: MASANIN ÜSTÜ BOÞ ---
            if (player.HasKitchenObject())
            {
                // Oyuncunun elinde bir þey varsa: MASAYA BIRAK
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                // Oyuncunun eli de boþsa: Þimdilik bir þey yapma
            }
        }
        else
        {
            // --- DURUM 2: MASANIN ÜSTÜ DOLU ---
            if (player.HasKitchenObject())
            {
                // Hem masa hem el doluysa: Þimdilik bir þey yapma (Ýleride tabak sistemi gelecek)
                Debug.Log("Hem masa hem el dolu!");
            }
            else
            {
                // Oyuncunun eli boþsa: MASADAKÝNÝ ELÝNE AL
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    // Bu fonksiyon BaseCounter'daki abstract/virtual metodu doldurur.
    // Eþyanýn tam olarak nerede duracaðýný sisteme söyler.
    public override Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
}
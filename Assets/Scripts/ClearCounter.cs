using UnityEngine;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform counterTopPoint; // Eþyanýn duracaðý nokta

    public override void Interact(Player player)
    {
        Debug.Log("ClearCounter: Etkilesime girildi!");

        // Videodaki sonraki adýmlarda buraya eþya koyma/alma mantýðý gelecek
    }
}
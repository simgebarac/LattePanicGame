using UnityEngine;

// Bu satýr, Unity içinde sað týklayýp yeni bir eþya (Bardak, Domates vb.) oluþturmaný saðlar
[CreateAssetMenu(fileName = "NewKitchenObject", menuName = "ScriptableObjects/KitchenObject")]
public class KitchenObjectSO : ScriptableObject
{

    public Transform prefab;     // Eþyanýn 3D modeli
    public Sprite sprite;        // UI'da görünecek simgesi
    public string objectName;    // Eþyanýn adý
}
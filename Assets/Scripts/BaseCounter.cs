using UnityEngine;

public class BaseCounter : MonoBehaviour
{
    // Virtual yaparak ClearCounter içinde bu fonksiyonu ezebileceðiz (override)
    public virtual void Interact(Player player)
    {
        Debug.Log("BaseCounter.Interact();");
    }
}
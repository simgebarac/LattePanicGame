using UnityEngine;

[System.Serializable]
public class OrderData
{
    public KitchenObjectSO drink;
    public KitchenObjectSO dessert;
    public bool drinkDelivered = false;
    public bool dessertDelivered = false; // YENİ

    public bool HasDessert => dessert != null;

    public bool IsComplete()
    {
        bool drinkDone = drink == null || drinkDelivered;
        bool dessertDone = dessert == null || dessertDelivered;
        return drinkDone && dessertDone;
    }

    public string GetOrderText()
    {
        string drinkText = drink != null ?
            (drinkDelivered ? $"✓ {drink.objectName}" : drink.objectName) : "";
        string dessertText = dessert != null ?
            (dessertDelivered ? $"✓ {dessert.objectName}" : $"+{dessert.objectName}") : "";

        if (drinkText != "" && dessertText != "")
            return $"{drinkText}\n{dessertText}";
        if (drinkText != "") return drinkText;
        return dessertText;
    }
}
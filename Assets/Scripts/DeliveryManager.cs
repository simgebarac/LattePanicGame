using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private List<KitchenObjectSO> recipeList;
    private List<KitchenObjectSO> activeOrders = new List<KitchenObjectSO>();

    public TextMeshProUGUI scoreText;
    private int score = 0;

    private void Awake() { Instance = this; }

    public KitchenObjectSO SpawnNewOrder()
    {
        if (recipeList.Count > 0)
        {
            KitchenObjectSO order = recipeList[Random.Range(0, recipeList.Count)];
            activeOrders.Add(order);
            return order;
        }
        return null;
    }

    public void DeliverOrder(KitchenObjectSO order)
    {
        if (activeOrders.Contains(order))
        {
            activeOrders.Remove(order);
            score += 100;
            if (scoreText != null) scoreText.text = "Puan: " + score;
        }
    }
}
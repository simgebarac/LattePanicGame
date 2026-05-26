using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [Header("Sipariþ Listesi")]
    [SerializeField] private List<KitchenObjectSO> drinkList;
    [SerializeField] private List<KitchenObjectSO> dessertList;

    [Header("Tatlý Þansý (0=yok, 1=hep)")]
    [Range(0f, 1f)]
    [SerializeField] private float dessertChance = 0f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private List<OrderData> activeOrders = new List<OrderData>();
    private int score = 0;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        // LevelSettings varsa tatlý þansýný oradan al
        if (LevelSettings.Instance != null)
            dessertChance = LevelSettings.Instance.dessertChance;
    }

    public OrderData SpawnNewOrder()
    {
        if (drinkList == null || drinkList.Count == 0) return null;

        OrderData order = new OrderData();
        order.drink = drinkList[Random.Range(0, drinkList.Count)];

        if (dessertList != null && dessertList.Count > 0 && Random.value < dessertChance)
            order.dessert = dessertList[Random.Range(0, dessertList.Count)];

        activeOrders.Add(order);
        return order;
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (score < 0) score = 0;
        UpdateScoreUI();
    }

    public int GetScore() => score;

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Puan: " + score;
    }
}
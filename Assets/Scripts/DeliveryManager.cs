using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro kullandýðýn için bu þart!

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI orderText; // Hierarchy'den sürükle
    [SerializeField] private TextMeshProUGUI scoreText; // Hierarchy'den sürükle
    [SerializeField] private List<KitchenObjectSO> recipeList; // SO'larý at

    private KitchenObjectSO currentOrder;
    private int score = 0;

    private void Awake() { Instance = this; }

    private void Start()
    {
        if (scoreText != null) scoreText.text = "Puan: 0";
        if (orderText != null) orderText.text = "Sipariþ: Bekleniyor...";
    }

    public KitchenObjectSO SpawnNewOrder()
    {
        if (recipeList != null && recipeList.Count > 0)
        {
            currentOrder = recipeList[Random.Range(0, recipeList.Count)];

            // UI'ý güncelle
            if (orderText != null) orderText.text = "Sipariþ: " + currentOrder.objectName;

            return currentOrder; // SEÇÝLEN SÝPARÝÞÝ MASAYA GÖNDER
        }
        return null;
    }

    public void DeliverCorrectOrder()
    {
        score += 100;
        if (scoreText != null) scoreText.text = "Puan: " + score;
        if (orderText != null) orderText.text = "Sipariþ: Bekleniyor...";

        CustomerAI activeCustomer = GameObject.FindAnyObjectByType<CustomerAI>();
        if (activeCustomer != null) activeCustomer.FinishAndLeave();
    }

    public KitchenObjectSO GetCurrentOrder() => currentOrder;
}
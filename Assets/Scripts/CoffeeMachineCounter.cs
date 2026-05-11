using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CoffeeMachineCounter : BaseCounter
{
    [Header("UI Elementleri (İkonlar)")]
    [SerializeField] private GameObject sutIcon;
    [SerializeField] private GameObject kahveIcon;
    [SerializeField] private GameObject suIcon;
    [SerializeField] private Slider progressBar;

    [Header("Spawn Ayarları")]
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private KitchenObjectSO espressoSO;
    [SerializeField] private KitchenObjectSO latteSO;
    [SerializeField] private KitchenObjectSO americanoSO;

    private List<string> ingredients = new List<string>();
    private float timer = 0f;
    private float maxTimer = 3f;

    public override void Interact(Player player)
    {
        if (HasKitchenObject())
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                ResetMachine();
            }
            return;
        }

        if (player.HasKitchenObject())
        {
            string itemName = player.GetKitchenObject().GetKitchenObjectSO().objectName.ToLower();

            // 🚫 GÜVENLİK KİLİDİ: Sadece geçerli malzemeleri ve sadece 1'er tane al
            if (itemName == "kahve" && !ingredients.Contains("kahve"))
            {
                AddIngredient("kahve", player);
            }
            else if ((itemName == "sut" || itemName == "süt") && !ingredients.Contains("sut"))
            {
                // Süt ekleyebilmek için içinde su olmamalı (Latte kuralı)
                if (!ingredients.Contains("su"))
                    AddIngredient("sut", player);
                else
                    Debug.Log("Su olan kahveye süt eklenemez!");
            }
            else if (itemName == "su" && !ingredients.Contains("su"))
            {
                // Su ekleyebilmek için içinde süt olmamalı (Americano kuralı)
                if (!ingredients.Contains("sut"))
                    AddIngredient("su", player);
                else
                    Debug.Log("Süt olan kahveye su eklenemez!");
            }
        }
    }

    private void AddIngredient(string ingredientName, Player player)
    {
        ingredients.Add(ingredientName);

        // Elindeki nesneyi yok et
        Destroy(player.GetKitchenObject().gameObject);
        player.ClearKitchenObject();

        // İKONLARI ANINDA GÜNCELLE
        UpdateIcons();
    }

    private void Update()
    {
        bool isLookingAtMe = Player.Instance != null && Player.Instance.GetSelectedCounter() == this;

        // PİŞİRME KONTROLÜ
        if (ingredients.Contains("kahve") && Input.GetKey(KeyCode.E) && !HasKitchenObject() && isLookingAtMe)
        {
            SetAllIconsVisible(false); // Pişirirken hepsini gizle

            if (progressBar != null) progressBar.gameObject.SetActive(true);
            timer += Time.deltaTime;
            if (progressBar != null) progressBar.value = timer / maxTimer;

            if (timer >= maxTimer) FinishCooking();
        }
        else
        {
            // Tuşu bıraktığında veya başka yere baktığında
            timer = 0;
            if (progressBar != null)
            {
                progressBar.value = 0;
                progressBar.gameObject.SetActive(false);
            }

            // Ürün yoksa ikonları geri getir
            if (!HasKitchenObject()) UpdateIcons();
        }
    }

    // İkonları listenin durumuna göre açıp kapatan fonksiyon
    private void UpdateIcons()
    {
        if (kahveIcon != null) kahveIcon.SetActive(ingredients.Contains("kahve"));
        if (sutIcon != null) sutIcon.SetActive(ingredients.Contains("sut"));
        if (suIcon != null) suIcon.SetActive(ingredients.Contains("su"));
    }

    private void SetAllIconsVisible(bool state)
    {
        if (kahveIcon != null) kahveIcon.SetActive(state);
        if (sutIcon != null) sutIcon.SetActive(state);
        if (suIcon != null) suIcon.SetActive(state);
    }

    private void FinishCooking()
    {
        KitchenObjectSO outputSO = null;

        bool hasKahve = ingredients.Contains("kahve");
        bool hasSut = ingredients.Contains("sut");
        bool hasSu = ingredients.Contains("su");

        if (hasKahve && hasSut) outputSO = latteSO;
        else if (hasKahve && hasSu) outputSO = americanoSO;
        else if (hasKahve) outputSO = espressoSO;

        if (outputSO != null && outputSO.prefab != null)
        {
            Transform coffeeTransform = Instantiate(outputSO.prefab, counterTopPoint);
            coffeeTransform.localPosition = Vector3.zero;
            coffeeTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }

        ingredients.Clear();
        UpdateIcons();
    }

    private void ResetMachine()
    {
        ingredients.Clear();
        timer = 0;
        UpdateIcons();
    }

    public override Transform GetKitchenObjectFollowTransform() => counterTopPoint;
}
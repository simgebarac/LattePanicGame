using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CoffeeMachineCounter : BaseCounter
{
    [Header("UI Elementleri")]
    [SerializeField] private GameObject sutIcon;
    [SerializeField] private GameObject kahveIcon;
    [SerializeField] private GameObject suIcon;
    [SerializeField] private Slider progressBar;

    [Header("Spawn Ayarlarý")]
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
            }
            return;
        }

        if (player.HasKitchenObject())
        {
            string itemName = player.GetKitchenObject().GetKitchenObjectSO().objectName;

            if (itemName == "Kahve" && !ingredients.Contains("Kahve"))
            {
                ingredients.Add("Kahve");
                if (kahveIcon != null) kahveIcon.SetActive(true);
                Destroy(player.GetKitchenObject().gameObject);
                player.ClearKitchenObject();
            }
            else if (itemName == "Sut" && !ingredients.Contains("Sut"))
            {
                ingredients.Add("Sut");
                if (sutIcon != null) sutIcon.SetActive(true);
                Destroy(player.GetKitchenObject().gameObject);
                player.ClearKitchenObject();
            }
        }
    }

    private void Update()
    {
        if (ingredients.Contains("Kahve") && Input.GetKey(KeyCode.E) && !HasKitchenObject())
        {
            // Piþirme baþlayýnca ikonu gizle, barý göster
            if (kahveIcon != null) kahveIcon.SetActive(false);
            if (sutIcon != null) sutIcon.SetActive(false);

            if (progressBar != null) progressBar.gameObject.SetActive(true);
            timer += Time.deltaTime;
            if (progressBar != null) progressBar.value = timer / maxTimer;

            if (timer >= maxTimer)
            {
                FinishCooking();
            }
        }
        else
        {
            // Elini çekerse ikon geri gelsin, bar gizlensin
            if (ingredients.Contains("Kahve") && !HasKitchenObject())
            {
                if (kahveIcon != null) kahveIcon.SetActive(true);
            }

            timer = 0;
            if (progressBar != null)
            {
                progressBar.value = 0;
                progressBar.gameObject.SetActive(false);
            }
        }
    }

    private void FinishCooking()
    {
        KitchenObjectSO outputSO = null;
        if (ingredients.Contains("Kahve") && ingredients.Contains("Sut")) outputSO = latteSO;
        else if (ingredients.Contains("Kahve")) outputSO = espressoSO;

        if (outputSO != null && outputSO.prefab != null)
        {
            Transform coffeeTransform = Instantiate(outputSO.prefab, counterTopPoint);
            coffeeTransform.localPosition = Vector3.zero;
            coffeeTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
        ResetMachine();
    }

    private void ResetMachine()
    {
        ingredients.Clear();
        if (sutIcon != null) sutIcon.SetActive(false);
        if (kahveIcon != null) kahveIcon.SetActive(false);
        if (suIcon != null) suIcon.SetActive(false);
        timer = 0;
    }

    public override Transform GetKitchenObjectFollowTransform()
    {
        return counterTopPoint;
    }
}
using UnityEngine;

public class RecipeBookManager : MonoBehaviour
{
    [Header("Tarif Defteri UI Paneli")]
    [SerializeField] private GameObject recipeBookPanel;

    private bool isRecipeBookOpen = false;

    void Update()
    {
        // T tuþuna basýldýðýnda tetiklenir
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Eðer Pause menüsü veya baþka bir panel açýksa açýlmasýný engellemek için kontrol koyabilirsin
            ToggleRecipeBook();
        }
    }

    public void ToggleRecipeBook()
    {
        isRecipeBookOpen = !isRecipeBookOpen;

        if (isRecipeBookOpen)
        {
            // Paneli aç ve oyunu dondur
            recipeBookPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Tarif Defteri Açýldý - Oyun Durduruldu");
        }
        else
        {
            // Paneli kapat ve oyunu devam ettir
            recipeBookPanel.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("Tarif Defteri Kapatýldý - Oyun Devam Ediyor");
        }
    }
}
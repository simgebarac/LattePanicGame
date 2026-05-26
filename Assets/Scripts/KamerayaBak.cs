using UnityEngine;

public class KamerayaBak : MonoBehaviour
{
    private Transform anaKamera;

    void Start()
    {
        // Sahnedeki ana kamerayý bulup hafýzaya alýyoruz
        if (Camera.main != null)
        {
            anaKamera = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (anaKamera != null)
        {
            // Objenin dünyadaki açýsýný, kameranýn açýsýyla birebir eþitliyoruz.
            // Böylece kamera nereye bakarsa baksýn bu yuvarlak hep ekrana düz bakacak.
            transform.rotation = anaKamera.rotation;
        }
    }
}
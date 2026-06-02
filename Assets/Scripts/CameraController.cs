using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Atamalar")]
    public Transform playerTarget;

    [Header("Sabit Mod Ayarları")]
    public bool isTakipActive = false;

    [Header("Takip Modu Ayarları")]
    public Vector3 followOffset = new Vector3(0, 4f, -6f);
    public float followSpeed = 8f;
    public float lookHeightOffset = 1.2f;

    // Sabit mod için başlangıç pozisyon VE rotasyonu kilitle
    private Vector3 staticPosition;
    private Quaternion staticRotation;

    void Start()
    {
        // Oyun başladığında kameranın pozisyonu ve rotasyonu kilitlenir
        staticPosition = transform.position;
        staticRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isTakipActive = !isTakipActive;
            Debug.Log("Kamera: " + (isTakipActive ? "TAKİP MODU" : "SABİT MOD"));
        }
    }

    void LateUpdate()
    {
        if (playerTarget == null) return;

        if (isTakipActive)
        {
            // Karakterin forward'ından BAĞIMSIZ — sabit dünya ekseninde takip
            Vector3 targetPos = playerTarget.position + followOffset;

            transform.position = Vector3.Lerp(
                transform.position, targetPos, Time.deltaTime * followSpeed);

            Vector3 lookTarget = playerTarget.position + Vector3.up * lookHeightOffset;
            transform.LookAt(lookTarget);
        }
        else
        {
            transform.position = staticPosition;
            transform.rotation = staticRotation;
        }
    }
}
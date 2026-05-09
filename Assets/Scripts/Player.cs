using UnityEngine;
using System;

public class Player : MonoBehaviour
{

    [Header("Ayarlar")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private CharacterController characterController;
    private Animator animator;
    private BaseCounter selectedCounter;
    private float interactDistance = 2f;

    private void Awake()
    {
        // Bileþenleri alýyoruz
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // GameInput'taki E tuþu etkinliðine abone oluyoruz
        if (gameInput != null)
        {
            gameInput.OnInteractAction += GameInput_OnInteractAction;
        }
        else
        {
            Debug.LogError("Player: GameInput referansý eksik! Lütfen Inspector'dan sürükle.");
        }
    }

    // "E" tuþuna basýldýðýnda GameInput burayý tetikler
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement()
    {
        // Girdiyi GameInput'tan alýyoruz
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // Girdiyi karakterin hareket yönüne (Dünya ekseni) çeviriyoruz
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        bool isWalking = moveDir != Vector3.zero;

        // Animasyon kontrolü
        if (animator != null)
        {
            animator.SetBool("run", isWalking);
        }

        if (isWalking)
        {
            // Karakteri hareket ettir
            characterController.Move(moveDir * moveSpeed * Time.deltaTime);

            // Karakteri gittiði yöne doðru yumuþakça döndür
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleInteractions()
    {
        // Raycast ýþýnýný karakterin bel hizasýndan ileriye doðru gönderiyoruz
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            // Eðer vurduðumuz nesne bir tezgahsa (BaseCounter)
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if (baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    // Seçili tezgahý deðiþtiren yardýmcý fonksiyon (ileride görsel efekt için lazým olacak)
    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
    }
}
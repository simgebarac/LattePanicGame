using UnityEngine;
using System;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [Header("Ayarlar")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float interactDistance = 2.5f; // Masalara yetiþebilmek için 1.5f'ten 2.5f'e yükselttik
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    [Header("Fizik Ayarlarý")]
    [SerializeField] private float gravity = -9.81f; // Yerçekimi gücü

    private CharacterController characterController;
    private Animator animator;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private Vector3 velocity; // Karakterin dikey hýzý (düþme hýzý)

    private void Awake()
    {
        if (Instance != null) { Debug.LogError("Birden fazla Player var!"); }
        Instance = this;

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (gameInput != null)
        {
            gameInput.OnInteractAction += GameInput_OnInteractAction;
        }
    }

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
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        bool isWalking = moveDir != Vector3.zero;

        if (animator != null)
            animator.SetBool("run", isWalking);

        // Yerçekimi
        if (!characterController.isGrounded)
            moveDir.y -= 9.81f * Time.deltaTime;

        if (isWalking)
        {
            characterController.Move(moveDir * moveSpeed * Time.deltaTime);
            transform.forward = Vector3.Slerp(transform.forward, new Vector3(moveDir.x, 0, moveDir.z), Time.deltaTime * 10f);
        }
        else
        {
            // Yerde tutmak için küçük bir aþaðý kuvvet
            characterController.Move(Vector3.down * 2f * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        // Karakter yerdeyse hýzý sabitle (Yukarý fýrlamayý engeller)
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Yerçekimini hesapla ve uygula
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleInteractions()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        // Scene view'da kýrmýzý çizgi olarak Raycast'i göster
        Debug.DrawRay(rayOrigin, transform.forward * interactDistance, Color.red);

        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit debugHit, interactDistance))
        {
            Debug.Log($"[DEBUG] Çarptý: {debugHit.transform.name} | Layer: {LayerMask.LayerToName(debugHit.transform.gameObject.layer)}");
        }
        else
        {
            Debug.Log("[DEBUG] Hiçbir þeye çarpmadý!");
        }

        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit raycastHit, interactDistance, countersLayerMask))
        {
            BaseCounter baseCounter = raycastHit.transform.GetComponent<BaseCounter>();
            if (baseCounter == null)
                baseCounter = raycastHit.transform.GetComponentInParent<BaseCounter>();

            if (baseCounter != null)
            {
                if (baseCounter != selectedCounter)
                    SetSelectedCounter(baseCounter);
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
    public BaseCounter GetSelectedCounter()
    {
        return selectedCounter;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    // --- IKitchenObjectParent Fonksiyonlarý ---
    public Transform GetKitchenObjectFollowTransform() { return kitchenObjectHoldPoint; }
    public void SetKitchenObject(KitchenObject kitchenObject) { this.kitchenObject = kitchenObject; }
    public KitchenObject GetKitchenObject() { return kitchenObject; }
    public void ClearKitchenObject() { kitchenObject = null; }
    public bool HasKitchenObject() { return kitchenObject != null; }
}
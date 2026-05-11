using UnityEngine;
using System;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    // --- SINGLETON YAPISI (Makinenin sana ulaþabilmesi için) ---
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [Header("Ayarlar")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private CharacterController characterController;
    private Animator animator;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake()
    {
        // Singleton atamasý
        if (Instance != null) { Debug.LogError("Birden fazla Player var!"); }
        Instance = this;

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // --- DIÞARIDAN BAKILAN COUNTER'I ALMAK ÝÇÝN FONKSÝYON ---
    public BaseCounter GetSelectedCounter()
    {
        return selectedCounter;
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
        {
            animator.SetBool("run", isWalking);
        }

        if (isWalking)
        {
            characterController.Move(moveDir * moveSpeed * Time.deltaTime);
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * 10f);
        }
    }

    private void HandleInteractions()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit raycastHit, 2f, countersLayerMask))
        {
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
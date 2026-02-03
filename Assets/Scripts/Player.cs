using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask itemLayermask;
    [SerializeField] private Interactable selectedItem;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;

    private Vector3 lastInteractDir;
    private bool isWalking;


    public class OnSelectItemChangedEventArgs : EventArgs {
        public Interactable selectedItem;
    }

    public event EventHandler<OnSelectItemChangedEventArgs> OnSelectItemChanged;    


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player Instance");
        }

        Instance = this;
    }

    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (selectedItem != null) {
            selectedItem.Interact(this);
        }
    }

    private void Update() {
        HandleMovements();
        HandleInteractions();
        PlayAnimationsAndFootSteps();

    }

    private void PlayAnimationsAndFootSteps() {
        if (isWalking) audioSource.enabled = true;
        else audioSource.enabled = false;

        animator.SetBool("IsWalking", isWalking);
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float playerRadius = .7f;
        float playerHeight = 2f;

        float interactDist = 2f;

        if (Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, lastInteractDir, out RaycastHit hit, interactDist, itemLayermask)) {
            if (hit.transform.TryGetComponent(out Interactable item)) {
                if (item != selectedItem) {
                    SetSelectedItem(item);
                }
            }
            else {
                SetSelectedItem(null);
            }
        }
        else {
            SetSelectedItem(null);
        }
    }

    private void HandleMovements() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float playerRadius = .7f;
        float playerHeight = 2f;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveSpeed * Time.deltaTime);

        float rotateSpeed = 15;

        isWalking = moveDir.sqrMagnitude > 0.001f;

        if (moveDir.sqrMagnitude > 0.001f) {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        }

        if (!canMove) {

            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0);

            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveSpeed * Time.deltaTime);

            if (canMove) {
                moveDir = moveDirX;
            }
            else {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z);

                canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveSpeed * Time.deltaTime);

                if (canMove) {
                    moveDir = moveDirZ;
                }
                else {
                    //Debug.Log("Player is completely blocked");
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveSpeed * Time.deltaTime;// increments a movement value of 1 in x & y axes every frame for smooth movement 
        }

        isWalking = moveDir.sqrMagnitude > 0.001f;
    }

    private void SetSelectedItem(Interactable selectedItem) {
        this.selectedItem = selectedItem;

        OnSelectItemChanged?.Invoke(this, new OnSelectItemChangedEventArgs {
            selectedItem = selectedItem
        });
    }

}

using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public event EventHandler OnInteractAction;

    //public event EventHandler OnInteractAlternateAction;

    private void Awake() {
        playerInputActions = new PlayerInputActions();

        playerInputActions.Enable();

        playerInputActions.Player.Interact.performed += InteractPerformed;
        //playerInputActions.Player.InteractAlternate.performed += InteractAlternatePerformed;

    }

    private void InteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    //private void InteractAlternatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    //    OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    //}

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }
}

using UnityEngine;
using UnityEngine.UI;                      // ← Added for UnityEngine.UI.Image
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InteractableTriggera : MonoBehaviour
{
    [Header("Interaction Settings")]
    public GameObject interactionPanela;           // The whole UI panel
    public TMP_InputField inputField;              // Where player types
    
    [Header("Result Images (instead of text)")]
    public Image victoryImage;                     // Green/check/success sprite/image
    public Image failureImage;                     // Red/cross/fail sprite/image
    
    public float messageDuration = 2f;             // How long to show the result image

    [Header("Which objects can be interacted with")]
    public LayerMask interactableLayer;

    private GameObject currentInteractable;
    private bool isInteracting = false;

    void Awake()
    {
        if (inputField != null)
        {
            inputField.gameObject.SetActive(false);
        }
        
        // Make sure result images start hidden
        if (victoryImage != null) victoryImage.gameObject.SetActive(false);
        if (failureImage != null) failureImage.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.RemoveAllListeners();
            inputField.onEndEdit.AddListener(OnInputSubmitted);
        }
    }

    void OnDisable()
    {
        if (inputField != null)
        {
            inputField.onEndEdit.RemoveAllListeners();
        }
    }

    void Update()
    {
        if (currentInteractable == null) return;
        if (Keyboard.current == null) return;

        // Start interaction with F (only when not already interacting)
        if (!isInteracting && Keyboard.current.fKey.wasPressedThisFrame)
        {
            StartInteraction();
        }

        // Force focus every frame while interacting & field is visible
        if (isInteracting && inputField != null && inputField.gameObject.activeInHierarchy)
        {
            if (!inputField.isFocused)
            {
                inputField.ActivateInputField();
                inputField.Select();

                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(inputField.gameObject);
                }

                inputField.MoveTextEnd(false); // caret to end for better UX
            }
        }
    }

    private void StartInteraction()
    {
        isInteracting = true;

        if (interactionPanela != null)
            interactionPanela.SetActive(true);

        if (inputField != null)
        {
            inputField.gameObject.SetActive(true);
            inputField.text = "";
            inputField.interactable = true;

            // Activate & focus
            inputField.ActivateInputField();
            inputField.Select();

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject);
            }

            inputField.MoveTextEnd(false);
        }

        // Hide both images at start of new interaction
        if (victoryImage != null) victoryImage.gameObject.SetActive(false);
        if (failureImage != null) failureImage.gameObject.SetActive(false);
    }

    // Called when player presses Enter (via onEndEdit)
    private void OnInputSubmitted(string value)
    {
        if (!isInteracting) return;
        SubmitAnswer();
    }

    private void SubmitAnswer()
    {
        if (inputField == null) return;

        string answer = inputField.text.Trim().ToLower();

        // Hide input immediately
        inputField.DeactivateInputField();
        inputField.gameObject.SetActive(false);

        // Show the appropriate result image
        if (interactionPanela != null)
        {
            if (answer == "satya")
            {
                if (victoryImage != null)
                {
                    victoryImage.gameObject.SetActive(true);
                }
                // ← Add win logic (sound, particles, scene change, etc.) here
            }
            else
            {
                if (failureImage != null)
                {
                    failureImage.gameObject.SetActive(true);
                }
                // ← Add fail logic here
            }
        }

        // Hide UI after delay
        StopAllCoroutines();
        StartCoroutine(HideUIAfterDelay());
    }

    private IEnumerator HideUIAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        if (interactionPanela != null)
            interactionPanela.SetActive(false);

        if (victoryImage != null)
            victoryImage.gameObject.SetActive(false);

        if (failureImage != null)
            failureImage.gameObject.SetActive(false);

        isInteracting = false;
        // Optional: re-enable player movement/controller here if disabled earlier
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInteractable(other.gameObject))
        {
            currentInteractable = other.gameObject;
            // Optional: show "Press F to interact" UI/prompt here
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInteractable(other.gameObject) && other.gameObject == currentInteractable)
        {
            currentInteractable = null;

            if (isInteracting)
            {
                StopAllCoroutines();

                if (interactionPanela != null)
                    interactionPanela.SetActive(false);

                if (inputField != null)
                {
                    inputField.DeactivateInputField();
                    inputField.gameObject.SetActive(false);
                }

                if (victoryImage != null) victoryImage.gameObject.SetActive(false);
                if (failureImage != null) failureImage.gameObject.SetActive(false);

                isInteracting = false;
            }
        }
    }

    private bool IsInteractable(GameObject obj)
    {
        return ((interactableLayer.value & (1 << obj.layer)) != 0);
    }
}
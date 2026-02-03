using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class InteractableTrigger : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private GameObject interactionPanel;     // Drag your UI panel here
    [SerializeField] private Image interactionImage;          // Drag the UI Image component here
    [SerializeField] private float displayDuration = 2f;

    [Header("Which objects can be interacted with")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Hint Images per Layer")]
    [Tooltip("Index 0 = unused\n1–31 correspond to Unity layer numbers\nDrag sprites here in Inspector")]
    [SerializeField] private Sprite[] layerHintSprites = new Sprite[32];

    private GameObject currentInteractable;

    void Awake()
    {
        // Quick startup check
        if (interactionPanel == null) Debug.LogWarning("InteractableTrigger: interactionPanel is not assigned!", this);
        if (interactionImage == null) Debug.LogWarning("InteractableTrigger: interactionImage is not assigned!", this);
        if (interactableLayer == 0)   Debug.LogWarning("InteractableTrigger: interactableLayer mask is set to Nothing!", this);

    }

    void Update()
    {
        //Debug.Log("[Interact] Update checking for input");
        //// Safety check for new Input System
        //if (Keyboard.current == null)
        //{
        //    Debug.LogWarning("Keyboard.current is null - New Input System not initialized?");
        //    return;
        //}

        if (currentInteractable == null) return;

        // Optional: show hint that you're in range (can remove later)
        // Debug.Log($"In range of: {currentInteractable.name} (layer: {LayerMask.LayerToName(currentInteractable.layer)})");

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log($"[Interact] F key pressed → trying to interact with {currentInteractable.name}");
            Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered");
        if (!IsInteractable(other.gameObject)) return;

        currentInteractable = other.gameObject;
        Debug.Log($"[Trigger] Entered interactable: {other.name} (layer: {LayerMask.LayerToName(other.gameObject.layer)})");
        
        // Optional: you can add visual/audio feedback here later (glow, sound, icon...)
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentInteractable)
        {
            Debug.Log($"[Trigger] Exited interactable: {other.name}");
            currentInteractable = null;
        }
    }

    private bool IsInteractable(GameObject obj)
    {
        if (obj == null) return false;
        return (interactableLayer.value & (1 << obj.layer)) != 0;
    }

    private void Interact()
    {
        if (currentInteractable == null)
        {
            Debug.LogWarning("[Interact] Called but currentInteractable is null");
            return;
        }

        int layer = currentInteractable.layer;
        Sprite sprite = GetSpriteForLayer(layer);

        // ← Your actual interaction logic should go here
        // Examples:
        // - Open door
        // - Pick up item
        // - Start dialogue
        // - Play animation/sound
        Debug.Log($"[Interact] Interacting with {currentInteractable.name} (layer {layer})");

        // Show visual feedback if we have a sprite
        if (sprite != null)
        {
            Debug.Log($"[Interact] Showing hint sprite for layer {layer}");
            StartCoroutine(ShowInteractionImage(sprite));
        }
        else
        {
            Debug.Log($"[Interact] No sprite assigned for layer {layer}");
        }
    }

    private Sprite GetSpriteForLayer(int layer)
    {
        if (layer < 0 || layer >= layerHintSprites.Length)
        {
            Debug.LogWarning($"Layer {layer} is out of bounds for hint sprites array");
            return null;
        }

        if (layerHintSprites[layer] == null)
        {
            // Debug.Log($"No sprite assigned for layer {layer} ({LayerMask.LayerToName(layer)})");
            return null;
        }

        return layerHintSprites[layer];
    }

    private IEnumerator ShowInteractionImage(Sprite sprite)
    {
        if (interactionPanel == null || interactionImage == null)
        {
            Debug.LogWarning("Cannot show interaction image - panel or image reference is missing");
            yield break;
        }

        interactionImage.sprite = sprite;
        interactionImage.enabled = true;
        interactionPanel.SetActive(true);

        Debug.Log($"Showing interaction panel for {displayDuration} seconds");

        yield return new WaitForSeconds(displayDuration);

        interactionPanel.SetActive(false);
        // interactionImage.enabled = false; // ← uncomment if you want to hide the image too
        // interactionImage.sprite = null;   // ← optional clean-up
    }

    // Optional: helper to see current status in Inspector
    private void OnDrawGizmosSelected()
    {
        if (currentInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(currentInteractable.transform.position, 0.5f);
        }
    }
}
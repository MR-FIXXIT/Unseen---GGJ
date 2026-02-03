using UnityEngine;
using UnityEngine.InputSystem;

public class LayerToggle : MonoBehaviour {
    [SerializeField] private int normalLayerId = 7;
    [SerializeField] private int glowLayerId = 8;

    [SerializeField] private GameObject thingToEnableOnGlow; // set in Inspector

    private bool isGlowActive;

    void Update() {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) {
            isGlowActive = !isGlowActive;
            int targetLayer = isGlowActive ? glowLayerId : normalLayerId;

            // Change player and all children
            SetLayerRecursively(gameObject, targetLayer);

            // Enable when layer 8, disable when layer 7
            if (thingToEnableOnGlow != null)
                thingToEnableOnGlow.SetActive(targetLayer == 8);

            Debug.Log("Player and children layer changed to " + targetLayer);
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}

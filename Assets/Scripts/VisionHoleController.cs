using UnityEngine;

public class VisionHoleController : MonoBehaviour {
    [Header("References")]
    public Transform player;
    public Camera targetCamera;
    public Material overlayMaterial;

    [Header("Hole Settings (pixels)")]
    public float radiusPixels = 220f;
    public float softnessPixels = 60f;

    [Header("Optional offset")]
    public Vector2 screenOffsetPixels = Vector2.zero;

    static readonly int CenterID = Shader.PropertyToID("_Center");
    static readonly int RadiusID = Shader.PropertyToID("_Radius");
    static readonly int SoftnessID = Shader.PropertyToID("_Softness");

    void Reset() {
        targetCamera = Camera.main;
    }

    void LateUpdate() {
        if (!player || !targetCamera || !overlayMaterial) return;

        // Convert player world -> screen pixel coordinates
        Vector3 sp = targetCamera.WorldToScreenPoint(player.position);

        // If player is behind camera, you may want to hide the hole (optional)
        if (sp.z < 0f) {
            overlayMaterial.SetVector(CenterID, new Vector4(-9999f, -9999f, 0f, 0f));
            return;
        }

        sp.x += screenOffsetPixels.x;
        sp.y += screenOffsetPixels.y;

        overlayMaterial.SetVector(CenterID, new Vector4(sp.x, sp.y, 0f, 0f));
        overlayMaterial.SetFloat(RadiusID, radiusPixels);
        overlayMaterial.SetFloat(SoftnessID, softnessPixels);
    }
}

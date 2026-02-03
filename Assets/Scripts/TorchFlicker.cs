using UnityEngine;

[RequireComponent(typeof(Light))]
public class TorchFlicker : MonoBehaviour {
    [Header("Intensity")]
    public float baseIntensity = 2f;
    public float intensityVariation = 0.6f;

    [Header("Range")]
    public float baseRange = 10f;
    public float rangeVariation = 1.5f;

    [Header("Flicker Speed")]
    public float speed = 8f;

    [Header("Smoothing (higher = smoother)")]
    public float smooth = 12f;

    private Light _light;
    private float _targetIntensity;
    private float _targetRange;

    void Awake() {
        _light = GetComponent<Light>();
        _light.intensity = baseIntensity;
        _light.range = baseRange;
    }

    void Update() {
        // Perlin noise gives a natural-looking flicker
        float noise = Mathf.PerlinNoise(Time.time * speed, 0f);

        _targetIntensity = baseIntensity + (noise - 0.5f) * 2f * intensityVariation;
        _targetRange = baseRange + (noise - 0.5f) * 2f * rangeVariation;

        _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, Time.deltaTime * smooth);
        _light.range = Mathf.Lerp(_light.range, _targetRange, Time.deltaTime * smooth);
    }
}

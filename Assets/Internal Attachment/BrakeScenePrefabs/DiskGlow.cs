using UnityEngine;

public class DiskGlow : MonoBehaviour
{
    [Header("References")]
    public DiskRotationV1 disk;
    public Renderer diskRenderer;

    [Header("Glow")]
    public float maxGlowDuration = 3f;
    public float glowIntensity = 3f;
    public float coolDownSpeed = 0.3f;   // lower = longer decay (try 0.1 - 0.5)

    public Color colorYellow = new Color(1f, 0.92f, 0f);
    public Color colorOrange = new Color(1f, 0.45f, 0f);
    public Color colorRed = new Color(1f, 0.05f, 0f);

    float _glowT = 0f;
    float _glowRate = 0f;
    bool _wasOuterActive = false;

    MaterialPropertyBlock _mpb;
    static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        bool outerActive = disk.outerEngaged;
        bool discSpinning = disk.currentSpeedKmh > 0f;  // use actual rotation speed

        if (outerActive && discSpinning)
        {
            // Snapshot rate the moment outer pad first engages
            if (!_wasOuterActive)
            {
                float speedRatio = disk.currentSpeedKmh / disk.maxSpeed;
                _glowRate = speedRatio / maxGlowDuration;
            }

            // Heat up only while disk is physically spinning
            _glowT += _glowRate * Time.deltaTime;
        }
        else if (_glowT > 0f)
        {
            // Disk stopped or brake released — decay
            _glowT -= Time.deltaTime * coolDownSpeed;
            _glowRate = 0f;
        }

        _wasOuterActive = outerActive && discSpinning;

        _glowT = Mathf.Clamp01(_glowT);

        Color glowColor;
        if (_glowT < 0.5f)
            glowColor = Color.Lerp(colorYellow, colorOrange, _glowT * 2f);
        else
            glowColor = Color.Lerp(colorOrange, colorRed, (_glowT - 0.5f) * 2f);

        Color emission = (_glowT > 0f) ? glowColor * (glowIntensity * _glowT) : Color.black;

        diskRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(EmissionID, emission);
        diskRenderer.SetPropertyBlock(_mpb);
    }
}
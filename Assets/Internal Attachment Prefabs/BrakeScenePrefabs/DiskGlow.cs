using UnityEngine;

public class DiskGlow : MonoBehaviour
{
    [Header("References")]
    public DiskRotationV1 disk;
    public Renderer diskRenderer;

    [Header("Glow")]
    public float maxGlowDuration = 3f;    // seconds to reach full red at max speed
    public float glowIntensity = 3f;
    public float coolDownSpeed = 1.5f;

    public Color colorYellow = new Color(1f, 0.92f, 0f);
    public Color colorOrange = new Color(1f, 0.45f, 0f);
    public Color colorRed = new Color(1f, 0.05f, 0f);

    float _glowT = 0f;
    float _glowRate = 0f;    // how fast _glowT climbs — set when outer engages
    bool _wasOuterActive = false; // tracks the moment outer pad first engages

    MaterialPropertyBlock _mpb;
    static readonly int EmissionID = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        bool outerActive = disk.outerEngaged;
        bool discSpinning = disk.speedKmh > 0f;

        // Detect the exact moment outer pad engages — snapshot actual speed then
        if (outerActive && !_wasOuterActive)
        {
            // speedRatio: 1.0 at max speed, lower at lower speeds
            // this makes _glowRate faster at high speed, slower at low speed
            float speedRatio = disk.speedKmh / disk.maxSpeed;
            _glowRate = speedRatio / maxGlowDuration;
        }

        _wasOuterActive = outerActive;

        if (outerActive && discSpinning)
        {
            // Climb at the rate set by actual speed — fast stops from high speed heat more
            _glowT += _glowRate * Time.deltaTime;
        }
        else
        {
            // Disc stopped or brake released — cool down from wherever we are
            _glowT -= Time.deltaTime * coolDownSpeed;
            _glowRate = 0f;
        }

        _glowT = Mathf.Clamp01(_glowT);

        // Seamless yellow → orange → red
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
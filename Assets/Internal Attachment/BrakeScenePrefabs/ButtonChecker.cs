using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonChecker : MonoBehaviour
{
    public bool isBrake = false;
    public DiskRotationV1 disk;
    public BrakePads brakePads;

    [Header("HUD")]
    public TMP_Text CurrentSpeed;
    public Slider speedBar;
    public Image speedBarFill; // drag the Fill image of the slider here

    [Header("Speed Step")]
    public float speedStep = 10f;

    void Start()
    {
        if (speedBar != null)
            speedBar.maxValue = disk.maxSpeed;
    }

    void Update()
    {
        if (CurrentSpeed != null)
            CurrentSpeed.text = Mathf.RoundToInt(disk.currentSpeedKmh) + " km/h";

        if (speedBar != null)
        {
            speedBar.value = disk.currentSpeedKmh;
            UpdateSliderColor();
        }
    }

    void UpdateSliderColor()
    {
        if (speedBarFill == null) return;

        float t = disk.currentSpeedKmh / disk.maxSpeed; // 0 to 1

        Color green = new Color(0.1f, 0.75f, 0.2f);
        Color orange = new Color(1f, 0.55f, 0f);
        Color red = new Color(0.9f, 0.1f, 0.1f);

        Color fillColor;
        if (t < 0.5f)
            fillColor = Color.Lerp(green, orange, t * 2f);
        else
            fillColor = Color.Lerp(orange, red, (t - 0.5f) * 2f);

        speedBarFill.color = fillColor;
    }

    // --- existing buttons ---
    public void onstartpress()
    {
        isBrake = true;
        brakePads.forceBrake = false;
        disk.StartRotation();
    }

    public void onstoppress()
    {
        isBrake = false;
        brakePads.forceBrake = true;
        disk.StopRotation();
    }

    // --- speed buttons ---
    public void OnSpeedUpPress()
    {
        disk.speedKmh = Mathf.Clamp(disk.speedKmh + speedStep, 0f, disk.maxSpeed);
    }

    public void OnSpeedDownPress()
    {
        disk.speedKmh = Mathf.Clamp(disk.speedKmh - speedStep, 0f, disk.maxSpeed);
    }
}
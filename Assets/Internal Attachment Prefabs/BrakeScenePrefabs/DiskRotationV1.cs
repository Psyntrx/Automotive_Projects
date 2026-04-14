using UnityEngine;

public class DiskRotationV1 : MonoBehaviour
{
    public float maxSpeed = 180f;
    public float speedKmh = 60f;
    public float speedChangeRate = 20f;

    [HideInInspector] public bool innerEngaged = false;
    [HideInInspector] public bool outerEngaged = false;

    float currentSpeedKmh;
    float innerBrakeStrength = 0.3f;
    float accelerationRate = 20f;
    float decelerationRate = 40f;

    bool _speedUpHeld = false;
    bool _speedDownHeld = false;

    // Called by BrakeUI — keyboard and on-screen buttons both feed into these
    public void SetSpeedUpHeld(bool held) => _speedUpHeld = held;
    public void SetSpeedDownHeld(bool held) => _speedDownHeld = held;
    public void SetBrakeHeld(bool held) => innerEngaged = held;

    void Start()
    {
        currentSpeedKmh = speedKmh;
    }

    void Update()
    {
        speedKmh = Mathf.Clamp(speedKmh, 0f, maxSpeed);

        if (_speedUpHeld)
            speedKmh = Mathf.Clamp(speedKmh + speedChangeRate * Time.deltaTime, 0f, maxSpeed);

        if (_speedDownHeld)
            speedKmh = Mathf.Clamp(speedKmh - speedChangeRate * Time.deltaTime, 0f, maxSpeed);

        if (outerEngaged)
        {
            currentSpeedKmh = Mathf.MoveTowards(currentSpeedKmh, 0f, decelerationRate * 2f * Time.deltaTime);
        }
        else if (innerEngaged)
        {
            float target = speedKmh * (1f - innerBrakeStrength);
            currentSpeedKmh = Mathf.MoveTowards(currentSpeedKmh, target, decelerationRate * Time.deltaTime);
        }
        else
        {
            currentSpeedKmh = Mathf.MoveTowards(currentSpeedKmh, speedKmh, accelerationRate * Time.deltaTime);
        }

        float degreesPerSecond = (currentSpeedKmh * 1000f / 3600f) / 0.33f * Mathf.Rad2Deg;
        transform.Rotate(0f, 0f, -degreesPerSecond * Time.deltaTime);
    }
}
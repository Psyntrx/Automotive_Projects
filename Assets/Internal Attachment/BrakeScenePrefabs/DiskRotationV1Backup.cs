using UnityEngine;

public class DiskRotationV1Backup : MonoBehaviour
{
    public float maxSpeed = 180f;   // change this — everything uses it
    public float speedKmh = 60f;    // displayed and edited in km/h
    public float speedChangeRate = 20f;    // km/h per second when holding arrow

    [HideInInspector] public bool innerEngaged = false;
    [HideInInspector] public bool outerEngaged = false;

    float currentSpeedKmh;
    float innerBrakeStrength = 0.3f;
    float accelerationRate = 20f;
    float decelerationRate = 30f;

    void Start()
    {
        currentSpeedKmh = speedKmh;
    }

    void Update()
    {
        // Keep speedKmh always within 0 and maxSpeed
        speedKmh = Mathf.Clamp(speedKmh, 0f, maxSpeed);

        // Right/left arrow adjusts speedKmh directly
        if (Input.GetKey(KeyCode.RightArrow))
            speedKmh = Mathf.Clamp(speedKmh + speedChangeRate * Time.deltaTime, 0f, maxSpeed);

        if (Input.GetKey(KeyCode.LeftArrow))
            speedKmh = Mathf.Clamp(speedKmh - speedChangeRate * Time.deltaTime, 0f, maxSpeed);

        // Brake logic
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
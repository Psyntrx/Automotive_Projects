using UnityEngine;

public class BrakePads : MonoBehaviour
{
    [Header("Pad References")]
    public Transform innerPad;
    public Transform outerPad;

    [Header("Disk Reference")]
    public DiskRotationV1 disk;

    [Header("Movement")]
    public float innerPadTravel = 0.003f;   // tune this in Play mode
    public float outerPadTravel = 0.003f;   // tune this in Play mode
    public float padMoveSpeed = 0.05f;
    public bool forceBrake = false; // set externally by ButtonChecker

    float outerDelay = 0.5f;
    float brakeHeldTimer = 0f;

    Vector3 innerStartPos;
    Vector3 outerStartPos;

    void Start()
    {
        innerStartPos = innerPad.localPosition;
        outerStartPos = outerPad.localPosition;
    }

    void Update()
    {
        bool brakeHeld = Input.GetKey(KeyCode.DownArrow) || forceBrake;

        if (brakeHeld)
        {
            brakeHeldTimer += Time.deltaTime;

            // Inner pad moves in positive Z
            MovePad(innerPad, innerStartPos, innerPadTravel);
            disk.innerEngaged = true;

            // Outer pad moves in negative Z (opposite direction)
            if (brakeHeldTimer >= outerDelay)
            {
                MovePad(outerPad, outerStartPos, outerPadTravel);
                disk.outerEngaged = true;
            }
        }
        else
        {
            brakeHeldTimer = 0f;
            disk.innerEngaged = false;
            disk.outerEngaged = false;

            MovePad(innerPad, innerStartPos, 0f);
            MovePad(outerPad, outerStartPos, 0f);
        }
    }

    void MovePad(Transform pad, Vector3 startPos, float targetOffset)
    {
        Vector3 targetPos = startPos + new Vector3(0f, 0f, targetOffset);
        pad.localPosition = Vector3.MoveTowards(pad.localPosition, targetPos, padMoveSpeed * Time.deltaTime);
    }
}
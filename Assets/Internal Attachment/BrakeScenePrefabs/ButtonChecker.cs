using UnityEngine;

public class ButtonChecker : MonoBehaviour
{
    public bool isBrake = false;
    public DiskRotationV1 disk;
    public BrakePads brakePads; // drag your BrakePads object here in Inspector

    public void onstartpress()
    {
        isBrake = true;
        brakePads.forceBrake = false; // release brake on start
        disk.StartRotation();
        Debug.Log("START BUTTON PRESSED");
    }

    public void onstoppress()
    {
        isBrake = false;
        brakePads.forceBrake = true; // trigger brake pads
        disk.StopRotation();
        Debug.Log("STOP BUTTON PRESSED");
    }
}
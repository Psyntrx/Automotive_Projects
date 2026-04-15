using UnityEngine;

public class ButtonChecker : MonoBehaviour
{
    public bool isBrake = false;
    public DiskRotationV1 disk;   // drag your disk object here in Inspector

    public void onstartpress()
    {
        isBrake = true;
        disk.StartRotation();
        Debug.Log("START BUTTON PRESSED");
    }

    public void onstoppress()
    {
        isBrake = false;
        disk.StopRotation();
        Debug.Log("STOP BUTTON PRESSED");
    }
}
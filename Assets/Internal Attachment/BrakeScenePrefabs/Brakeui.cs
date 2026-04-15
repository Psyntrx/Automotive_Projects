using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach to: the Canvas GameObject
/// Requires: a Canvas set to World Space mode
/// Displays speed, max speed and brake state.
/// On-screen buttons work interchangeably with keyboard keys.
/// </summary>
public class BrakeUI : MonoBehaviour
{
    [Header("Reference")]
    public DiskRotationV1 disk;         // drag Right Disk here

    [Header("Text Displays")]
    public TextMeshProUGUI speedText;    // shows current speed in km/h
    public TextMeshProUGUI maxSpeedText; // shows max speed

    [Header("Buttons")]
    public Button brakeButton;          // on-screen brake button
    public Button speedUpButton;        // on-screen speed up button
    public Button speedDownButton;      // on-screen speed down button

    // Tracks whether on-screen buttons are held down
    bool _brakeHeld = false;
    bool _speedUpHeld = false;
    bool _speedDownHeld = false;

    void Start()
    {
        // Hook up button press and release events
        AddHoldEvents(brakeButton, () => _brakeHeld = true, () => _brakeHeld = false);
        AddHoldEvents(speedUpButton, () => _speedUpHeld = true, () => _speedUpHeld = false);
        AddHoldEvents(speedDownButton, () => _speedDownHeld = true, () => _speedDownHeld = false);
    }

    void Update()
    {
        // Keyboard OR on-screen button — whichever is active
        bool braking = Input.GetKey(KeyCode.DownArrow) || _brakeHeld;
        bool speedingUp = Input.GetKey(KeyCode.RightArrow) || _speedUpHeld;
        bool slowingDown = Input.GetKey(KeyCode.LeftArrow) || _speedDownHeld;

        // Push input into disk script
        disk.SetBrakeHeld(braking);
        disk.SetSpeedUpHeld(speedingUp);
        disk.SetSpeedDownHeld(slowingDown);

        // Update speed display
        speedText.text = Mathf.RoundToInt(disk.speedKmh) + " km/h";
        maxSpeedText.text = "Max: " + Mathf.RoundToInt(disk.maxSpeed) + " km/h";
    }

    // Helper: wires up pointer down and pointer up events on a button
    void AddHoldEvents(Button btn, UnityEngine.Events.UnityAction onDown, UnityEngine.Events.UnityAction onUp)
    {
        if (btn == null) return;

        var trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        var down = new UnityEngine.EventSystems.EventTrigger.Entry();
        down.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        down.callback.AddListener((e) => onDown());
        trigger.triggers.Add(down);

        var up = new UnityEngine.EventSystems.EventTrigger.Entry();
        up.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        up.callback.AddListener((e) => onUp());
        trigger.triggers.Add(up);
    }
}
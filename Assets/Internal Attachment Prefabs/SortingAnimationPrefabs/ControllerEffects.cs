using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerEffects : MonoBehaviour
{
    public Animator sortingAnimator;

    void Start()
    {
        if (sortingAnimator == null)
            sortingAnimator = GetComponent<Animator>();

        Debug.Log("ControllerEffects is running!");
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Debug.Log("1 pressed");
            sortingAnimator.SetBool("CubeMovement", false);
            sortingAnimator.SetBool("CylinderSorting", false);
            sortingAnimator.SetBool("Idle", false);
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Debug.Log("2 pressed");
            sortingAnimator.SetBool("CubeMovement", true);
            sortingAnimator.SetBool("CylinderSorting", false);
            sortingAnimator.SetBool("Idle", false);
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Debug.Log("3 pressed");
            sortingAnimator.SetBool("CubeMovement", false);
            sortingAnimator.SetBool("CylinderSorting", true);
            sortingAnimator.SetBool("Idle", false);
        }
        else if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R pressed - Reset/Idle");
            sortingAnimator.SetBool("CubeMovement", false);
            sortingAnimator.SetBool("CylinderSorting", false);
            sortingAnimator.SetBool("Idle", true);
        }
    }
}
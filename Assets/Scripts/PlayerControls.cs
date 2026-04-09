using Plane;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    public InputActionAsset inputActions;
    public PlaneController planeController;

    // Private State
    private InputActionMap inputMap;
    private InputAction throttleAction;
    private InputAction pitchAction;
    private InputAction rollAction;
    private InputAction yawAction;
    private InputAction fireMissileAction;
    private InputAction fireCannonAction;
    private InputAction selectTargetAction;
    private InputAction clearTargetAction;

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    private void OnEnable()
    {
        inputMap = inputActions.FindActionMap("Player");

        throttleAction = inputMap.FindAction("Throttle");
        pitchAction = inputMap.FindAction("Pitch");
        rollAction = inputMap.FindAction("Roll");
        yawAction = inputMap.FindAction("Yaw");
        fireMissileAction = inputMap.FindAction("FireMissile");
        fireCannonAction = inputMap.FindAction("FireCannon");
        selectTargetAction = inputMap.FindAction("SelectTarget");
        clearTargetAction = inputMap.FindAction("ClearTarget");

        fireMissileAction.performed += _ => FireMissile();
        fireCannonAction.performed += _ => FireCannon();
        selectTargetAction.performed += _ => SelectTarget();
        clearTargetAction.performed += _ => ClearTarget();

        inputMap.Enable();
    }

    private void OnDisable()
    {
        fireMissileAction.performed -= _ => FireMissile();
        fireCannonAction.performed -= _ => FireCannon();
        selectTargetAction.performed -= _ => SelectTarget();
        clearTargetAction.performed -= _ => ClearTarget();

        inputMap.Disable();
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------

    private void FixedUpdate()
    {
        HandleFlight();
    }

    // -------------------------------------------------------------------------
    // Flight
    // -------------------------------------------------------------------------

    private void HandleFlight()
    {
        if (planeController == null) return;

        float pitch = pitchAction.ReadValue<float>();
        float roll = rollAction.ReadValue<float>();
        float yaw = yawAction.ReadValue<float>();
        float throttle = throttleAction.ReadValue<float>();

          planeController.ApplyPitch(pitch);
          planeController.ApplyRoll(roll);
          planeController.ApplyYaw(yaw);
          planeController.ApplyThrottle(throttle);
    }

    // -------------------------------------------------------------------------
    // Weapons
    // -------------------------------------------------------------------------

    private void FireMissile()
    {
        // TODO: Implement missile firing
    }

    private void FireCannon()
    {
        // TODO: Implement cannon firing
    }

    // -------------------------------------------------------------------------
    // Targeting
    // -------------------------------------------------------------------------

    private void SelectTarget()
    {
        // TODO: Implement target selection
    }

    private void ClearTarget()
    {
        // TODO: Implement target clearing
    }
}

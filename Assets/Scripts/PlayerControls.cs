using Plane;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class PlayerControls : MonoBehaviour
{
    [Header("References")]
    public InputActionAsset inputActions;
    public PlaneController planeController;
    public Launcher missiles;
    public GunCannon gunCannon;
    // Private State
    private InputActionMap inputMap;
    private InputAction throttleAction;
    private InputAction rollPitchAxis;
    private InputAction yawAction;
    private InputAction fireMissileAction;
    private InputAction fireCannonAction;
    private InputAction selectTargetAction;
    private InputAction clearTargetAction;


    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------



    private void Start()
    {
        
    }
    private void OnEnable()
    {
        inputMap = inputActions.FindActionMap("Player");
        yawAction = inputMap.FindAction("Yaw");
        throttleAction = inputMap.FindAction("Throttle");
        
        rollPitchAxis = inputMap.FindAction("PitchRoll");
        fireMissileAction = inputMap.FindAction("FireMissile");
        fireCannonAction = inputMap.FindAction("FireCannon");
        selectTargetAction = inputMap.FindAction("SelectTarget");
        clearTargetAction = inputMap.FindAction("ClearTarget");

        fireMissileAction.performed += _ => FireMissile();
        
        selectTargetAction.performed += _ => SelectTarget();
        clearTargetAction.performed += _ => ClearTarget();

        inputMap.Enable();
    }

    private void OnDisable()
    {
        fireMissileAction.performed -= _ => FireMissile();
     
        selectTargetAction.performed -= _ => SelectTarget();
        clearTargetAction.performed -= _ => ClearTarget();

        inputMap.Disable();
    }

    // -------------------------------------------------------------------------
    // Update
    // -------------------------------------------------------------------------
    private void Update()
    {
        FireCannon();
    }
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
        Vector2 rollPitch = rollPitchAxis.ReadValue<Vector2>();
        float pitch = rollPitch.y;
        float roll =  -rollPitch.x;
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
        Debug.Log("Missile");
        missiles.FireMissile();
    }

    private void FireCannon()
    {
        
        if (fireCannonAction.IsPressed())
        {
            gunCannon.FireGuns();
        }
    }

    // -------------------------------------------------------------------------
    // Targeting
    // -------------------------------------------------------------------------

    private void SelectTarget()
    {
        // TODO: Implement target selection
        if(planeController == null) return;
    
        EntityTargeting.instance.SelectTarget(transform);
        missiles.target = EntityTargeting.instance.target;

    }

    private void ClearTarget()
    {
        // TODO: Implement target clearing
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 10);
    }
}

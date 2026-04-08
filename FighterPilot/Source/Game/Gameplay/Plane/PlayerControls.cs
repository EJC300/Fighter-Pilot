using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

/// <summary>
/// PlayerControls Script.
/// </summary>
public class PlayerControls : Script
{
 public PlaneController PlaneController;

    // Input axis names
    public string PitchAxis = "PitchAxis";
    public string RollAxis = "RollAxis";
    public string YawAxis = "YawAxis";
    public string ThrottleAxis = "ThrottleAxis";

    // Button names
    public string FireMissileButton = "FireMissile";
    public string FireCannonButton = "FireCannon";
    public string SelectTargetButton = "SelectTarget";
    public string ClearTargetButton = "ClearTarget";

    // Sensitivity
    public float PitchSensitivity = 1.0f;
    public float RollSensitivity = 1.0f;
    public float YawSensitivity = 1.0f;

    public override void OnEnable()
    {
        if (PlaneController == null)
            Debug.LogError("PlayerPlaneController: No PlaneController assigned!");
    }

    public override void OnUpdate()
    {
        HandleWeapons();
        HandleTargeting();
    }

    public override void OnFixedUpdate()
    {
        HandleFlight();
    }

    void HandleFlight()
    {
        if (PlaneController == null) return;

        float pitch    = Input.GetAxis(PitchAxis)    * PitchSensitivity;
        float roll     = Input.GetAxis(RollAxis)     * RollSensitivity;
        float yaw      = Input.GetAxis(YawAxis)      * YawSensitivity;
        float throttle = Input.GetAxis(ThrottleAxis);

        PlaneController.ApplyPitch(pitch);
        PlaneController.ApplyRoll(roll);
        PlaneController.ApplyYaw(yaw);
        PlaneController.ApplyThrottle(throttle);
    }

    void HandleWeapons()
    {
        if (Input.GetAction(FireMissileButton))
            FireMissile();

        if (Input.GetAction(FireCannonButton))
            FireCannon();
    }

    void HandleTargeting()
    {
        if (Input.GetAction(SelectTargetButton))
            SelectTarget();

        if (Input.GetAction(ClearTargetButton))
            ClearTarget();
    }

    // Stubs
    void FireMissile()
    {
        // TODO: Implement missile firing
    }

    void FireCannon()
    {
        // TODO: Implement cannon firing
    }

    void SelectTarget()
    {
        // TODO: Implement target selection
    }

    void ClearTarget()
    {
        // TODO: Implement target clearing
    }
}


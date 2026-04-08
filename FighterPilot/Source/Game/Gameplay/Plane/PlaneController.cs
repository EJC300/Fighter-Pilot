using System;
using FlaxEngine;

namespace Game;

public class PlaneController : Script
{
    [Header("References")]
    public RigidBody RB;

    [Header("Drag")]
    public float CdForward  = 0.02f;
    public float CdBackward = 0.5f;
    public float CdLeft     = 0.35f;
    public float CdRight    = 0.35f;
    public float CdUp       = 0.35f;
    public float CdDown     = 0.35f;

    [Header("Lift")]
    public float Cl               = 100f;
    public float cID              = 0f;
    public float AngleOfIncidence = 0.5f;

    [Header("Thrust")]
    public float TotalThrust      = 0f;
    public float ThrottleSpeed    = 0f;
    public float accelerationRate = 0f;
    public float decelerationRate = 0f;

    [Header("Stall")]
    public float stallSpeed        = 50f;
    public float stallWarningSpeed = 70f;

    [Header("Control Strength")]
    public float PitchStrength = 1.0f;
    public float RollStrength  = 1.0f;
    public float YawStrength   = 1.0f;
    public float turnSpeed     = 0f;

    [Header("PID Controllers")]
    public PIDController PitchPID = new PIDController();
    public PIDController RollPID  = new PIDController();
    public PIDController YawPID   = new PIDController();

    // Private State
    private float   currentThrottle;
    private Vector3 currentVelocity;
    private Vector3 previousVelocity;

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    public override void OnEnable()
    {
        RB = Actor as RigidBody;
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnFixedUpdate()
    {
        UpdateState();
    }

    // -------------------------------------------------------------------------
    // State
    // -------------------------------------------------------------------------

    void UpdateState()
    {
        ApplyLift(Actor.Transform.Right);
        ApplyDrag();
        ApplyCoordinatedTurn();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    Vector3 GetVelocity() => RB.LinearVelocity;

    float StallFactor()
    {
        Vector3 localVelocity = Actor.Transform.WorldToLocalVector(RB.LinearVelocity);
        float forwardSpeed    = Mathf.Max(0.0f, localVelocity.Z);
        return Mathf.Clamp((forwardSpeed - stallSpeed), 0f, 1f) / (stallWarningSpeed - stallSpeed);
    }

    float CalculateLinearAngleOfAttack()
    {
        Vector3 localVelocity = Actor.Transform.WorldToLocalVector(RB.LinearVelocity);
        return Mathf.Atan2(-localVelocity.Y, localVelocity.Z);
    }

    Vector3 CalculateAcceleration()
    {
        currentVelocity  = Actor.Transform.WorldToLocalVector(GetVelocity());
        Vector3 diff     = previousVelocity - currentVelocity;
        previousVelocity = currentVelocity;
        return diff;
    }

    // -------------------------------------------------------------------------
    // Drag
    // -------------------------------------------------------------------------

    Vector3 CalculateDrag()
    {
        Vector3 localVelocity = Actor.Transform.WorldToLocalVector(GetVelocity());

        float xCd = localVelocity.X > 0 ? CdRight    : CdLeft;
        float yCd = localVelocity.Y > 0 ? CdUp       : CdDown;
        float zCd = localVelocity.Z > 0 ? CdForward  : CdBackward;

        float xDrag = 0.5f * xCd * (localVelocity.X * localVelocity.X);
        float yDrag = 0.5f * yCd * (localVelocity.Y * localVelocity.Y);
        float zDrag = 0.5f * zCd * (localVelocity.Z * localVelocity.Z);

        Vector3 localDrag = new Vector3(xDrag, yDrag, zDrag) * localVelocity.Normalized;
        return Actor.Transform.LocalToWorldVector(localDrag);
    }

    void ApplyDrag()
    {
        RB.AddForce(-CalculateDrag());
    }

    // -------------------------------------------------------------------------
    // Lift
    // -------------------------------------------------------------------------

    Vector3 CalculateLift(Vector3 axis)
    {
        Vector3 velocity = Actor.Transform.WorldToLocalVector(GetVelocity());
        if (velocity.Z < 0) return Vector3.Zero;

        Vector3 liftVelocity  = Vector3.ProjectOnPlane(velocity, axis);
        Vector3 liftDirection = Vector3.Cross(liftVelocity.Normalized, axis).Normalized;
        float   aoa           = CalculateLinearAngleOfAttack() + AngleOfIncidence;
        float   liftCoef      = 0.5f * aoa * liftVelocity.LengthSquared * Cl;

        Vector3 lift         = liftCoef * StallFactor() * liftDirection;
        Vector3 inducedDrag  = liftVelocity.LengthSquared * cID * -liftVelocity.Normalized;

        return Actor.Transform.LocalToWorldVector(lift + inducedDrag);
    }

    void ApplyLift(Vector3 axis)
    {
        RB.AddForce(CalculateLift(axis));
    }

    // -------------------------------------------------------------------------
    // Coordinated Turn
    // -------------------------------------------------------------------------

    void ApplyCoordinatedTurn()
    {
        Vector3 liftForce      = CalculateLift(Actor.Transform.Right);
        Vector3 horizontalLift = Vector3.ProjectOnPlane(liftForce, Vector3.Up);
        float   turnForce      = horizontalLift.Length * Vector3.Dot(horizontalLift.Normalized, Actor.Transform.Right);

        RB.AddTorque(Actor.Transform.Up * turnForce * turnSpeed * StallFactor());
    }

    // -------------------------------------------------------------------------
    // Thrust
    // -------------------------------------------------------------------------

    public void ApplyThrottle(float input)
    {
        currentThrottle = Mathf.Clamp(currentThrottle + input * ThrottleSpeed * Time.DeltaTime, 0f, 1f);

        Vector3 localVelocity    = Actor.Transform.WorldToLocalVector(RB.LinearVelocity);
        float   currentForward   = localVelocity.Z;
        float   targetSpeed      = currentThrottle * TotalThrust;
        float   rate             = currentForward < targetSpeed ? accelerationRate : decelerationRate;
        float   newSpeed         = Mathf.Lerp(currentForward, targetSpeed, rate * Time.DeltaTime);

        Vector3 newLocalVelocity = localVelocity;
        newLocalVelocity.Z       = newSpeed;
        RB.AddForce(Actor.Transform.WorldToLocalVector(newLocalVelocity));
    }

    // -------------------------------------------------------------------------
    // Flight Controls
    // -------------------------------------------------------------------------

    public void ApplyPitch(float input)
    {
        Vector3 localAngular  = Actor.Transform.WorldToLocalVector(RB.AngularVelocity);
        float   correction    = PitchPID.CalculateResult(Time.DeltaTime, input, localAngular.X);
        RB.AddTorque(Actor.Transform.Right * correction * PitchStrength * StallFactor());
    }

    public void ApplyRoll(float input)
    {
        Vector3 localAngular  = Actor.Transform.WorldToLocalVector(RB.AngularVelocity);
        float   correction    = RollPID.CalculateResult(Time.DeltaTime, input, localAngular.Z);
        RB.AddTorque(Actor.Transform.Forward * correction * RollStrength * StallFactor());
    }

    public void ApplyYaw(float input)
    {
        Vector3 localAngular  = Actor.Transform.WorldToLocalVector(RB.AngularVelocity);
        float   correction    = YawPID.CalculateResult(Time.DeltaTime, input, localAngular.Y);
        RB.AddTorque(Actor.Transform.Up * correction * YawStrength * StallFactor());
    }
}

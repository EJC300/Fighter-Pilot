using System;
using System.Collections.Generic;
using FlaxEditor.GUI.Timeline.Tracks;
using FlaxEngine;

namespace Game;

/// <summary>
/// PlaneController Actor.
/// </summary>
public class PlaneController : Script
{

    public RigidBody RB;
    public float CdForward = 0.02f;
    public float CdRight = 0.35f;
    public float CdUp = 0.35f;

    public float Cl = 100;

    public float cID;

    public float turnSpeed;

    public float AngleOfIncidence = 0.5f;
    public float TotalThrustInPounds;


    /// <inheritdoc/>


    /// <inheritdoc/>
    public override void OnEnable()
    {

        RB = Actor as RigidBody;

    }

    /// <inheritdoc/>
    public override void OnDisable()
    {
        base.OnDisable();
        // Here you can add code that needs to be called when Actor is disabled (eg. unregister from events). This is called during edit time as well.
    }
    public override void OnUpdate()
    {
        UpdatePositionAndRotation();
    }
    public override void OnFixedUpdate()
    {
        UpdateState();
    }

    Vector3 GetVelocity()
    {
        return RB.LinearVelocity;
    }
    Vector3 CalculateLift(Vector3 axis)
    {
    
        Vector3 velocity = Actor.Transform.WorldToLocalVector( GetVelocity());
        if(velocity.Z < 0) return Vector3.Zero;
      
        Vector3 liftVelocity = Vector3.ProjectOnPlane(velocity,axis);
        Vector3 liftDirection = Vector3.Cross(liftVelocity.Normalized,axis).Normalized;
        float aoa = CalculateLinearAngleOfAttack() + AngleOfIncidence;
        
        float liftCoef = 0.5f * liftVelocity.LengthSquared* Cl;
        Vector3 lift =aoa * liftCoef * liftDirection;
        Vector3 inducedDrag = liftVelocity.LengthSquared * cID *  -liftVelocity.Normalized;
      
        
        return Actor.Transform.LocalToWorldVector( lift + inducedDrag);
        
        
    }

    Vector3 CalculateDrag()
    {
        Vector3 worldVelocity = GetVelocity();


        Vector3 localVelocity = (worldVelocity);



        float xDrag = 0.5f * CdRight * MathF.Abs(localVelocity.X * localVelocity.X);
        float yDrag = 0.5f * CdUp * MathF.Abs(localVelocity.Y * localVelocity.Y);
        float zDrag = 0.5f * CdForward * MathF.Abs(localVelocity.Z * localVelocity.Z);


        Vector3 localDrag = new Vector3(xDrag, yDrag, zDrag) * localVelocity.Normalized;
        return (localDrag);
    }
    

 
void ApplyStability(Vector3 axis,float stability)
{
 
  
    float currentLift = Vector3.Dot(CalculateLift(axis),axis);
    
    // Pitch up force proportional to lift
    RB.AddTorque(-axis * currentLift * -stability);
}
    float CalculateLinearAngleOfAttack()
    {
        Vector3 localVelocity = Actor.Transform.WorldToLocalVector(RB.LinearVelocity);
        float angle = Mathf.Atan2(-localVelocity.Y, localVelocity.Z); // up vs forward
        return angle;
    }
    void ApplyDrag()
    {
        Vector3 dragForce = CalculateDrag();

        RB.AddForce(-dragForce);
    }

    void ApplyLift(Vector3 axis)
    {
        Vector3 liftForce = CalculateLift( axis);
        Debug.Log(liftForce);
        RB.AddForce(liftForce );
    }
    public void ApplyThrust(float input)
    {
        if(input < 0) return;
        float thrustInPounds = TotalThrustInPounds;
        float appliedThrust = input * thrustInPounds;
        Vector3 localForward = Actor.Transform.Forward;
        Vector3 force = localForward * appliedThrust;
        
        RB.AddForce(force);

    }

    void UpdateState()
    {  
        ApplyThrust(Input.GetAxis("Vertical"));
        ApplyDrag();
        ApplyLift(Vector3.Right);
        RB.AddTorque(Actor.Transform.Forward * Input.GetAxis("Horizontal"),ForceMode.Acceleration);
        ApplyStability(Actor.Transform.Right,0.5f);
        ApplyStability(Actor.Transform.Up,0.1f);
        ApplyStability(Actor.Transform.Forward,0.0001f);
      
    }
    void UpdatePositionAndRotation()
    {
      
    }
}

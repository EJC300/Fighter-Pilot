using Plane;
using UnityEngine;

public class Missile : MonoBehaviour
{
    //Missile applies constant thrust and adjusts pitch and yaw to adjust itself to target via Proportional Guidance System It knows where it is always


    //SeekerTarget returns a target vector


    //Missile is an aircraft yeah no ? With it's throttle set to maximum at all times 
    public Transform targetTransform;
    public PlaneController aircraftController;
    //Calculation of Target
    public float NavigationRate = 5;

    Vector3 previousPosition;
   public Vector3 TargetVector;
    float previousUp;
    float previousLeft;
    float previousRoll;
    private void LateUpdate()
    {
        Seeker();
    }
    private void FixedUpdate()
    {
        ApplyThrottle();
        ApplyPitch();
        ApplyRoll();
        ApplyYaw();
    }
    void Seeker()
    {
        Vector3 reltativePosition = targetTransform.position - transform.position;
        Vector3 targetVelocity = (reltativePosition - previousPosition) / Time.fixedDeltaTime ;
        Vector3 relativeVelcity = targetVelocity - aircraftController.planeVelocity;
        previousPosition = reltativePosition;
        Vector3 LOSrate = Vector3.Cross(reltativePosition,relativeVelcity)/Vector3.Dot(reltativePosition,reltativePosition);

        Vector3 targetVector = LOSrate * NavigationRate;
       
        TargetVector = targetVector;
    }

    void ApplyYaw()
    {

        float left = Vector3.Dot(transform.up, TargetVector); 
        float leftDelta = ( previousLeft - left)/Time.fixedDeltaTime;
        aircraftController.ApplyYaw(leftDelta);

        previousLeft = left;
    }
    void ApplyPitch()
    {

        float up = Vector3.Dot(transform.right, TargetVector);
        float upDelta = (previousUp - up) / Time.fixedDeltaTime;
        aircraftController.ApplyPitch(upDelta);

        previousUp = up;
    }
    void ApplyRoll()
    {
        Vector3 direction = Vector3.Cross(TargetVector, transform.forward).normalized;
        float roll = Vector3.Dot(transform.forward, direction);
        float rollDelta = (previousRoll - roll)/Time.fixedDeltaTime;
        previousRoll = roll;
        aircraftController.ApplyRoll(rollDelta);
    }
    void ApplyThrottle()
    {
        float input = 1;
   

        aircraftController.ApplyThrottle(input);
    }

}

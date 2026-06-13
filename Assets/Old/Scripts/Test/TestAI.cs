using UnityEngine;
using Weapons;
using Utilities;
using Plane;
[RequireComponent(typeof(Rigidbody))]
public class TestAI : MonoBehaviour
{
    private VerticalStabilizer rudder;
    private Throttle rocketMotor;
    private Elevator elevator;
    public Seeker seeker;
    public Transform test;
    public float missileSpeed;
    public Rigidbody rb;
    private Vector3 previousTarget;
    private Vector3 relativeMissileVelocity;
    private void Start()
    {
        if (!seeker)
        {
            seeker = GetComponent<Seeker>();

            rb.linearVelocity = transform.forward * missileSpeed;
        
        }
        rocketMotor = GetComponent<Throttle>();
        elevator = GetComponent<Elevator>();
        rudder = GetComponent<VerticalStabilizer>();
    }
    //Lets the aerodynamics and control surfaces as a test then convert this into a full blown AAM missile.
    private void FixedUpdate()
    {
        relativeMissileVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        Vector3 target = seeker.CalculateTargetVector(test.position, rb.linearVelocity, missileSpeed) * Time.fixedDeltaTime;
        float throttle = 1;
        rocketMotor.ApplyThrottle(throttle);
        
        Debug.DrawRay(rb.position, target);
        //This method works well with missiles
        float up = Vector3.Dot(target ,transform.up);
        float right = Vector3.Dot(target,transform.right);

        MathHelpers.TorqueByRate(-up , transform.right, rb);
        MathHelpers.TorqueByRate((right) , transform.up, rb);
    

    }
    private void OnDrawGizmos()
    {
      
    }

}

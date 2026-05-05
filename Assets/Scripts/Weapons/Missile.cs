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
    public float maxFuelTime;
    public float ignitionTime;
    public float ignitionTimer;
    private float timer;
    Vector3 previousPosition;
    public Vector3 TargetVector;
    public float minImpactDistance;
    public float maxImpactDistance;
    public float blastRadius;
    public float damageAmount;
    Vector3 targetVelocity;
    

    private void LateUpdate()
    {
        Seeker();
    }
    private void FixedUpdate()
    {
        if (Mathf.Abs( targetVelocity.magnitude) < 0) return;
        ApplyThrottle();
        ApplyPitch();
        ApplyRoll();
        ApplyYaw();
        if (targetTransform != null)
        {
            if (Vector3.Distance(transform.position, targetTransform.position) < maxImpactDistance)
            {

                ImpactAndDetonate();
            }
        }
        
    }
    float Noise()
    {
        /*
         * if angle is perp from target to target Vector LOSRate is zero
         */
        Vector3 cross = Vector3.Cross(transform.up, (transform.position - TargetVector)).normalized;

        return Mathf.PerlinNoise1D( Vector3.Dot(targetTransform.right,cross));

    }

    void ImpactAndDetonate()
    {
        if(Physics.Raycast(new Ray(transform.position, transform.forward),out RaycastHit hit,minImpactDistance))
        {
           
            if(Physics.SphereCast(hit.point,blastRadius,transform.forward,out hit))
            {
                //Apply Damage
                Debug.Log("BOOM");
            }
            
            Destroy(gameObject);
        }
    }
    void Seeker()
    {

        if (targetTransform != null)
        {

            Vector3 reltativePosition = targetTransform.position - transform.position;
            targetVelocity = (previousPosition - targetTransform.position) / Time.fixedDeltaTime;
            Vector3 relativeVelcity = targetVelocity - aircraftController.planeVelocity;

            previousPosition = targetTransform.position;
            Vector3 LOSrate = Vector3.Cross(relativeVelcity, reltativePosition) / Vector3.Dot(reltativePosition, reltativePosition);

            Vector3 targetVector = LOSrate * (NavigationRate * Mathf.Abs(Noise())) * 2;

            TargetVector = targetVector;
        }
    }

    void ApplyYaw()
    {

        float left = -Vector3.Dot(transform.up, TargetVector);
        
        aircraftController.ApplyYaw(left);

       
    }
    void ApplyPitch()
    {

        float up = -Vector3.Dot(transform.right, TargetVector);
        
        
        aircraftController.ApplyPitch(up);

       
    }
    void ApplyRoll()
    {
        Vector3 direction = Vector3.Cross(transform.up, transform.right).normalized;
        float roll = Vector3.Dot(TargetVector, direction);
     
       
        aircraftController.ApplyRoll(roll);
    }
    float input = 0;
    void ApplyThrottle()
    {
        timer += Time.deltaTime;
        ignitionTimer += Time.deltaTime;
        if(ignitionTimer >= ignitionTime)
        {
            input = 1;
        }
        else
        {
            input = 0;
        }

        if (timer >= maxFuelTime)
        {
            timer = maxFuelTime;
            input = 0;

        }

            aircraftController.ApplyThrottle(input);
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position,new Vector3(10,10,10));
    }
}

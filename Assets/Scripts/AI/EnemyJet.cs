using Plane;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public enum EnemyState
{
    Avoid,
    Pursue,
    Attack,
    loiter
}
public class EnemyJet : MonoBehaviour
{
    public float NavigationRate;
    public PlaneController aircraft;

    public Transform player;

    private PlaneController playerAircraft;

    private Vector3 directionToPlayer;

    private Vector3 targetVelocity;

    private Vector3 previousTargetPosition;

   public Transform targetTransform;

   public Vector3 TargetVector;

    Vector3 previousTarget;
    

    private void Start()
    {
       
            player = GameObject.FindGameObjectWithTag("Player").transform;
            playerAircraft = player.gameObject.GetComponent<PlaneController>();

        
        
    }
    private void LateUpdate()
    {
        SolveForTargetPosition();
    }
    private void FixedUpdate()
    {
      
        AdjustThrottleRelativeToTarget();

        EvadeTarget();
       //PursueTarget();
       // AvoidTarget();
        
    }
    void ApplyYaw()
    {
        Vector3 directionToTarget = -(targetTransform.position  - transform.position + TargetVector.normalized).normalized;
        Vector3 direction = Vector3.Cross(transform.up, transform.forward);
        float left = -Vector3.Dot(direction, TargetVector);

        aircraft.ApplyYaw(left);


    }
 
    void PursueTarget()
    {
        Vector3 desiredDirection = (TargetVector + targetTransform.position - transform.position).normalized;

       
        Vector3 localDesired = transform.InverseTransformDirection(desiredDirection);

  



  
        aircraft.ApplyRoll(Mathf.Clamp(-localDesired.x, -1f, 1f));
        aircraft.ApplyPitch(Mathf.Clamp(-localDesired.y, -1f, 1f));

      
   
   







    }
    /*
     * 
     * TODO : Have Aircraft Maneuvere by:
     * Deciding when to break left or right when in front of the player, Half Loop, Barrel roll , Corkscrew or give chase then lead
     * Awareness of the ground
     * Collision Avoidance Steering
     * Energy Awareness to avoid stalling and allowing to corner when chasing
     * Deciding when to fire missiles and when to use guns
     */
    void EvadeTarget()
    {
        Vector3 desiredDirection = (targetTransform.position - transform.position ).normalized;
     
        //For now turn and break left 
        float angle = Vector3.Dot(transform.up,Vector3.up);
        bool infrontOFplayer =  Vector3.Dot(transform.right, desiredDirection) * Mathf.Rad2Deg < 45f && Mathf.Abs( Vector3.Dot(transform.forward, desiredDirection))* Mathf.Rad2Deg > 55f;
        float currentAngle = transform.up.y;
        Debug.Log(Vector3.Dot(transform.forward, desiredDirection) * Mathf.Rad2Deg);
        if(infrontOFplayer)
        {
            aircraft.ApplyRoll(0.0f- angle);
            if (angle >= 1)
            {
                Debug.Log(" In fRont");
                //aircraft.ApplyPitch(-1);

            }
        }
      
        else
        {
            Debug.Log("No Longer In fRont");
            aircraft.ApplyRoll(1f - angle);

        }
    }
    void AvoidTarget()
    {
        Vector3 desiredDirection = ( transform.position - targetTransform.position + TargetVector).normalized;


        Vector3 localDesired = transform.InverseTransformDirection(desiredDirection);






        aircraft.ApplyRoll(Mathf.Clamp(-localDesired.x, -1f, 1f));
        aircraft.ApplyPitch(Mathf.Clamp(-localDesired.y, -1f, 1f));
    }
    void AdjustThrottleRelativeToTarget()
    {
        Vector3 direction =  TargetVector -transform.position;
        float dot = Vector3.Dot(transform.forward, direction);

        float throttleInput = (direction.normalized - previousTarget).magnitude;
        Debug.Log(throttleInput);
        aircraft.ApplyThrottle(throttleInput);
        previousTarget = direction.normalized;
    }


    void SolveForTargetPosition()
    {
        Vector3 reltativePosition = targetTransform.position - transform.position;
        targetVelocity = (previousTargetPosition - targetTransform.position) / Time.fixedDeltaTime;
        Vector3 relativeVelcity = targetVelocity - aircraft.planeVelocity;

        previousTargetPosition = targetTransform.position;
        Vector3 LOSrate = Vector3.Cross(relativeVelcity, reltativePosition) / Vector3.Dot(reltativePosition, reltativePosition);

        Vector3 targetVector = LOSrate * NavigationRate;

        TargetVector = targetVector;
    }
}

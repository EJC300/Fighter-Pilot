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

    public float maxFaceAngle;

    public float RollPressure;

    public float BreakPressure;

    public float halfLoopPressure;

    public float corkScrewPressure;

    public float maxAbove;
    bool InfrontOfPlayer;

    bool NotLevelWithPlayer;

    bool fromBehindOfPlayer;
    float pressure;
    bool canBreak;
    float direction;
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

        EvasionBrain();

        EvadePlayer();
        BreakLeftORRight();
    } 
 

    /*
     * 
     * TODO : Have Aircraft Maneuver by:
     * Deciding when to break left or right when in front of the player, Half Loop, Barrel roll , Corkscrew or give chase then lead
     * Awareness of the ground
     * Collision Avoidance Steering
     * Energy Awareness to avoid stalling and allowing to corner when chasing
     * Deciding when to fire missiles and when to use guns
     */

    void BreakLeftORRight()
    {
       

        if (canBreak)
        {
     
            float angle = Vector3.Angle(transform.up, Vector3.up);
           
            if ((angle) > 55)
            {
                aircraft.ApplyRoll(0);
                aircraft.ApplyPitch(-1);
            }
            else
            {
                aircraft.ApplyRoll(-1);
            }
        }
               
        
    }
    void EvasionBrain()
    {
        Vector3 directionToTarget = (transform.position - targetTransform.position).normalized;
        InfrontOfPlayer = Vector3.Angle(transform.forward, directionToTarget) > maxFaceAngle;
        NotLevelWithPlayer = Mathf.Abs(transform.InverseTransformDirection(directionToTarget).y) >0.0f;
        fromBehindOfPlayer = Vector3.Angle(directionToTarget,transform.forward) < maxFaceAngle;
    }
    void EvadePlayer()
    {

        if (InfrontOfPlayer)
        {

            //Increase Pressure
            pressure = 0;
            Debug.Log("Player Is in front of me");
        }

        else if (fromBehindOfPlayer)
        {

            //Decrease Pressure
            pressure += Time.deltaTime * 10;
            Debug.Log("Player Is in behind me");
        }
        if (NotLevelWithPlayer)
        {   //DecreasePressure;

            Debug.Log("Player Is either above me or below me");
            pressure -= Time.deltaTime;
        }
        pressure = Mathf.Clamp(pressure, 0f, RollPressure + corkScrewPressure + halfLoopPressure + BreakPressure);
        canBreak = pressure > BreakPressure && pressure < RollPressure;
        if (pressure > BreakPressure && pressure < RollPressure)
        {
            Debug.Log("Break");
            if (Random.value > 0.5f )
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
         
           
        }

        else if (pressure > RollPressure && pressure < halfLoopPressure)
        {

            Debug.Log("Roll");
            return;

        }

        else if (pressure > corkScrewPressure && pressure < halfLoopPressure)
        {

            Debug.Log("Corkscrew");
            return;
        }
        else if (pressure > halfLoopPressure)
        {

            Debug.Log("HalfLoop");
            return;
        }
        else
        {
            PursueTarget();
        }
    }
    void PursueTarget()
    {
        Vector3 desiredDirection = (TargetVector + targetTransform.position - transform.position).normalized;


        Vector3 localDesired = transform.InverseTransformDirection(desiredDirection);






        aircraft.ApplyRoll(Mathf.Clamp(-localDesired.x, -1f, 1f));
        aircraft.ApplyPitch(Mathf.Clamp(-localDesired.y, -1f, 1f));











    }
    void AdjustThrottleRelativeToTarget()
    {
        Vector3 direction =  TargetVector -transform.position;
        float dot = Vector3.Dot(transform.forward, direction);
        float throttleInput = (direction.normalized - previousTarget).magnitude;
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

using Plane;
using UnityEngine;
using UnityEngine.Windows;
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
    public PIDController SinkPID;
    public PIDController PitchPID;
    public PIDController rollPID;
    public PIDController yawPID;
    public float NavigationRate;
    public PlaneController aircraft;

    public float maxPitch;

    public float maxYaw;

    public float maxRoll;
    public float PitchDamp;

    public float RollDamp;

    public float YawDamp;
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
    float previousSink;
    float direction;
 
    private void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerAircraft = player.gameObject.GetComponent<PlaneController>();



    }
    private void LateUpdate()
    {
     

    }
    private void FixedUpdate()
    {
        SolveForTargetPosition();
        AdjustThrottleRelativeToTarget();

        //EvasionBrain();
        Pursue();
       // EvadePlayer();
        //BreakLeftORRight();
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
  
    void Pursue()
    {
    
        Vector3 direction = transform.InverseTransformDirection(targetTransform.position - transform.position).normalized;


        /*
         float pitchRate = transform.InverseTransformDirection(aircraft.rb.angularVelocity).x;
         float altitudeDiff = direction.y;
         float sink =  ( SinkPID.CalculateResult(Time.fixedDeltaTime,aircraft.relativePlaneVelocity.y,altitudeDiff));

         float error = direction.y - pitchRate;
         float pitch = Mathf.Clamp( PitchPID.CalculateResult(Time.deltaTime,error, pitchRate * sink),-1,1);

         float roll = rollPID.CalculateResult(Time.deltaTime, direction.x, transform.InverseTransformDirection(aircraft.rb.angularVelocity).z * sink);
         float yaw = yawPID.CalculateResult(Time.deltaTime, direction.z, transform.InverseTransformDirection(aircraft.rb.angularVelocity).x);
         */
  
     
        float pitchRate = direction.y;
        float currentPitchRate = transform.InverseTransformDirection(aircraft.rb.angularVelocity).y;
        float correctedPitch =  (currentPitchRate) - (pitchRate - currentPitchRate * PitchDamp) ;



        float desiredBank = direction.x * 180f;
        float currentBank = Vector3.SignedAngle(transform.up, Vector3.up, transform.forward);
        float bankError = currentBank- desiredBank;



        aircraft.ApplyPitch(correctedPitch);
        
    
       aircraft.ApplyRoll(Mathf.Clamp( bankError/180f, -1,1));

       aircraft.ApplyYaw(direction.x);

    
    }
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
    //Working on Pursue Only Evasion Is on Hold.
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
        
        }
    }
  
    void AdjustThrottleRelativeToTarget()
    {
        Vector3 direction =  TargetVector -transform.position;

        float throttleInput = (direction.normalized).magnitude - previousTarget.magnitude ;
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

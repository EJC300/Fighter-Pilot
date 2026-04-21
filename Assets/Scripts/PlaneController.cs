
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Windows;

namespace Plane
{
    public class PlaneController : MonoBehaviour
    {
        // -------------------------------------------------------------------------
        // References
        // -------------------------------------------------------------------------

        public Rigidbody rb;
        public PIDController yawPID;
        public PIDController pitchPID;
        public PIDController rollPID;
        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        [HideInInspector] public Vector3 planeVelocity;
        [HideInInspector] public Vector3 relativePlaneVelocity;
        [HideInInspector] public float relativePitchVelocityPrev;
        [HideInInspector] public float pitchAcceleration;


        // -------------------------------------------------------------------------
        // Drag
        // -------------------------------------------------------------------------

        [Header("Drag")]
        public float CoefOfDrag;
        public AnimationCurve dragCurve;
       // public float CoefOfDragUp;
      //  public float CoefOfDragDown;
       // public float CoefOfDragLeft;
       // public float CoefOfDragRight;
       // public float CoefOfDragForward;
       // public float CoefOfDragBackward;
        public float CoefOfIDrag;
        //public AnimationCurve atmoDragForward;
      //  public AnimationCurve atmoDragBackward;
      //  public AnimationCurve atmoDragLeft;
      //  public AnimationCurve atmoDragRight;
      //  public AnimationCurve atmoDragTop;
      //  public AnimationCurve atmoDragDown;
        public AnimationCurve inducedDrag;
        // -------------------------------------------------------------------------
        // Lift
        // -------------------------------------------------------------------------

        [Header("Lift")]
        public float liftCoef;
        public AnimationCurve aoaCurve;
        public AnimationCurve aoaYawCurve;

        // -------------------------------------------------------------------------
        // Thrust
        // -------------------------------------------------------------------------

        [Header("Thrust")]
        public float totalThrust;
        public float throttleSpeed;

        [HideInInspector] public float currentThrottle;

        // -------------------------------------------------------------------------
        // Controls
        // -------------------------------------------------------------------------

        [Header("Controls")]
        public float pitchStrength;
        public float pitchRate;
        public float pitchInputStrength;
        public float yawStrength;
        public float rollInputStrength;
        public float rollStrength;
        public float yawInputStrength;
        
        // -------------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------------

        private void FixedUpdate()
        {
          
           
            ApplyLiftOnWings();
            ApplyInducedDragOnWings();
            ApplyAtmosphericDrag();
          
            UpdateAircraftPhysicsState();
        }

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------
        void TorqueByRate(float rate,Vector3 axis)
        { 
        
           if (Mathf.Abs(rate) < 0) return;
            Vector3 target = rate * axis;
            Vector3 current = Vector3.Dot(axis, transform.InverseTransformDirection(rb.angularVelocity)) * axis;
            Vector3 error = target - current;
           
            rb.AddRelativeTorque(target , ForceMode.Acceleration);

        }
        void UpdateAircraftPhysicsState()
        {
            planeVelocity = rb.linearVelocity;
            relativePlaneVelocity = transform.InverseTransformDirection(planeVelocity);
            ApplyNaturalYawStability();
            ApplyNaturalPitchStability();
            ApplyNaturalRollStability();
        }

        // -------------------------------------------------------------------------
        // Angle Of Attack
        // -------------------------------------------------------------------------

        public float CalculateAngleOfAttack()
        {
            return Mathf.Atan2(-relativePlaneVelocity.y, relativePlaneVelocity.z);
        }

        public float CalculateAngleOfAttackYaw()
        {
            return Mathf.Atan2(-relativePlaneVelocity.x, relativePlaneVelocity.z);
        }

        // -------------------------------------------------------------------------
        // Lift
        // -------------------------------------------------------------------------

        float CalculateLiftOnWings()
        {
            Vector3 velocity = relativePlaneVelocity;
         
            float aoa = aoaCurve.Evaluate(CalculateAngleOfAttack() * Mathf.Rad2Deg);
            float liftAmount = 0.5f * velocity.sqrMagnitude * aoa * liftCoef;




            return liftAmount;
        }
     
        float CalculateInducedDrag(float liftCoef)
        {
           
            float inducedDragCurve = inducedDrag.Evaluate(Mathf.Max(0, relativePlaneVelocity.z));
            float v2 = relativePlaneVelocity.sqrMagnitude;

            return 0.5f * v2 * CoefOfIDrag * inducedDragCurve;
        }
        void ApplyInducedDragOnWings()
        {
      
           
            float inducedDrag = CalculateInducedDrag(CalculateLiftOnWings());

            rb.AddRelativeForce(inducedDrag * -relativePlaneVelocity.normalized);


        }
        void ApplyLiftOnWings()
        {
            Vector3 velocity = relativePlaneVelocity;
            Vector3 liftDirection = Vector3.Cross(velocity, Vector3.right).normalized;
            Vector3 lift = liftDirection * CalculateLiftOnWings();
            rb.AddRelativeForce(lift);
        }


        // -------------------------------------------------------------------------
        // Natural Pitch Stability
        // -------------------------------------------------------------------------

        void ApplyNaturalPitchStability()
        {
            Vector3 velocity = relativePlaneVelocity;
            float liftMagnitude = CalculateLiftOnWings();

            float currentPitch = transform.InverseTransformDirection(rb.angularVelocity).y;

            float controlledPitch = transform.InverseTransformDirection(rb.angularVelocity).z;
            float pressure = 0.5f * velocity.sqrMagnitude;
            float pitchError = controlledPitch - currentPitch;
            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * pitchError;

            controlAuthority = Mathf.Clamp(controlAuthority, -pitchStrength, pitchStrength);
            if (Mathf.Abs(pressure) > 0.0f)
            {
                TorqueByRate(controlAuthority, Vector3.right);
            }
        }
        void ApplyNaturalRollStability()
        {
            Vector3 velocity = relativePlaneVelocity;
            float liftMagnitude = CalculateLiftOnWings();

            float currentRoll = transform.InverseTransformDirection(rb.angularVelocity).z;

            float controlledRoll = transform.InverseTransformDirection(rb.angularVelocity).y;
            float pressure = 0.5f * velocity.sqrMagnitude;
            float rollError = controlledRoll - currentRoll;
            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * rollError;

            controlAuthority = Mathf.Clamp(controlAuthority, -rollStrength, rollStrength);
            if (Mathf.Abs(pressure) > 0.0f)
            {
                TorqueByRate(controlAuthority, Vector3.forward);
            }
        }
        void ApplyNaturalYawStability()
        {

            Vector3 velocity = relativePlaneVelocity;
            float liftMagnitude = CalculateLiftOnWings();

            float currentYaw = transform.InverseTransformDirection(rb.angularVelocity).y;

            float controlledYaw = transform.InverseTransformDirection(rb.angularVelocity).z;
            float pressure = 0.5f * velocity.sqrMagnitude;
            float yawError = controlledYaw - currentYaw;
            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * yawError;

            controlAuthority = Mathf.Clamp(controlAuthority, -yawStrength, yawStrength);
            if (Mathf.Abs(pressure) > 0.0f)
            {
                TorqueByRate(controlAuthority, -Vector3.up);
            }
        }
        // -------------------------------------------------------------------------
        // Drag
        // -------------------------------------------------------------------------

        void ApplyAtmosphericDrag()
        {
            /*
            float speedForward = relativePlaneVelocity.z;
            float speedRight = relativePlaneVelocity.x;
            float speedUp = relativePlaneVelocity.y;

            float dragForward = speedForward > 0
                ? -atmoDragForward.Evaluate(speedForward) * CoefOfDragForward
                : atmoDragBackward.Evaluate(speedForward) * CoefOfDragBackward;

            float dragRight = speedRight > 0
                ? -atmoDragLeft.Evaluate(speedRight) * CoefOfDragRight
                : atmoDragRight.Evaluate(speedRight) * CoefOfDragLeft;

            float dragUp = speedUp > 0
                ? atmoDragTop.Evaluate(speedUp) * CoefOfDragUp
                : atmoDragDown.Evaluate(speedUp) * CoefOfDragDown;

            */
            float dragCoef = dragCurve.Evaluate(relativePlaneVelocity.magnitude) * CoefOfDrag;
            Vector3 dragForce = 0.5f * relativePlaneVelocity.normalized * dragCoef *  relativePlaneVelocity.sqrMagnitude;
                                
            Debug.Log(dragForce.ToString());
            rb.AddRelativeForce(-dragForce);
        }

        // -------------------------------------------------------------------------
        // Thrust
        // -------------------------------------------------------------------------

        public void ApplyThrottle(float input)
        {
            currentThrottle = Mathf.Clamp01(currentThrottle + input * throttleSpeed * Time.deltaTime);
          
            rb.AddRelativeForce( Vector3.forward * currentThrottle * totalThrust,ForceMode.Acceleration);
        }

        public void ApplyPitch(float input)
        {
            Vector3 velocity = relativePlaneVelocity;


            float currentPitchRate = transform.InverseTransformDirection(rb.angularVelocity).x;
            float targetPitchRate = input * pitchInputStrength;
            float pressure = 0.5f * velocity.sqrMagnitude;
            float pitchError = input - currentPitchRate;
            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * pitchError;


            controlAuthority = Mathf.Clamp(controlAuthority, -pitchRate, pitchRate);
            if (Mathf.Abs(pressure) > 0.0f)
            {
                float flyByWirePitch = pitchPID.CalculateResult(Time.fixedDeltaTime, controlAuthority, currentPitchRate);



                if (Mathf.Abs(flyByWirePitch) > 0.0f)
                {

                    TorqueByRate(flyByWirePitch, Vector3.right);
                }
            }

        }

        
        public void ApplyRoll(float input)
        {
            Vector3 velocity = relativePlaneVelocity;


            float liftMagnitude = CalculateLiftOnWings();

            float currentRoll = transform.InverseTransformDirection(rb.angularVelocity).z;

            float controlledRoll = input * 5;
            float pressure = 0.5f * velocity.sqrMagnitude;
            float rollError = controlledRoll - currentRoll;
            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * rollError;

            controlAuthority = Mathf.Clamp(controlAuthority, -rollInputStrength, rollInputStrength);
            if (Mathf.Abs(pressure) > 0.0f)
            {
                float flyByWireRoll = rollPID.CalculateResult(Time.fixedDeltaTime, controlAuthority, currentRoll);
                //rb.AddRelativeTorque(Vector3.forward * controlAuthority * rollInputStrength,ForceMode.Acceleration);
                TorqueByRate(flyByWireRoll, Vector3.forward);
            }
        }

        public void ApplyYaw(float input)
        {
            Vector3 velocity = relativePlaneVelocity;
            
            float liftMagnitude = CalculateLiftOnWings();

            float currentYaw = transform.InverseTransformDirection(rb.angularVelocity).y;
             
            float controlledYaw = input * yawInputStrength;
            float pressure = 0.5f * velocity.sqrMagnitude;
            
            float yawError = controlledYaw - currentYaw;
            float controlAuthority = Mathf.Clamp( pressure / velocity.z,-yawError, yawError);

            if (Mathf.Abs(pressure) > 0.0f)
            {
                float flyByWireYaw = yawPID.CalculateResult(Time.fixedDeltaTime, controlAuthority, currentYaw);



                TorqueByRate(flyByWireYaw, Vector3.up);
            }
            
        }
    }
}
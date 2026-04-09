using UnityEngine;
using static UnityEngine.GraphicsBuffer;


namespace Plane
{
 

public class PlaneController : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody rb;

        [Header("Drag Coefficients")]
        public float dragForward;
        public float dragBackward;
        public float dragLeft;
        public float dragRight;
        public float dragTop;
        public float dragBottom;

        [Header("Drag Curves")]
        public AnimationCurve dragForwardCurve;
        public AnimationCurve dragBackwardCurve;
        public AnimationCurve dragLeftCurve;
        public AnimationCurve dragRightCurve;
        public AnimationCurve dragTopCurve;
        public AnimationCurve dragBottomCurve;

        [Header("Lift")]
        public float liftAmount;
        public AnimationCurve aoaCurve;

        [Header("Induced Drag")]
        public float inducedDragAmount;
        public AnimationCurve inducedDragCurve;

        [Header("Thrust")]
        public float totalThrust;
        public float throttleSpeed;

        [Header("Stall")]
        public float stallSpeed;
        public float stallWarningSpeed;

        [Header("Angle Of Incidence")]
        public float angleOfIncidence = 0.5f;

        [Header("Control Strength")]
        public float pitchStrength = 1.0f;
        public float rollStrength = 1.0f;
        public float yawStrength = 1.0f;
        public float turnSpeed = 0f;

        [Header("PID Controllers")]
        public PIDController pitchPID = new PIDController();
        public PIDController rollPID = new PIDController();
        public PIDController yawPID = new PIDController();

        // Private State
        private float currentThrottle;

        // -------------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------------

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            UpdateState();
        }

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        void UpdateState()
        {
            ApplyDragForces();
            ApplyLiftForces(Vector3.right);
            ApplyCoordinatedTurn();
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private Vector3 GetLocalVelocity()
        {
            return Quaternion.Inverse(transform.rotation) * rb.linearVelocity;
        }

        private Vector3 GetVelocity() => rb.linearVelocity;

        private float StallFactor()
        {
            float forwardSpeed = Mathf.Max(0f, GetLocalVelocity().z);
            return Mathf.Clamp01((forwardSpeed - stallSpeed) / (stallWarningSpeed - stallSpeed));
        }

        private float CalculateAOA()
        {
            var velocity = GetLocalVelocity();
            return Mathf.Atan2(-velocity.y, velocity.z) + angleOfIncidence;
        }

        // -------------------------------------------------------------------------
        // Drag
        // -------------------------------------------------------------------------

        void ApplyDragForces()
        {
            var velocity = GetLocalVelocity();

            float xCd = velocity.z > 0 ? dragForward : dragBackward;
            float yCd = velocity.y > 0 ? dragBottom : dragTop;
            float zCd = velocity.x > 0 ? dragRight : dragLeft;

            float vx2 = velocity.x * Mathf.Abs(velocity.x);
            float vy2 = velocity.y * Mathf.Abs(velocity.y);
            float vz2 = velocity.z * Mathf.Abs(velocity.z);

            float forwardCoef = dragForwardCurve.Evaluate(Mathf.Abs(velocity.z)) * xCd * vz2;
            float upCoef = dragTopCurve.Evaluate(Mathf.Abs(velocity.y)) * yCd * vy2;
            float leftCoef = dragLeftCurve.Evaluate(Mathf.Abs(velocity.x)) * zCd * vx2;

            Vector3 drag = new Vector3(leftCoef, upCoef, forwardCoef);
            rb.AddRelativeForce(-drag);
        }

        // -------------------------------------------------------------------------
        // Lift
        // -------------------------------------------------------------------------

        Vector3 CalculateLift(Vector3 right)
        {
            var liftVelocity = Vector3.ProjectOnPlane(GetLocalVelocity(), right);
            var liftVelocitySquard = liftVelocity.sqrMagnitude;
            var liftCoef = aoaCurve.Evaluate(CalculateAOA() * Mathf.Rad2Deg);
            var liftPower = liftVelocitySquard * liftCoef * liftAmount;
            var liftDirection = Vector3.Cross(liftVelocity.normalized, right);
            var lift = liftDirection * liftPower;
            var dragCoef = liftCoef * liftCoef;
            var dragPower = liftVelocitySquard * dragCoef * inducedDragCurve.Evaluate(Mathf.Max(0, GetLocalVelocity().z));
            var inducedDragDirection = -liftVelocity.normalized;
            var drag = inducedDragDirection * dragPower;
            return lift + drag;
        }

        void ApplyLiftForces(Vector3 right)
        {
            rb.AddRelativeForce(CalculateLift(right));
        }

        // -------------------------------------------------------------------------
        // Coordinated Turn
        // -------------------------------------------------------------------------

        void ApplyCoordinatedTurn()
        {
            Vector3 liftForce = CalculateLift(Vector3.right);
            Vector3 horizontalLift = Vector3.ProjectOnPlane(liftForce, Vector3.up);
            float turnForce = horizontalLift.magnitude * Vector3.Dot(horizontalLift.normalized, transform.right);

            rb.AddTorque(transform.up * turnForce * turnSpeed * StallFactor());
        }

        // -------------------------------------------------------------------------
        // Thrust
        // -------------------------------------------------------------------------

        public void ApplyThrottle(float input)
        {
            currentThrottle = Mathf.Clamp01(currentThrottle + input * throttleSpeed * Time.fixedDeltaTime);
            if (currentThrottle <= 0f) return;
            rb.AddRelativeForce(Vector3.forward * currentThrottle * totalThrust);
        }

        // -------------------------------------------------------------------------
        // Flight Controls
        // -------------------------------------------------------------------------

        public void ApplyPitch(float input)
        {
            Vector3 localAngular = transform.InverseTransformDirection(rb.angularVelocity);
            float correction = pitchPID.CalculateResult(Time.fixedDeltaTime, input, localAngular.x);
            rb.AddTorque(-transform.right * correction * pitchStrength * StallFactor());
        }

        public void ApplyRoll(float input)
        {
            Vector3 localAngular = transform.InverseTransformDirection(rb.angularVelocity);
            float correction = rollPID.CalculateResult(Time.fixedDeltaTime, input, localAngular.z);
            rb.AddTorque(transform.forward * correction * rollStrength * StallFactor());
        }

        public void ApplyYaw(float input)
        {
            Vector3 localAngular = transform.InverseTransformDirection(rb.angularVelocity);
            float correction = yawPID.CalculateResult(Time.fixedDeltaTime, input, localAngular.y);
            rb.AddTorque(transform.up * correction * yawStrength * StallFactor());
        }
    }
}

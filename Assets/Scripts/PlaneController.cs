
using UnityEngine;

namespace Plane
{
    public class PlaneController : MonoBehaviour
    {
        // -------------------------------------------------------------------------
        // References
        // -------------------------------------------------------------------------

        public Rigidbody rb;

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        [HideInInspector] public Vector3 planeVelocity;
        [HideInInspector] public Vector3 relativePlaneVelocity;

        // -------------------------------------------------------------------------
        // Drag
        // -------------------------------------------------------------------------

        [Header("Drag")]
        public float CoefOfDrag;

        public AnimationCurve atmoDragForward;
        public AnimationCurve atmoDragBackward;
        public AnimationCurve atmoDragLeft;
        public AnimationCurve atmoDragRight;
        public AnimationCurve atmoDragTop;
        public AnimationCurve atmoDragDown;
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
        public float yawStrength;

        // -------------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------------

        private void FixedUpdate()
        {
            UpdateAircraftPhysicsState();
            ApplyAtmosphericDrag();
            ApplyLiftOnWings();
            ApplyInducedDragOnWings();
            ApplyLiftOnVerticalStabilizer();
            ApplyNaturalYawStability();
        }

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        void UpdateAircraftPhysicsState()
        {
            planeVelocity = rb.linearVelocity;
            relativePlaneVelocity = transform.InverseTransformDirection(planeVelocity);
            ApplyNaturalPitchStability();
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
            Vector3 relativeVelocityOnWings = Vector3.Dot(relativePlaneVelocity, transform.forward) * transform.forward;

            float aoa = CalculateAngleOfAttack();
            float liftCurveValue = aoaCurve.Evaluate(aoa + 0.5f * Mathf.Rad2Deg);
            float velocitySquared = relativeVelocityOnWings.sqrMagnitude;
            float liftOnWings = 0.5f * liftCoef * velocitySquared * liftCurveValue;

            // TODO: Replace with proper induced drag formula
            float inducedDrag = CalculateInducedDrag(liftOnWings);

            return liftOnWings;
        }
        float CalculateLiftOnVerticalStabilizer()
        {
            Vector3 relativeVelocityOnWings = Vector3.Dot(relativePlaneVelocity, transform.forward) * transform.forward;

            float aoa = CalculateAngleOfAttackYaw();
            float liftCurveValue = aoaYawCurve.Evaluate(aoa * Mathf.Rad2Deg);
            float velocitySquared = relativeVelocityOnWings.sqrMagnitude;
            float liftOnVerticalStabilizer = 0.5f * liftCoef * velocitySquared * liftCurveValue;

            // TODO: Replace with proper induced drag formula
          
            return liftOnVerticalStabilizer;
        }
        float CalculateInducedDrag(float liftCoef)
        {
            Vector3 relativeVelocityOnWings = Vector3.Dot(relativePlaneVelocity, transform.forward) * transform.forward;
            float inducedDragCurve = inducedDrag.Evaluate(Mathf.Max(0, relativePlaneVelocity.z));
            float liftSquared = liftCoef;

            return liftSquared * inducedDragCurve;
        }
        void ApplyInducedDragOnWings()
        {
      
            Vector3 wingCross = Vector3.Cross(transform.forward, transform.right);
            Vector3 liftDirection = Vector3.ProjectOnPlane((Vector3.up * CalculateLiftOnWings()).normalized, wingCross);
            float inducedDrag = CalculateInducedDrag(CalculateLiftOnWings());

            rb.AddForce(inducedDrag * relativePlaneVelocity.normalized);


        }
        void ApplyLiftOnWings()
        {
            Vector3 relativeVelocityOnWings = Vector3.ProjectOnPlane(relativePlaneVelocity, transform.forward);
            Vector3 wingCross = Vector3.Cross(relativeVelocityOnWings.normalized, transform.right);
            Vector3 liftDirection = Vector3.ProjectOnPlane(relativeVelocityOnWings.normalized, wingCross);
            float liftMagnitude = CalculateLiftOnWings();

            Vector3 liftForce = liftMagnitude * liftDirection;
            rb.AddForce(-liftForce);
        }
        void ApplyLiftOnVerticalStabilizer()
        {
            Vector3 relativeVelocityOnVerticalStabilizer = Vector3.ProjectOnPlane(relativePlaneVelocity, transform.forward);
            Vector3 verticalStabilizerCross = Vector3.Cross(relativeVelocityOnVerticalStabilizer.normalized, transform.forward);
            Vector3 liftDirection = Vector3.ProjectOnPlane(relativeVelocityOnVerticalStabilizer.normalized, verticalStabilizerCross);
            float liftMagnitude = CalculateLiftOnVerticalStabilizer();

            Vector3 liftForce = -liftMagnitude * liftDirection;
            rb.AddForce(liftForce);
        }

        // -------------------------------------------------------------------------
        // Natural Pitch Stability
        // -------------------------------------------------------------------------

        void ApplyNaturalPitchStability()
        {
            float liftMagnitude = CalculateLiftOnWings();
            float currentPitch = transform.InverseTransformDirection(rb.angularVelocity).x;
            float targetPitch = Mathf.Atan2(-relativePlaneVelocity.y, relativePlaneVelocity.z);
            float rotationError = currentPitch - targetPitch;

            rb.AddRelativeTorque(transform.right * rotationError * pitchStrength * liftMagnitude * Time.fixedDeltaTime);
        }
        void ApplyNaturalYawStability()
        {
            float yawLift = CalculateLiftOnVerticalStabilizer();
            float currentYaw = transform.InverseTransformDirection(rb.angularVelocity).y;
            float targetYaw = Mathf.Atan2(-relativePlaneVelocity.x, relativePlaneVelocity.z);
            float rotationError = currentYaw - targetYaw;

            rb.AddRelativeTorque(transform.up * rotationError * pitchStrength * yawLift* Time.fixedDeltaTime);
        }
        // -------------------------------------------------------------------------
        // Drag
        // -------------------------------------------------------------------------

        void ApplyAtmosphericDrag()
        {
            float speedForward = relativePlaneVelocity.z;
            float speedRight = relativePlaneVelocity.x;
            float speedUp = relativePlaneVelocity.y;

            float dragForward = speedForward > 0
                ? atmoDragForward.Evaluate(speedForward) * CoefOfDrag
                : -atmoDragBackward.Evaluate(speedForward) * CoefOfDrag;

            float dragRight = speedRight > 0
                ? atmoDragLeft.Evaluate(speedRight) * CoefOfDrag
                : -atmoDragRight.Evaluate(speedRight) * CoefOfDrag;

            float dragUp = speedUp > 0
                ? atmoDragTop.Evaluate(speedUp) * CoefOfDrag
                : -atmoDragDown.Evaluate(speedUp) * CoefOfDrag;

            Vector3 dragForce = 0.5f * new Vector3(dragRight, dragUp, dragForward).sqrMagnitude
                                * relativePlaneVelocity.normalized;

            rb.AddForce(-dragForce);
        }

        // -------------------------------------------------------------------------
        // Thrust
        // -------------------------------------------------------------------------

        public void ApplyThrottle(float input)
        {
            currentThrottle = Mathf.Clamp01(currentThrottle + input * throttleSpeed * Time.deltaTime);
            rb.AddRelativeForce(Vector3.forward * currentThrottle * totalThrust);
        }

        public void ApplyPitch(float input)
        {
            //
        }
        public void ApplyRoll(float input)
        {
            //
        }

        public void ApplyYaw(float input)
        {
            //
        }
    }
}
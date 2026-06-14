using UnityEngine;
using Utilities;
namespace Behavior
{
    [System.Serializable]
    public class FlightPhysics
    {
        private Vector3 planeVelocity;
        public Vector3 PlaneVelocity { get { return planeVelocity; } set { planeVelocity = value; } }
        private Vector3 relativeVelocity;
        public Vector3 RelativeVelocity { get { return relativeVelocity; } set { relativeVelocity = value; } }
        public void InducedDrag(AnimationCurve inducedDragCurve, Rigidbody rb, float inducedDragCoef)
        {
            float inducedDrag = inducedDragCoef * inducedDragCurve.Evaluate(relativeVelocity.z);
            MathHelpers.ApplyInducedDragOnWings(rb, inducedDrag, relativeVelocity);
        }

        public void Drag(AnimationCurve dragCurve, Rigidbody rb, float dragCoef)
        {
            float drag = dragCurve.Evaluate(relativeVelocity.magnitude) * dragCoef;
            Vector3 dragForce = 0.5f * relativeVelocity.normalized * dragCoef * relativeVelocity.sqrMagnitude;

            float speed = relativeVelocity.magnitude;

            rb.AddRelativeForce(-dragForce);
        }

        public void Lift(Rigidbody rb, AnimationCurve aoaCurve, float liftCoef)
        {
            float lift = MathHelpers.CalculateLiftOnWings(liftCoef, relativeVelocity, aoaCurve);
            Vector3 liftDirection = Vector3.Cross(planeVelocity, Vector3.right).normalized;
            rb.AddForce(liftDirection * lift);
        }

        public void YawEffect(Transform transform, Rigidbody rb, float liftCoef, float minSpeed)
        {
            if (relativeVelocity.z > minSpeed)
            {
                float bankAngle = Vector3.Dot(transform.right, Physics.gravity.normalized);
                float dropInfluence = (bankAngle / relativeVelocity.z) * (liftCoef * liftCoef) * Time.fixedDeltaTime;

                MathHelpers.TorqueByRate(dropInfluence, Vector3.up, rb);
            }
        }

        public void Stall(Rigidbody rb, float maxStallSpeed)
        {
            float gravityAccel = Physics.gravity.sqrMagnitude * 0.01f;
            Transform transform = rb.transform;
            bool stalling = Mathf.Approximately(relativeVelocity.z, maxStallSpeed);
            if (stalling)
            {

                Vector3 direction = Vector3.Lerp(relativeVelocity, Vector3.Cross(transform.right, transform.forward).normalized, gravityAccel);



                MathHelpers.TorqueByRate(relativeVelocity.z * 0.1f, direction.normalized, rb);
            }
        }

        public void ApplyYaw(Rigidbody rb, float input, float controlRate, Vector3 relativePlaneVelocity)
        {

            Vector3 velocity = relativePlaneVelocity;


            float angularRate = rb.angularVelocity.y;

            float pressure = 0.5f * velocity.sqrMagnitude;

            float pitchError = (input) - angularRate;

            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * pitchError * Time.fixedDeltaTime * controlRate;

            if (Mathf.Abs(pressure) > 0.0f)
            {



                MathHelpers.TorqueByRate(controlAuthority, rb.transform.up, rb);
            }


        }
        public void ApplyPitch(Rigidbody rb, float input, float controlRate, Vector3 relativePlaneVelocity)
        {

            Vector3 velocity = relativePlaneVelocity;


            float angularRate = rb.angularVelocity.x;

            float pressure = 0.5f * velocity.sqrMagnitude;

            float pitchError = (input) - angularRate;

            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * pitchError * Time.fixedDeltaTime * controlRate;

            if (Mathf.Abs(pressure) > 0.0f)
            {



                MathHelpers.TorqueByRate(controlAuthority, rb.transform.right, rb);
            }


        }
        public void ApplyRoll(Rigidbody rb, float input, float controlRate, Vector3 relativePlaneVelocity)
        {

            Vector3 velocity = relativePlaneVelocity;


            float angularRate = rb.angularVelocity.z;

            float pressure = 0.5f * velocity.sqrMagnitude;

            float pitchError = (input) - angularRate;

            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * pitchError * Time.fixedDeltaTime * controlRate;

            if (Mathf.Abs(pressure) > 0.0f)
            {



                MathHelpers.TorqueByRate(controlAuthority, rb.transform.up, rb);
            }


        }
        public void ApplyThrottle(Rigidbody rb, float input, float totalThrust, float currentThrottle, float throttleSpeed, float thrustRatio)
        {
            if (rb == null) return;
            currentThrottle = Mathf.Clamp01(currentThrottle + input * throttleSpeed * Time.fixedDeltaTime);

            Vector3 maxThrust = Vector3.forward * currentThrottle * totalThrust;
            Vector3 thrust = maxThrust * rb.mass * thrustRatio;
            rb.AddRelativeForce(thrust);


        }
    }
}

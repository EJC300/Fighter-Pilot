using UnityEngine;
using Utilities;
namespace Plane
{
    [RequireComponent(typeof(Rigidbody))]
    public class Aerodynamics : MonoBehaviour
    {
        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public float liftCoef;
        public float inducedDragCoef;
        public float dragCoef;
        public AnimationCurve aoaCurve;
        public AnimationCurve dragCurve;
        public AnimationCurve inducedDragCurve;
        private Vector3 planeVelocity;
        private Vector3 relativeVelocity;


        private void FixedUpdate()
        {
            UpdateAircraftPhysicsState();
        }
        private void UpdateAircraftPhysicsState()
        {
            planeVelocity = rb.linearVelocity;
            relativeVelocity = transform.InverseTransformDirection(planeVelocity);
            ApplyInducedDrag();
            ApplyAtmosphericDrag();
            ApplyLift();
            if (relativeVelocity.z > 5)
            {
                rb.linearVelocity = rb.linearVelocity * WorldSettings.speedScale;
                rb.angularVelocity = rb.angularVelocity * WorldSettings.turnScale;
            }
        }

        private void ApplyLift()
        {
            float lift = MathHelpers.CalculateLiftOnWings(liftCoef, relativeVelocity, aoaCurve);
            Vector3 liftDirection = Vector3.Cross(planeVelocity, Vector3.right).normalized;
            rb.AddForce(liftDirection * lift);
        }

        private void ApplyInducedDrag()
        {
            float inducedDrag = inducedDragCoef * inducedDragCurve.Evaluate(relativeVelocity.z);
            MathHelpers.ApplyInducedDragOnWings(rb, inducedDrag, relativeVelocity);
        }
    
        void ApplyAtmosphericDrag()
        {
        
            float drag = dragCurve.Evaluate(relativeVelocity.magnitude) * dragCoef;
            Vector3 dragForce = 0.5f * relativeVelocity.normalized * dragCoef * relativeVelocity.sqrMagnitude;

            float speed = relativeVelocity.magnitude;

            rb.AddRelativeForce(-dragForce);
        }
    }
}
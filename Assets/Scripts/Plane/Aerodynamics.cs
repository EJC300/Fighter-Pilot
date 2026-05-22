using UnityEngine;
using Utilities;
namespace Plane
{
    [RequireComponent(typeof(Rigidbody))]
    public class Aerodynamics : MonoBehaviour
    {

        //May Switch from curves to a more fixed calculation 

        //TODO Plane needs to yaw a towards the gravity 
        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public float liftCoef;
        public float inducedDragCoef;
        public float dragCoef;
        public AnimationCurve aoaCurve;
        public AnimationCurve dragCurve;
        public AnimationCurve inducedDragCurve;
        private Vector3 planeVelocity;
        private Vector3 relativeVelocity;

        public Vector3 RelativeVelocity { get { return relativeVelocity; } }

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
                YawEffect();
                rb.linearVelocity = rb.linearVelocity * WorldSettings.speedScale;
                rb.angularVelocity = rb.angularVelocity * WorldSettings.turnScale;

            }
        }


        private void YawEffect()
        {
            float bankAngle = Vector3.Dot(transform.right,Physics.gravity.normalized);
            float dropInfluence = (bankAngle/ relativeVelocity.z) * (liftCoef * liftCoef) * Time.fixedDeltaTime;
            
            MathHelpers.TorqueByRate(dropInfluence, Vector3.up, rb);

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
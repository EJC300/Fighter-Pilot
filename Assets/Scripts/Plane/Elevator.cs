using UnityEngine;
using Utilities;

namespace Plane
{
    [RequireComponent(typeof(Rigidbody))]
    public class Elevator : MonoBehaviour
    {
     
        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public float controlRate;
        public void ApplyElevator(float input, Vector3 relativePlaneVelocity, float angularRate)
        {
            if (rb == null) return;
            Vector3 velocity = relativePlaneVelocity;


            float pressure = 0.5f * relativePlaneVelocity.sqrMagnitude;

            float pitchError = ((input) - angularRate) * Time.fixedDeltaTime;
            float controlAuthority = Mathf.Clamp01(pressure / relativePlaneVelocity. z) * pitchError;
            if (Mathf.Abs(pressure) > 0.0f)
            {



       

                MathHelpers.TorqueByRate(controlAuthority * controlRate, Vector3.right, rb);
            }
       

        }
    }
}

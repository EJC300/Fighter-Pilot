using UnityEngine;
using Utilities;

namespace Plane
{
    [RequireComponent(typeof(Rigidbody))]
    public class Ailerons : MonoBehaviour
    {
       
        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public float controlRate;
        public void ApplyAilerons(float input, Vector3 relativePlaneVelocity, float angularRate)
        {
            if (rb == null) return;
            Vector3 velocity = relativePlaneVelocity;




            float pressure = 0.5f * relativePlaneVelocity.sqrMagnitude;

            float pitchError = ((input) - angularRate) *Time.fixedDeltaTime;
            float controlAuthority = Mathf.Clamp01(pressure / relativePlaneVelocity.z) * controlRate * pitchError;

            if (Mathf.Abs(pressure) > 0.0f)
            {



         
               
                MathHelpers.TorqueByRate(controlAuthority , Vector3.forward, rb);
            }


        }
    }
}

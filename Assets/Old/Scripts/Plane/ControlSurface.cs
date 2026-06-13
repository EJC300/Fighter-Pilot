using UnityEngine;
using UnityEngine.Animations;

namespace Plane
{
    [RequireComponent(typeof(Rigidbody))]
    public class ControlSurface : MonoBehaviour
    {
        [SerializeField] private string controlName;
        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public float CalculateControlAuthority(float input, Vector3 relativePlaneVelocity,float rate,float angularRate)
        {
            if(rb == null) return 0f;
            Vector3 velocity = relativePlaneVelocity;

          


            float pressure = 0.5f * velocity.sqrMagnitude;

            float pitchError = (input) - angularRate;

            float controlAuthority = Mathf.Clamp01(pressure / velocity.z) * pitchError;
            Debug.Log(controlAuthority);

            if (Mathf.Abs(pressure) > 0.0f)
            {

                return controlAuthority;
            }
            else
            {
                return 0;

            }
     
        }
    }
}

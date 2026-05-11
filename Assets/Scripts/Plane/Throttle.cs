using UnityEngine;
namespace Plane
{
    [RequireComponent (typeof (Rigidbody))]
    public class Throttle : MonoBehaviour
    {
        public float thrustRatio;
        public float totalThrust;
        public float throttleSpeed;
        private Rigidbody rb {  get { return GetComponent<Rigidbody>(); } }
        private float currentThrottle;


        private void Start()
        {
            currentThrottle = 1;
        }
        public void ApplyThrottle(float input)
        {
            if (rb == null) return;
            currentThrottle = Mathf.Clamp01(currentThrottle + input * throttleSpeed * Time.fixedDeltaTime);

            Vector3 maxThrust = Vector3.forward * currentThrottle * totalThrust;
            Vector3 thrust = maxThrust * rb.mass * thrustRatio;
            rb.AddRelativeForce(thrust);


        }
    }

}
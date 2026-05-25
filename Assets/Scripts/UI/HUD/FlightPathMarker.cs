using Plane;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
    public class FlightPathMarker : MonoBehaviour
    {
        public Canvas canvas;
        public Aerodynamics aerodynamics;
        public float maxDistance = 50;
        private void FixedUpdate()
        {
            canvas.worldCamera = Camera.main;
            //Calculate Forward Velocity
            Vector3 forwardVelocityDirection = aerodynamics.RelativeVelocity;
      
 

            //Calculate world position based on forward velocity, max reticle distance and upward velocity
            Vector3 worldPosition = aerodynamics.transform.rotation *aerodynamics.transform.InverseTransformDirection (forwardVelocityDirection.normalized) * maxDistance;
         
            
            transform.localPosition = worldPosition;

        }


    }
}

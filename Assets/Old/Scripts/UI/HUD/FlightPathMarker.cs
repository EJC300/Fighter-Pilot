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


            Quaternion forwardRotation =  Quaternion.FromToRotation(aerodynamics.transform.forward, canvas.worldCamera.transform.forward);
            //Calculate world position based on forward velocity, max reticle distance and upward velocity
            Vector3 velocity = aerodynamics.transform.InverseTransformDirection(forwardVelocityDirection.normalized) * maxDistance;
            Vector3 worldPosition =   Camera.main.WorldToScreenPoint(aerodynamics.transform.position + aerodynamics.transform.rotation * aerodynamics.transform.forward);
            Vector3 viewportPos = Camera.main.WorldToScreenPoint(worldPosition);

           




            // 5. Clamp the positions to the screen boundaries
            viewportPos.x = Mathf.Clamp(viewportPos.x, -Screen.width, Screen.width);
            viewportPos.y = Mathf.Clamp(viewportPos.y, -Screen.height, Screen.height);
            if(Mathf.Abs( viewportPos.z) <45)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
                // 6. Convert clamped viewport position back to screen pixels
                Vector3 screenPos = (viewportPos);
           
            transform.position = worldPosition;

        }


    }
}

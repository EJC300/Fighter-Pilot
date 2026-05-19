using UnityEngine;
namespace Player
{
    public class FreelLookCamera : MonoBehaviour
    {
        public float distance;
        public float camPitchSpeed;
        public float camYawSpeed;
        private Vector2 currentCamLook;
        public Camera freelookCamera;
   

        public void SetActiveFreeLookCamera(bool switchCamera)
        {
            if (switchCamera)
            {
                bool active = !freelookCamera.gameObject.activeInHierarchy;
                freelookCamera. gameObject.SetActive(active);
                return;
            }
        }
        public void ControlFreeLookCamera(bool reset, Vector2 input,Transform jetTransform)
        {
        


            if (reset)
            {
                currentCamLook = Vector3.MoveTowards(currentCamLook, new Vector3(0, 0, 0), camPitchSpeed * camYawSpeed);
            }
            else
            {
                float maxPitch = 90;
                float minPitch = -90;

                currentCamLook.x += camPitchSpeed * input.y;
                currentCamLook.x = Mathf.Clamp(currentCamLook.x, minPitch, maxPitch);
                currentCamLook.y += camYawSpeed * input.x;
            }
       
            Quaternion freelookRotation = ( Quaternion.Euler(currentCamLook));
            Vector3 defaultOffset = jetTransform.rotation * freelookRotation * new Vector3(0f, 0,-distance);
            Vector3 desiredPosition = jetTransform.position + defaultOffset;

            transform.position = desiredPosition;
            transform.rotation= jetTransform.rotation * freelookRotation;

        }
    }

}
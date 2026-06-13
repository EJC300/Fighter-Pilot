using UnityEngine;
using UnityEngine.UIElements;
namespace Player
{
    public class FreelLookCamera : MonoBehaviour
    {
        public float camDistance;
        public float distance;
        private float resetTimer;
        private bool reseting;
        public float camPitchSpeed;
        public float camYawSpeed;
        private Vector2 currentCamLook;
        public Camera freelookCamera;
   
        //DEf need To fix this
        public void SetActiveFreeLookCamera(bool switchCamera)
        {
            if (switchCamera)
            {
                bool active = !freelookCamera.gameObject.activeInHierarchy;
                freelookCamera. gameObject.SetActive(active);
                currentCamLook = Vector2.zero;
                return;
            }
        }
        public void ControlFreeLookCamera(bool reset, Vector2 input,Transform jetTransform)
        {

            distance = Vector3.Distance(currentCamLook, Vector3.zero);


            if (reset)
            {
                reseting = true;
            }



            if (!reseting)
            {

                float maxPitch = 90;
                float minPitch = -90;
                currentCamLook.x += camPitchSpeed * input.y;
                currentCamLook.x = Mathf.Clamp(currentCamLook.x, minPitch, maxPitch);
                currentCamLook.y += camYawSpeed * input.x;
            }
            else if (resetTimer < distance && reseting)
            {
                resetTimer += Time.deltaTime;


                currentCamLook = Vector3.Lerp(currentCamLook, new Vector3(0, 0, 0), Time.deltaTime * camYawSpeed * camPitchSpeed);
            }
            else if(resetTimer > distance)
            {
                distance = 0;
                resetTimer = 0;
                reseting = false;
           
            }
            
            Quaternion freelookRotation = ( Quaternion.Euler(currentCamLook));
            Quaternion rotation = Quaternion.Slerp( transform.rotation, jetTransform.rotation * freelookRotation, Time.deltaTime * camYawSpeed * camPitchSpeed);
            Vector3 defaultOffset = rotation * new Vector3(0f, 0,-camDistance);
            Vector3 desiredPosition = jetTransform.position + defaultOffset;

            transform.position = desiredPosition;
            transform.rotation = rotation;

        }
    }

}
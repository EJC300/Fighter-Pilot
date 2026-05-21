using UnityEngine;
namespace Player
{
    public class FirstPersonHUDCamera : MonoBehaviour
    {
        //Parented To Camera Rig
        
        public float camPitchSpeed;
        public float camYawSpeed;
        private Vector2 currentCamLook;
        private bool reseting;
        private float distance;
        private float resetTimer;
        public Camera hudCamera;

        private void Start()
        {
            currentCamLook = transform.localEulerAngles;
        }
        public void SetActiveHudCamera(bool switchCamera)
        {
            if (switchCamera)
            {
                bool active = !hudCamera.gameObject.activeInHierarchy;
                hudCamera.gameObject.SetActive(active);
                currentCamLook = Vector2.zero;
                return;
            }
        }
        public void ControlHUDCamera(bool reset,Vector2 input)
        {

            distance = Vector3.Distance(currentCamLook,Vector3.zero);


        

           

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
               
                resetTimer = 0;
                reseting = false;
            }
            Quaternion HudLookRotation = Quaternion.Euler(currentCamLook);
            transform.localRotation= Quaternion.Slerp(transform.localRotation, HudLookRotation,Time.deltaTime * camPitchSpeed * camYawSpeed);
        }

    }
}

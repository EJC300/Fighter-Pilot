using UnityEngine;
namespace Player
{
    public class FirstPersonHUDCamera : MonoBehaviour
    {
        //Parented To Camera Rig
        
        public float camPitchSpeed;
        public float camYawSpeed;
        private Vector2 currentCamLook;
        public Camera hudCamera;


        public void SetActiveHudCamera(bool switchCamera)
        {
            if (switchCamera)
            {
                bool active = !hudCamera.gameObject.activeInHierarchy;
                hudCamera.gameObject.SetActive(active);
                return;
            }
        }
        public void ControlHUDCamera(bool reset,Vector2 input)
        {
        
           

            if(reset)
            {
                currentCamLook = Vector3.MoveTowards(currentCamLook,new Vector3(0,0,0),camPitchSpeed * camYawSpeed);
            }
            else
            {
                float maxPitch = 90;
                float minPitch = -90;
                currentCamLook.x += camPitchSpeed * input.y;
                currentCamLook.x = Mathf.Clamp(currentCamLook.x, minPitch, maxPitch);
                currentCamLook.y += camYawSpeed * input.x;
            }
                Quaternion HudLookRotation = Quaternion.Euler(currentCamLook);
            transform.localRotation= Quaternion.Slerp(transform.localRotation, HudLookRotation,Time.deltaTime * camPitchSpeed * camYawSpeed);
        }

    }
}

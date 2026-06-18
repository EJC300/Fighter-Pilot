using UnityEngine;
using UnityEngine.InputSystem;
namespace JetComponents
{
    public class JetPlayerCamera : MonoBehaviour
    {
        /*
         * This is a camera that gets added to the jet player on spawn.
         * Two camera modes exist 3rd person and first person.
         * The third person camera is a basic orbit camera that is controlled by the camera.
         * First person camera is just camera that is a HUD camera no jet is shown.
         * 
         */
        public float camDistance;
       private InputSystem_Actions player;
        public float camPitchSpeed;
        public float camYawSpeed;
        public Transform jetTransform;
        int cameraMode = 0;
        int prevCameraMode = 0;
        public Camera playerCamera;
        private Vector3 cameraLook;
        private int isChanging;
        private bool reset;
        private float resetTimer;
        private bool resetting;
        private bool switchCamera;
        
        private Vector2 input;
        private void OnEnable()
        {
            player = new InputSystem_Actions();
            playerCamera = Camera.main;
            player.Enable();
        
        }
        private void OnDisable()
        {
            player.Disable();
        }
        private void Update()
        {
            var playerControls = player.Player;
            playerCamera.enabled = isChanging == 0;
            input = playerControls.PitchYaw.ReadValue<Vector2>();
            Debug.Log(input);
            reset = playerControls.ResetCamera.ReadValue<float>() > 0.0f;
            switchCamera = playerControls.SwitchCamera.ReadValue<float>() > 0.5f;
            SwitchCamera();
            ThirdPersonCamera();
            FirstPersonCamera();

        }
        void SwitchCamera()
        {
            if (switchCamera)
            {
            


         
               
                if (cameraMode != 1)
                {
                    cameraMode = 1;
                }
                else if (cameraMode != 0)
                {
                    cameraMode = 0;
                }
            }
           
        }

        void ThirdPersonCamera()
        {
            if(cameraMode == 1)
            {
              var  distance = Vector3.Distance(cameraLook, Vector3.zero);


                if (reset)
                {
                    resetting = true;
                }



                if (!resetting)
                {

                    float maxPitch = 90;
                    float minPitch = -90;
                    cameraLook.x += camPitchSpeed * input.y;
                    cameraLook.x = Mathf.Clamp(cameraLook.x, minPitch, maxPitch);
                    cameraLook.y += camYawSpeed * input.x;
                }
                else if (resetTimer < distance && resetting)
                {
                    resetTimer += Time.deltaTime;


                    cameraLook = Vector3.Lerp(cameraLook, new Vector3(0, 0, 0), Time.deltaTime * camYawSpeed * camPitchSpeed);
                }
                else if (resetTimer > distance)
                {
                    distance = 0;
                    resetTimer = 0;
                    resetting = false;

                }

                Quaternion freelookRotation = (Quaternion.Euler(cameraLook));
                Quaternion rotation = Quaternion.Slerp(playerCamera.transform.rotation, jetTransform.rotation * freelookRotation, Time.deltaTime * camYawSpeed * camPitchSpeed);
                Vector3 defaultOffset = rotation * new Vector3(0f, 0, -camDistance);
                Vector3 desiredPosition = jetTransform.position + defaultOffset;

                playerCamera.transform.position = desiredPosition;
                playerCamera.transform.rotation = rotation;
            }
        }
        void FirstPersonCamera()
        {
            if(cameraMode == 0)
            {
               var distance = Vector3.Distance(cameraLook, Vector3.zero);


                playerCamera.transform.parent = jetTransform;
                playerCamera.transform.localPosition = Vector3.zero;



                if (reset)
                {
                    resetting = true;
                }



                if (!resetting)
                {

                    float maxPitch = 90;
                    float minPitch = -90;
                    cameraLook.x += camPitchSpeed * input.y;
                    cameraLook.x = Mathf.Clamp(cameraLook.x, minPitch, maxPitch);
                    cameraLook.y += camYawSpeed * input.x;
                }
                else if (resetTimer < distance && resetting)
                {
                    resetTimer += Time.deltaTime;


                    cameraLook = Vector3.Lerp(cameraLook, new Vector3(0, 0, 0), Time.deltaTime * camYawSpeed * camPitchSpeed);
                }
                else if (resetTimer > distance)
                {

                    resetTimer = 0;
                    resetting = false;
                }
                Quaternion HudLookRotation = Quaternion.Euler(cameraLook);
               playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, HudLookRotation, Time.deltaTime * camPitchSpeed * camYawSpeed);
            }
        }
    }
}


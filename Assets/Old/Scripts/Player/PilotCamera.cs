
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PilotCamera : MonoBehaviour
    {
        public Transform jetTransform;

        //Later have a master script that helps with New Input System
        public FreelLookCamera freelookCam;
        public FirstPersonHUDCamera hudCam;
        public InputActionAsset inputActions;
        private InputActionMap inputMap;
        private InputAction jetCameraInput;
        private InputAction jetCamSwitch;
        private InputAction camReset;
        private bool previousJetCamSwitch;
        
        public void OnEnable()
        {

            inputMap = inputActions.FindActionMap("Player");

            camReset = inputMap.FindAction("ResetCamera");

            jetCamSwitch = inputMap.FindAction("SwitchCamera");

            jetCameraInput = inputMap.FindAction("PitchYaw");


        }

        private void Update()
        {
 
        }


        private void LateUpdate()
        {
            bool switchCamera = jetCamSwitch.ReadValue<float>() > 0.5;
            hudCam.SetActiveHudCamera(switchCamera);
            previousJetCamSwitch = switchCamera;
            Vector2 jetCamInputAxi = jetCameraInput.ReadValue<Vector2>();
            bool reset = camReset.ReadValue<float>() > 0;
            hudCam.ControlHUDCamera(reset, jetCamInputAxi);
            freelookCam.ControlFreeLookCamera(reset, jetCamInputAxi,jetTransform);
        }


    }
}

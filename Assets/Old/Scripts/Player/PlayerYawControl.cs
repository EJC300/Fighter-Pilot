using Plane;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerYawControl : MonoBehaviour
    {

        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
    
        public VerticalStabilizer playerRudder;

        public string inputAxisName;
        public InputActionAsset inputActions;
        private InputActionMap inputMap;
        private InputAction inputAction;
        Vector3 previousVelocity;
        private float input;
        public void OnEnable()
        {

            inputMap = inputActions.FindActionMap("Player");




            inputAction = inputMap.FindAction(inputAxisName);
          

        }

        public void OnDisable()
        {
          
        }
        private void FixedUpdate()
        {

            ApplyControlAxis();
        }

        private void ApplyControlAxis()
        {
            if (rb == null || inputMap == null || inputAction == null) return;


            Vector3 relativePlaneVelocity = transform.InverseTransformDirection(rb.linearVelocity);

            float angularRate = Vector3.Dot(Vector3.up, transform.InverseTransformDirection(rb.angularVelocity));
            float pitchLimiter = MathHelpers.CalculateEnergyLimit(rb, relativePlaneVelocity, ref previousVelocity);


            input = inputAction.ReadValue<float>();
            input = Mathf.Clamp(input, -pitchLimiter, pitchLimiter);

            

            playerRudder.ApplyVerticalSabilizer(input, relativePlaneVelocity, angularRate);


            previousVelocity = relativePlaneVelocity;

        }
    }
}

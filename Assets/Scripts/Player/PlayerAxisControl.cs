using Plane;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAxisControl : MonoBehaviour
    {

        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public float controlRate;
        public Vector3 axis;
        public ControlSurface controlSurface;
        public PIDController axisGovernor;
        public string inputAxisName;
        public InputActionAsset inputActions;
        private InputActionMap inputMap;
        private InputAction inputAction;
        private InputAction inputSecondAction;
        Vector3 previousVelocity;
        private float yawInput;
        private float input;
        public void OnEnable()
        {

            inputMap = inputActions.FindActionMap("Player");



            inputSecondAction = inputMap.FindAction("Yaw");
            inputAction = inputMap.FindAction(inputAxisName);
            inputMap.Enable();

        }

        public void OnDisable()
        {
            inputMap.Disable();
        }
        private void FixedUpdate()
        {
            input = inputSecondAction.ReadValue<float>();
            ApplyControlAxis();
        }

        private void ApplyControlAxis()
        {
            if (rb == null || inputMap == null || inputAction == null) return;
            
         
            Vector3 relativePlaneVelocity = transform.InverseTransformDirection(rb.linearVelocity);
          
            float angularRate = Vector3.Dot(axis, transform.InverseTransformDirection(rb.angularVelocity));
            float pitchLimiter = MathHelpers.CalculateEnergyLimit(rb, relativePlaneVelocity, ref previousVelocity);
            bool isPitch = axis == Vector3.right;
            bool isRoll = axis  == Vector3.forward;
            bool isYaw = axis == Vector3.up;

            if (isPitch)
            {
                Vector2 rollPitch = inputAction.ReadValue<Vector2>();
                input = rollPitch.y;
                input = Mathf.Clamp(input, -pitchLimiter, pitchLimiter);
            }
            else if(isRoll) 
            {

                Vector2 rollPitch = inputAction.ReadValue<Vector2>();

                input = -rollPitch.x;
            }
            else if(isYaw)
            {
                Debug.Log(isYaw);

                yawInput = inputSecondAction.ReadValue<float>();
            }

                float authority = controlSurface.CalculateControlAuthority(input + yawInput, relativePlaneVelocity, controlRate, angularRate) * controlRate;



            float result = axisGovernor.CalculateResult(Time.fixedDeltaTime, authority, angularRate) ;
          
            MathHelpers.TorqueByRate(result, axis, rb);
            
            previousVelocity = relativePlaneVelocity;
        }
    }
}
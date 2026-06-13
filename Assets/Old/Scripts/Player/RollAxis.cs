using Plane;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerRollControl : MonoBehaviour
    {

        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
    
        public Ailerons playerAilerons;

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
            inputMap.Enable();

        }

        public void OnDisable()
        {
            inputMap.Disable();
        }
        private void FixedUpdate()
        {

            ApplyControlAxis();
        }

        private void ApplyControlAxis()
        {
            if (rb == null || inputMap == null || inputAction == null) return;


            Vector3 relativePlaneVelocity = transform.InverseTransformDirection(rb.linearVelocity);

            float angularRate = Vector3.Dot(Vector3.forward, transform.InverseTransformDirection(rb.angularVelocity));
            float pitchLimiter = MathHelpers.CalculateEnergyLimit(rb, relativePlaneVelocity, ref previousVelocity);


            Vector2 rollPitch = inputAction.ReadValue<Vector2>();
            input = -rollPitch.x;
  



            playerAilerons.ApplyAilerons(input, relativePlaneVelocity, angularRate);


            previousVelocity = relativePlaneVelocity;

        }
    }
}

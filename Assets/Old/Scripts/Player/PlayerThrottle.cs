using Plane;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;
namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerThrottle : MonoBehaviour
    {
        private Rigidbody rb { get { return GetComponent<Rigidbody>(); } }
        public InputActionAsset inputActions;
        private InputActionMap inputMap;
        private InputAction inputAction;
        public string throttleAxisName;
        public Throttle throttle;


    


         public void OnEnable()
        {
            if (inputMap == null)
                inputMap = inputActions.FindActionMap("Player");
            inputAction = inputMap.FindAction(throttleAxisName);

            inputMap.Enable();
        }

        public void OnDisable()
        {
            inputMap.Disable();
        }
        private void FixedUpdate()
        {
            ApplyThrottle();
        }
        public void ApplyThrottle()
        {
            if (rb == null || inputMap == null || inputAction == null) return;
            float throttleInput = inputAction.ReadValue<float>();
           
            throttle.ApplyThrottle(throttleInput);
        }
    }
}

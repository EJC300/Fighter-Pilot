using UnityEngine;
using UnityEngine.InputSystem;
namespace Player
{
    public class PlayerWeapons : MonoBehaviour
    {
        public GunCannon cannon;
        public InputActionAsset inputActions;
        private InputActionMap inputMap;
        private InputAction inputAction;
        public string inputAxisName;
        
        public void OnEnable()
        {
            inputMap = inputActions.FindActionMap("Player");
            inputAction = inputMap.FindAction(inputAxisName);
        }

        private void Update()
        {
            bool fireGun = inputAction.ReadValue<float>() > 0;
            if (fireGun)
            {
                cannon.FireGuns();
            }
        }
    }
}
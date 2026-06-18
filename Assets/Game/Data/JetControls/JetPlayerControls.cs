using UnityEngine;
using UnityEngine.InputSystem;
namespace Behavior
{
    public class JetPlayerControls : JetBrain
    {
        public InputSystem_Actions playerActions;

        public override void ControlJetFlightSurfaces()
        {
            var player = playerActions.Player;

            jetEntity.ControlJet(player.PitchRoll.ReadValue<Vector2>(), player.Yaw.ReadValue<float>(), player.Throttle.ReadValue<float>());
        }

     
        public override void SelectWeapon()
        {
            base.SelectWeapon();
        }

        public override void FireSelectedWeapons()
        {
            base.FireSelectedWeapons();
        }

        

    }
}
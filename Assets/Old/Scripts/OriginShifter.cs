using UnityEngine;
using Model;
namespace Plane
{
    public class OriginShifter : MonoBehaviour
    {
        // -------------------------------------------------------------------------
        // Settings
        // -------------------------------------------------------------------------

        [Header("Settings")]
        public float shiftThreshold = 1000f;
        private Transform player;

        // -------------------------------------------------------------------------
        // State
        // -------------------------------------------------------------------------

        [HideInInspector] public Vector3 worldOffset = Vector3.zero;

        // -------------------------------------------------------------------------
        // Lifecycle
        // -------------------------------------------------------------------------

        private void Start()
        {
            player = LevelRegistry.instance.Player.transform;
        }
        private void LateUpdate()
        {
            if (player != null)
            {
                player = LevelRegistry.instance.Player.transform;
            }
            if (player == null) return;
        }
        private void FixedUpdate()
        {
            if (player == null) return;

            if (player.position.magnitude > shiftThreshold)
                ShiftOrigin();
        }

        // -------------------------------------------------------------------------
        // Origin Shift
        // -------------------------------------------------------------------------

        void ShiftOrigin()
        {
            Vector3 shift = player.position;

            // Shift all root objects except the player
            foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (obj.transform == player) continue;
                obj.transform.position -= shift;
            }

            // Track cumulative world offset for world space calculations
            worldOffset += shift;

            // Reset player to origin
            player.position = Vector3.zero;

            Debug.Log($"Origin shifted by {shift} | Total offset: {worldOffset}");
        }

        // -------------------------------------------------------------------------
        // Utility
        // -------------------------------------------------------------------------

        /// <summary>
        /// Convert a world position back to true world space accounting for origin shifts
        /// </summary>
        public Vector3 ToTrueWorldPosition(Vector3 localWorldPosition)
        {
            return localWorldPosition + worldOffset;
        }

        /// <summary>
        /// Convert a true world position to current shifted space
        /// </summary>
        public Vector3 ToShiftedPosition(Vector3 trueWorldPosition)
        {
            return trueWorldPosition - worldOffset;
        }
    }
}

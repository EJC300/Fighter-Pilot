using UnityEngine;
using Utilities;
using static UnityEngine.GraphicsBuffer;
namespace Weapons {
    public class Seeker : MonoBehaviour
    {

     

       //Wonder If I could make a list of of projected target vectors that are sorted then chosen based on a heuristic cost?

        public float LineOfSightConstant = 5;

        private Vector3 targetPreviousPosition;
        private Vector3 targetPosDiff;

        public float accelLimit;

        private void Start()
        {
            
        }
        //This is perfect!
        public Vector3 CalculateTargetVector(Vector3 target,Vector3 missileVelocity,float missileSpeed)
        {
            targetPosDiff = target - targetPreviousPosition;


            Vector3 targetVelocity = targetPosDiff / Time.deltaTime;


            Vector3 relativeVelocity = missileVelocity - targetVelocity;

            Vector3 relativePosition = target - transform.position;
            relativePosition.Normalize();
           
          
            Vector3 losRate =  Vector3.Cross( relativePosition, relativeVelocity) / Vector3.Dot(relativePosition, relativePosition);
            float closingVelocity = Vector3.Dot(relativePosition, relativeVelocity);
            Vector3 propAccel = LineOfSightConstant * closingVelocity * Vector3.Cross(losRate,relativePosition);
            propAccel = Vector3.ClampMagnitude(propAccel, accelLimit);
            targetPreviousPosition = target;

            return -propAccel;
          

        
        }
        private void FixedUpdate()
        {

         
        }

    }

}

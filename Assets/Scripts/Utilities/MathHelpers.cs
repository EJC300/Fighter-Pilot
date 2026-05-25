using UnityEngine;
using UnityEngine.Rendering;
namespace Utilities
{
    public class MathHelpers
    {
       public static void TorqueByRate(float rate, Vector3 axis,Rigidbody rb)
        {

            if (Mathf.Abs(rate) < 0) return;
            Vector3 target = rate * axis;
            Vector3 current = Vector3.Dot(axis, rb.transform.InverseTransformDirection(rb.angularVelocity)) * axis;
            Vector3 error = target - current;

            rb.AddRelativeTorque(error * rb.mass);

        }

        public static float WrapDegrees(float pos)
        {
            float degree = pos;
            if(pos > 189)
            {
                degree -= 360;
            }
            else if(pos < -180)
            {
                degree += 360;
            }
            return degree;
        }
        public static float WrapFloat(float pos,float min,float max)
        {
            float value = pos;
            if (pos > min)
            {
                value -= max;
            }
            else if (pos < -min)
            {
                value += min; ;
            }
            return value;
        }
        public static string SetDirectionMark(float pos,ref float value)
        {
            float degree = pos;

            if(degree == 0) 
            {
                return "N";
            }
            else if(degree == 270)
            {
                return "W";
            }
            else if(degree == 180)
            {
                return "S";
            
            }
            else if(degree == 90)
            {
                return "E";
            }
            return pos.ToString();
        }
        
      
        public static float CalculateEnergyLimit(Rigidbody rb,Vector3 relativePlaneVelocity, ref Vector3 previousVelocity)
        {
            float acceleration = relativePlaneVelocity.magnitude;
            float gLoad = rb.angularVelocity.magnitude * relativePlaneVelocity.magnitude / Physics.gravity.magnitude;


            previousVelocity = rb.linearVelocity;
            
            return Mathf.Abs(1 - ((gLoad / 10)) - Mathf.Clamp01((acceleration / 1500)));
        }

        public static float CalculateAngleOfAttack(Vector3 relativeVelocity)
        {
            return Mathf.Atan2(-relativeVelocity.y, relativeVelocity.z);
        }

        public static float CalculateLiftOnWings(float liftCoef,Vector3 relativeVelocity,AnimationCurve aoaCurve)
        {
            Vector3 velocity = relativeVelocity;

            float aoa = aoaCurve.Evaluate(CalculateAngleOfAttack(relativeVelocity) * Mathf.Rad2Deg);
            float liftAmount = 0.5f * velocity.sqrMagnitude * aoa * liftCoef;




            return liftAmount;
        }

        public static float CalculateInducedDrag(Rigidbody rb,float liftCoef,AnimationCurve inducedDragCurve,float InduncedDragCoef,Vector3 relativePlaneVelocity)
        {

            float inducedDragValue = inducedDragCurve.Evaluate(Mathf.Max(0, relativePlaneVelocity.z));
            float v2 = relativePlaneVelocity.sqrMagnitude;

            return 0.5f * v2 * InduncedDragCoef * inducedDragValue;
        }
        public static void ApplyInducedDragOnWings(Rigidbody rb,float inducedDrag,Vector3 relativeVelocity)
        {

               

            rb.AddRelativeForce(inducedDrag * -relativeVelocity.normalized);


        }

        public static Vector3 GetVectorRelativeFacingDirection(Vector3 axis, Vector3 position )
        {
            Quaternion lookDirection = Quaternion.LookRotation((axis));
            return lookDirection * axis;
        }
    }
}
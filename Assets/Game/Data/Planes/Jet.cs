using UnityEngine;
using Utilities;


namespace Data
{
    public enum ControlType
    {
        Player,
        Bot
    }
   

    [CreateAssetMenu(fileName = "Jet", menuName = "Data/Jet")]
    public class Jet : ScriptableObject
    {
        public ControlType controlType;
        public AnimationCurve aoaCurve;
        public AnimationCurve dragCurve;
        public AnimationCurve inducedDragCurve;
        public string JetModelName;
        public string JetColliderModelName;
        public float totalThrust;
        public float throttleSpeed;
        public float thrustRatio;
        public float currentThrottle;
        public float minSpeed;
        public float stallSpeed;
        public float mass;
        public float pitchRate;
        public float rollRate;
        public float yawRate;
        public float liftCoef;
        public float inducedDragCoef;
        public float dragCoef;
 


   
        
        
    }

}
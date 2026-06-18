using Data;
using JetComponents;
using Model;
using UnityEngine;
namespace Behavior
{
    public class JetEntity : MonoBehaviour
    {

        private Jet jet;
        public Jet Jet {  get { return jet; } }
        private FlightPhysics flightPhysics;
        private float currentThrottle;
        private Rigidbody rb;
        private JetPlayerCamera jetPlayerCamera;
        //Jet brain
        
        JetBrain jetPilot;
        public void Setup(Jet jet,Vector3 spawnPosition)
        {
            if (flightPhysics == null)
            {
                flightPhysics = new FlightPhysics();
            }
            this.jet = jet;
            rb = gameObject.AddComponent<Rigidbody>();
            var model = Resources.Load<GameObject>(jet.JetModelName);
            var modelToLoad = Instantiate(model,transform);

            this.jet.aoaCurve = jet.aoaCurve;
            this.jet.dragCurve = jet.dragCurve;
            this.jet.inducedDragCurve = jet.inducedDragCurve;
            this.jet.controlType = jet.controlType;
            this.jet.liftCoef = jet.liftCoef;
            this.jet.inducedDragCoef = jet.inducedDragCoef;
            this.jet.dragCoef = jet.dragCoef;
            this.jet.stallSpeed = jet.stallSpeed;
            this.jet.stallSpeed = jet.stallSpeed;
            this.jet.mass = jet.mass;
            this.jet.throttleSpeed = jet.throttleSpeed;
            this.jet.totalThrust = jet.totalThrust;
            this.jet.JetModelName = jet.JetModelName;
            this.jet.pitchRate = jet.pitchRate;
            this.jet.rollRate = jet.rollRate;
            this.jet.yawRate = jet.yawRate;
            //Register Jet to the Registry
            if (Jet.controlType == ControlType.Player)
            {
                LevelRegistry.instance.RegisterPlayer(this);
                //instance the jetPlayerCamera don't worry it will find the player
                
               jetPlayerCamera = Instantiate(this.jet.jetCameraPrefab).GetComponent<JetPlayerCamera>();
               jetPlayerCamera.jetTransform = this.transform;
         
            }
            else
            {
                LevelRegistry.instance.RegisterJet(this);
            }
                transform.position = spawnPosition;
            
        }
        private void FixedUpdate()
        {
            flightPhysics.RelativeVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            flightPhysics.PlaneVelocity = rb.linearVelocity;
            flightPhysics.Drag(jet.dragCurve, rb, jet.dragCoef);
            flightPhysics.InducedDrag(jet.inducedDragCurve, rb, jet.inducedDragCoef);
            flightPhysics.Lift(rb, jet.aoaCurve, jet.liftCoef);
            flightPhysics.YawEffect(gameObject.transform, rb, jet.liftCoef, jet.minSpeed);
        }

        //Jet brain controls this
        public void ControlJet(Vector3 PitchYaw,float Yaw,float throttle)
        {
            
            flightPhysics.ApplyPitch(rb, PitchYaw.y, jet.pitchRate, flightPhysics.RelativeVelocity);
            flightPhysics.ApplyRoll(rb, PitchYaw.x, jet.rollRate, flightPhysics.RelativeVelocity);
            flightPhysics.ApplyYaw(rb, Yaw, jet.yawRate, flightPhysics.RelativeVelocity);
            flightPhysics.ApplyThrottle(rb, throttle, jet.totalThrust, currentThrottle, jet.throttleSpeed, jet.thrustRatio);

        }
    }
}
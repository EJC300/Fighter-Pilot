using System;
using System.Collections.Generic;
using FlaxEngine;

namespace Game;

public class PIDController
{
    public float Kp, Ki, Kd;
    public float integralMin,integralMax;
    public float maxValue,minValue;
     private float lastError;
    private float integral;
    public float currentError;
    
    public float lastTarget;
    float result;
    public float CalculateResult(float dt,float target,float current)
    {
        
        if(Mathf.Abs(target) > lastTarget)
        {
           result = 0;
           integral = 0;
           
        }

        float error = target - current;
        integral += error * dt; 
        integral = Mathf.Clamp(integral,integralMin,integralMax);
        float derivative = (error - lastError) / dt; 

        lastError = error;
        result = Mathf.Clamp((Kp * error) + (Ki * integral) + (Kd * derivative),maxValue,minValue);
        lastTarget = target;
        return result;
    }
    
  
    
}
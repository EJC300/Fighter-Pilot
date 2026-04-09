using UnityEngine;

using UnityEngine;
[System.Serializable]
public class PIDController
{
    public float Kp;
    public float Ki;
    public float Kd;

    public float integralMin;
    public float integralMax;
    public float minValue;
    public float maxValue;

    private float lastError;
    private float integral;
    private float lastTarget;

    public float CalculateResult(float dt, float target, float current)
    {
        // Reset integral on direction change
        if (Mathf.Sign(target) != Mathf.Sign(lastTarget))
            integral = 0f;

        float error = target - current;
        integral = Mathf.Clamp(integral + error * dt, integralMin, integralMax);
        float derivative = dt > 0f ? (error - lastError) / dt : 0f;

        lastError = error;
        lastTarget = target;

        return Mathf.Clamp((Kp * error) + (Ki * integral) + (Kd * derivative), minValue, maxValue);
    }

    public void Reset()
    {
        lastError = 0f;
        integral = 0f;
        lastTarget = 0f;
    }
}

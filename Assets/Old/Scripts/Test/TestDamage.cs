using Plane;
using UnityEngine;

public class TestDamage : MonoBehaviour
{
    public Health testHealth;
    public float damageToApply = 30;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            testHealth.ApplyDamage(damageToApply);
        }
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    //I am not aerodynamically simulated.

 
    
    public float maxSpeed;

    public Rigidbody rb;

    public float minImpactDistance;

    public float maxImpactDistance;

    public float blastRadius;

    public float lifeTime;

    public float maxLiftTime;

    void Fly()
    {
        rb.AddRelativeForce(Vector3.forward * maxSpeed, ForceMode.Acceleration);

        rb.maxLinearVelocity = Mathf.Max(rb.maxLinearVelocity, maxSpeed);
    }

    void ImpactAndDetonate()
    {
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hit, minImpactDistance))
        {

            if (Physics.SphereCast(hit.point, blastRadius, transform.forward, out hit))
            {
                //Apply Damage
                Debug.Log("BOOM");
            }

          
        }
    }
    void DieOnEndOFLife()
    {
        lifeTime += Time.deltaTime * maxSpeed * 0.01f;

        if (lifeTime > maxLiftTime) { Destroy(this.gameObject); }
            
        
    }
    private void FixedUpdate()
    {
        Fly();
    }
    private void Update()
    {
        ImpactAndDetonate();
        DieOnEndOFLife();
    }
}

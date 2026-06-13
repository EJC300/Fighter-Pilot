using UnityEngine;
using Plane;
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
        rb.AddRelativeForce(transform.forward * maxSpeed, ForceMode.Acceleration);

        rb.maxLinearVelocity = Mathf.Max(rb.maxLinearVelocity, maxSpeed);
    }

    void ImpactAndDetonate()
    {
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hit, minImpactDistance))
        {

            if (Physics.SphereCast(hit.point, blastRadius, transform.forward, out hit))
            {
          
                if (hit.collider.transform.GetComponentInChildren<Health>() && hit.collider.transform != this.transform)
                {
                   //Apply Damage
                }
                else if (hit.collider.transform != this.transform)
                {
                    Debug.Log("BOOM");
                }
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

    }
    private void Update()
    {
        Fly();
        ImpactAndDetonate();
        DieOnEndOFLife();
    }
}

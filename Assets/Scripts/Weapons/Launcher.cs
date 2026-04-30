using Plane;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Hardpoint
{
    public bool hasFired;
    public bool canFire;

    public float launchSpeed;

    public Vector3 offset;

    public Transform plane;

    public PlaneController hostPlane;

    public Transform missilePrefab;

    private Missile missile;
    private Rigidbody rb;
    //Handle seeker behavior
    public Transform target;
  //Use object pooling??? Later??
    public void LaunchMissile()
    {
        
        
        if (!hasFired)
        {
            
            Vector3 dir = new Vector3(plane.position.x,plane.position.y,plane.position.z);
            missile = GameObject.Instantiate(missilePrefab, dir, plane.rotation).GetComponent<Missile>();
            
            Rigidbody rbMissile = missile.GetComponent<Rigidbody>();

            
            Vector3 lepTo = Vector3.Lerp(plane.transform.position, plane.transform.position + offset, Time.deltaTime * launchSpeed);
           


            missile.targetTransform = target;
            missile.transform.position = lepTo;
            rbMissile.linearVelocity = rb.linearVelocity;
            hasFired = true;
        }
       

        
       
    }

    public void SetupMissile(GameObject obj)
    {
        rb = obj.GetComponent<Rigidbody>();
    }
}
public class Launcher : MonoBehaviour
{
  public List<Hardpoint> hardPoint = new List<Hardpoint>();
    private int missileCount;
    private void Start()
    {
     
        for (int i = 0; i < hardPoint.Count; i++)
        {
            hardPoint[i].SetupMissile(gameObject);
        }
    }
    public  void FireMissile()
    {
            missileCount = (missileCount + 1) % hardPoint.Count;
            hardPoint[missileCount].LaunchMissile();
        return;
        
    }

  

}

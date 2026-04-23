using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Hardpoint
{
    public bool hasFired;

    public float launchSpeed;

    public Vector3 offset;

    public Transform plane;

    public Transform missilePrefab;

    private Missile missile;
    //Handle seeker behavior
    public Transform target;
    public void LaunchMissile()
    {

        
        if (!hasFired)
        {
            Vector3 dir = -(plane.transform.up + plane.forward) * launchSpeed;
            missile = GameObject.Instantiate(missilePrefab, offset, plane.rotation).GetComponent<Missile>();
            Vector3 lepTo = Vector3.Lerp(plane.transform.position, dir, Time.deltaTime * launchSpeed);
          
            missile.targetTransform = target;
            missile.transform.position = lepTo;
            //hasFired = true; just for fun
        }
       

        
       
    }
}
public class Launcher : MonoBehaviour
{
  public List<Hardpoint> hardPoint = new List<Hardpoint>();
  
  public  void FireMissile()
    {
        for (int i = 0; i < hardPoint.Count; i++)
        {
            hardPoint[i].LaunchMissile();
        }
    }


}

using UnityEngine;

public class GunCannon : MonoBehaviour
{
    public float fireRate;

    public float bulletsPerSecond;

    public Transform bulletPrefab;

    public Transform muzzleOffset;

    private float nextFire;
    private void Start()
    {
        fireRate = 3600f/ bulletsPerSecond;
    }
     public  void FireGuns()
    {
        Debug.Log("fire");

        if (Time.time >= nextFire)
        {       
                
          
                nextFire = Time.time + fireRate;
               Instantiate(bulletPrefab,muzzleOffset.position,transform.rotation);
        }






    }
}

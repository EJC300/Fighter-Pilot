using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EntityTargeting : MonoBehaviour
{

    public static EntityTargeting instance;

    public List<Transform> targets = new List<Transform>();

    public float maxAngle;

    public int index;

    public Transform target;
    public Target previousTarget;
    private void Start()
    {
        instance = this;
    }
    public void AddToTargets(Transform targetable)
    {
        if(targets.Contains(targetable)) return;
            targets.Add(targetable);
        
    }
    public void RemoveFromTargets(Transform target)
    {
      
        targets.Remove(target);
    }

    public void SelectTarget(Transform player)
    {
        target = targets[index];

        float angle = Vector3.Angle(player.transform.forward, (target.position - player.transform.position).normalized);
        if (previousTarget)
        {
            previousTarget.SetTargeted(false);

        }
        if (angle < maxAngle)
        {
            CycleTargets();
            target.GetComponent<Target>().SetTargeted(true);


          


        }
      previousTarget = target.GetComponent<Target>();
    }
    void CycleTargets()
    {
        index = (index + 1) % targets.Count;
       
    }
 }


  




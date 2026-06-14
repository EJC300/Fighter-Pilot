using System.Collections.Generic;
using UnityEngine;
namespace Model
{
    public class JetFactoryMaster : MonoBehaviour
    {
     private Dictionary<string,JetFactory> spawns;

        public static JetFactoryMaster instanace;


        public void Start()
        {
            instanace = this;
        }
        public void RegisterSpawnPoint(string name)
     {
            if(!spawns.TryGetValue(name,out JetFactory value))
            {
                spawns.Add(name,value);
            }
           
     }

        public void DeRegisterSpawnPoint(string name)
        {
            if (spawns.TryGetValue(name, out JetFactory value))
            {
               spawns.Remove(name);
            }

        }

        public JetFactory GetSpawn(string name)
        {
           if(spawns.TryGetValue(name,out JetFactory value))
            {
                return value;
            }
            return null;
          
                 
        }

    }
}

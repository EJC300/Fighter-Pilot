using System.Collections.Generic;
using UnityEngine;
namespace Model
{
    public class JetFactoryMaster : MonoBehaviour
    {
     private Dictionary<string,JetFactory> spawns = new Dictionary<string, JetFactory>();

        public static JetFactoryMaster instanace;


        public void Awake()
        {
            instanace = this;
        }
        public void RegisterSpawnPoint(string name,JetFactory jetFactory)
     {
            if(!spawns.TryGetValue(name,out JetFactory value))
            {
                Debug.Log(name);
                spawns.Add(name,jetFactory);
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

using Behavior;
using Data;
using UnityEngine;
namespace Model
{
    public class JetFactory : MonoBehaviour
    {
       [SerializeField] Jet jet;
        [SerializeField] string factoryName;
        //Maybe the jet should have a unique ID so it can be sorted? But probably not there is a max of 10 jets per level
        //Factories create jets then spawn them the jet then registers with the level registry

        public void Start()
        {
            JetFactoryMaster.instanace.RegisterSpawnPoint(factoryName,this);
        }
        public void OnDisable()
        {
            JetFactoryMaster.instanace.DeRegisterSpawnPoint(factoryName);
        }
        public void CreateJet(Vector3 spawnPosition)
        {
            GameObject product = new GameObject();
            //If jet is a player then add player controller if it's a jet then add jet ai controller
            JetEntity jetEntity = product.AddComponent<JetEntity>();
            if (jet.controlType == ControlType.Player)
            {
                //add player
                LevelRegistry.instance.RegisterPlayer(jetEntity);
            }
            else if (jet.controlType == ControlType.Bot)
            {
                //Add bot
                LevelRegistry.instance.RegisterJet(jetEntity);
            }
           
            jetEntity.Setup(jet, spawnPosition);
            GameObject createdJet = Instantiate(jetEntity.gameObject);
          
          
        }
        
    }
}

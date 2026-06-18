using UnityEngine;
using Model;
public class LevelController : MonoBehaviour
{

    [SerializeField] JetFactoryMaster factoryMaster;
    private bool hasPlayerSpawned;
    [SerializeField] JetFactory playerSpawn;
    private void Awake()
    {
      
    }
    private void Start()
    {
   
        factoryMaster = JetFactoryMaster.instanace;
        SetupPlayerSpawnPoint();
    }
    private void Update()
    {
        SpawnJetAtLevelStart();
    }
    void SetupPlayerSpawnPoint()
    {
      playerSpawn =  factoryMaster.GetSpawn("PlayerSpawnFactory");
           
    }
    void SpawnJetAtLevelStart()
    {
        if (!hasPlayerSpawned)
        {
            playerSpawn.CreateJet(transform.position);
            hasPlayerSpawned = true;
        }
    }

    //spawn ai jet waves

   
}

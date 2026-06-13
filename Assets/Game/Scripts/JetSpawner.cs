using UnityEngine;
using Data;
public class JetSpawner : MonoBehaviour
{
    [SerializeField] Jet jet;

    private void Start()
    {
    
        jet.CreateJetAsset(transform.position, transform.rotation);
         
    }
 
}

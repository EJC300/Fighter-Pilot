using UnityEngine;
using System.Collections.Generic;
using Behavior;
public class LevelRegistry : MonoBehaviour
{
    private List<JetEntity> jets = new();
    private JetEntity player;
    public static LevelRegistry instance;
    
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void OnDestroy()
    {
        jets.Clear();
        player = null;
    }
    public void RegisterJet(JetEntity jet)
    {
        if(!jets.Contains(jet))
        {
            
            jets.Add(jet);
        }
    }
    public void RegisterPlayer(JetEntity jet)
    {
        if (player != null)
        {
            player = jet;
        }
    }
    public void DeRegisterPlayer(JetEntity jet)
    {
       if(player == jet)
        {
            player = null;
        }
    }
    public void DeRegisterJet(JetEntity jet)
    {
        if (jets.Contains(jet))
        {
            jets.Remove(jet);
        }
    }


}

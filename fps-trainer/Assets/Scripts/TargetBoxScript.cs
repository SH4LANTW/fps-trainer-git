using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBoxScript : MonoBehaviour, IShotable
{
    private GameManager gameManagerScript;
    
    private void Start(){
        gameManagerScript = FindObjectOfType<GameManager>();
    }
    
    public void GotShot(ulong shooterClientId){
        
        gameManagerScript.TargetBoxHitAdd(this.gameObject, shooterClientId);
    }

}

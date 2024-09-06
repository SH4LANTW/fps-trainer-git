using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArAnimatorAr : MonoBehaviour
{
    private Animator arAnimAr;
    private void Start()
    {
        arAnimAr = GetComponent<Animator>();
    }
    public void ArMoveBlendAr(float speedBlend){
        arAnimAr.SetFloat("MoveSpeed_ar", speedBlend);
    }

    public void ArFireAr()
    {
        arAnimAr.SetTrigger("FireTriggerAr");
        
    }
    public void ArAimFireAr()
    {
        arAnimAr.SetTrigger("AimFireTriggerAr");
    }
    public void ArReloadAr()
    {
        arAnimAr.SetTrigger("ReloadTriggerAr");
    }
    public void ArAimAr()
    {
        arAnimAr.SetBool("AimTriggerAr", true);
    }
    public void ArUnAimAr()
    {
        arAnimAr.SetBool("AimTriggerAr", false);
    }
    public void ArHolsterAr()
    {
        arAnimAr.SetTrigger("HolsterTriggerAr");
    }
    public void ArEquipAr()
    {
        arAnimAr.SetTrigger("EquipTriggerAr");
    }
}

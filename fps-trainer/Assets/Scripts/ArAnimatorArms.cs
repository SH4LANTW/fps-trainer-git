using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArAnimatorArms : MonoBehaviour
{
    private Animator arAnimArms;
    private void Start()
    {
        arAnimArms = GetComponent<Animator>();
    }

    public void ArMoveBlendArms(float speedBlend){
        arAnimArms.SetFloat("MoveSpeed_arms", speedBlend);
    }
    public void ArFireArms()
    {
        arAnimArms.SetTrigger("FireTriggerArms");
    }
    public void ArAimFireArms()
    {
        arAnimArms.SetTrigger("AimFireTriggerArms");
    }
    public void ArReloadArms()
    {
        arAnimArms.SetTrigger("ReloadTriggerArms");
    }
    public void ArAimArms()
    {
        arAnimArms.SetBool("AimTriggerArms", true);
    }
    public void ArUnAimArms()
    {
        arAnimArms.SetBool("AimTriggerArms", false);
    }
    public void ArHolsterArms()
    {
        arAnimArms.SetTrigger("HolsterTriggerArms");
    }
    public void ArEquipArms()
    {
        arAnimArms.SetTrigger("EquipTriggerArms");
    }
}

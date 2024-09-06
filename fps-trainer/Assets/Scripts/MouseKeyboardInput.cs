using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseKeyboardInput : MonoBehaviour
{
    public ArAnimatorArms arAnimArmsScript;
    public ArAnimatorAr arAnimArScript;
    public Camera fpsCam;

    public bool isAiming = false;
    public float aimingTimeIndex = 0;
    public float adsTime = 0.5f;
    public float hipFireFOV = 75f;
    public float adsFOV = 45f;
    public float currentFOV;

    void Update()
    {
        // InputCheck();
        // AimingCheck();
        // currentFOV = fpsCam.fieldOfView;
    }

    private void InputCheck()
    {
        if (Input.GetMouseButtonDown(1)) //ADS_AR
        {
            arAnimArmsScript.ArAimArms();
            arAnimArScript.ArAimAr();

            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1)) //Un_ADS_AR
        {
            arAnimArmsScript.ArUnAimArms();
            arAnimArScript.ArUnAimAr();

            isAiming = false;
        }


        if (!isAiming && Input.GetMouseButtonDown(0)) //Fire_AR
        {
            arAnimArmsScript.ArFireArms();
            arAnimArScript.ArFireAr();
        }
        
        else if(isAiming && Input.GetMouseButtonDown(0))
        {
            arAnimArmsScript.ArAimFireArms();
            arAnimArScript.ArAimFireAr();
        }
        
        
        if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(2)) //Reload
        {
            arAnimArmsScript.ArReloadArms();
            arAnimArScript.ArReloadAr();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            arAnimArmsScript.ArHolsterArms();
            arAnimArScript.ArHolsterAr();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            arAnimArmsScript.ArEquipArms();
            arAnimArScript.ArEquipAr();
        }
    }
    private void AimingCheck()
    {
        if (isAiming)
        {
            if(aimingTimeIndex < 1)
            {
                aimingTimeIndex += 1 / adsTime * Time.deltaTime;
                aimingTimeIndex = Mathf.Clamp(aimingTimeIndex, 0, 1);
                fpsCam.fieldOfView = Mathf.Lerp(hipFireFOV, adsFOV, aimingTimeIndex);
            }
        }
        else
        {
            if (aimingTimeIndex > 0)
            {
                aimingTimeIndex -= 1 / adsTime * Time.deltaTime;
                aimingTimeIndex = Mathf.Clamp(aimingTimeIndex, 0, 1);
                fpsCam.fieldOfView = Mathf.Lerp(hipFireFOV, adsFOV, aimingTimeIndex);
            }
        }
    }
}

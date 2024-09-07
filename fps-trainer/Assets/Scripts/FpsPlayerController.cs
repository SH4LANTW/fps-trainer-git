using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FpsPlayerController : NetworkBehaviour
{
    [Header("InputAction attribute")]
    public Vector2 moveInput;
    private InputAction fpsInput;
    private float lookRotationX;
    private float lookRotationY;
    private Vector2 mouseMovement;

    private Vector3 moveInputDirection;
    private float _moveSpeed;

    [Header("Scripte reference attribute")]
    [SerializeField] private ArAnimatorAr arAnimScript;
    [SerializeField] private ArAnimatorArms armsAnimScript;
    [SerializeField] private WeaponHandle weaponHandleScript;

    [Header("PlayerMovement attributes")]
    [SerializeField] private Transform bodyOrientation;
    [SerializeField] private Transform camOrientation;
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float lookSpeed = 1f;
    [SerializeField] private float sprintSpeedMultiplier = 3f;
    private Rigidbody rb;

    [Header("ADS FOV change attribute")]
    [SerializeField] private float adsTime = 0.2f;
    [SerializeField] private float hipFireFOV = 75f;
    [SerializeField] private float adsFOV = 45f;
    [SerializeField] private Camera fpsCam;
    private float aimingTimeIndex = 0;

    [Header("Animator State Switch attributes")]
    public float currentBlendIndex = 0;
    private bool isAiming = false;
    private bool pressedSprint = false;
    private bool isSprinting = false;
    [SerializeField] private float blendSpeed = 10f;


    [Header("WeaponSway Attributes")]
    [SerializeField] private Transform camRig;
    [SerializeField] private float maxSwayDistance = 0.06f;
    [SerializeField] private float swayMoveScale = 0.01f;
    [SerializeField] private float maxSwayRotation = 5f;
    [SerializeField] private float swayRotScale = 4f;
    private Vector2 swayPos;
    private Vector3 swayRot;
    [SerializeField] private float smoothSway = 10f;
    [SerializeField] private float smoothSwayRot = 10f;


    public override void OnNetworkSpawn()
    {
        if(IsLocalPlayer){
            fpsCam.gameObject.SetActive(true); //only owner can enable this cam

            PlayerInput playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>(); //only owner can have this input
            playerInput.enabled = true;

        }
        if(!IsOwner){ 
            enabled = false; //if it's not the owner running this script then disable script
            return;
        }
    }

    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed) pressedSprint = true;
        else if (context.canceled) pressedSprint = false;
    }
    public void Look(InputAction.CallbackContext context)
    {
        //mouse input actions

        mouseMovement.x = context.ReadValue<Vector2>().x;
        mouseMovement.y = context.ReadValue<Vector2>().y;
        lookRotationX -= mouseMovement.y * lookSpeed * 0.01f;
        lookRotationX = Mathf.Clamp(lookRotationX, -90f, 90f);
        lookRotationY += mouseMovement.x * lookSpeed * 0.01f;
    }
    public void ADS(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            armsAnimScript.ArAimArms();
            arAnimScript.ArAimAr();

            isAiming = true;
        }
        else if (context.canceled)
        {
            armsAnimScript.ArUnAimArms();
            arAnimScript.ArUnAimAr();

            isAiming = false;
        }

    }
    public void Fire(InputAction.CallbackContext context)
    {
        if (context.started && !isAiming)
        {
            armsAnimScript.ArFireArms();
            arAnimScript.ArFireAr();
            weaponHandleScript.ArFireRaycast();
        }
        else if (context.started && isAiming)
        {
            armsAnimScript.ArAimFireArms();
            arAnimScript.ArAimFireAr();
            weaponHandleScript.ArFireRaycast();
        }
        
    }
    public void Reload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            armsAnimScript.ArReloadArms();
            arAnimScript.ArReloadAr();
        }
    }
    public void Holster(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            armsAnimScript.ArHolsterArms();
            arAnimScript.ArHolsterAr();
        }
    }
    public void EquipWeapon1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            armsAnimScript.ArEquipArms();
            arAnimScript.ArEquipAr();
        }
    }


    void Update()
    {
        PlayerMovementControl(); //player movement
        PlayerMovementAnimator(); //player move animator blend tree input
        PlayerActionControl(); //action keys input: reload, equip etc.
        WeaponSway();

    }


    private void PlayerMovementControl()
    {
        camOrientation.rotation = Quaternion.Euler(lookRotationX, lookRotationY, 0); //cam and hand rotation
        bodyOrientation.rotation = Quaternion.Euler(0, lookRotationY, 0); //body rotation
        moveInputDirection = bodyOrientation.forward * moveInput.y + bodyOrientation.right * moveInput.x; //set move direction
        rb.velocity = moveInputDirection * moveSpeed * Time.deltaTime; //send movement to rb
        // rb.AddForce(moveInputDirection * moveSpeed, ForceMode.Force);
        if (isSprinting) rb.velocity *= sprintSpeedMultiplier;
    }
    private void PlayerMovementAnimator()
    {
        isSprinting = false;
        _moveSpeed = Mathf.Sqrt(moveInput.x * moveInput.x + moveInput.y * moveInput.y); //check move speed for animator blend tree
        if (pressedSprint && moveInput.y == 1){
            _moveSpeed += 1;//able to sprint then blend tree param +1 and isSprinting
            isSprinting = true;
        } 
        currentBlendIndex = Mathf.Lerp(currentBlendIndex, _moveSpeed, blendSpeed * Time.deltaTime); //lerp index towards target movespeed(0-1)
        arAnimScript.ArMoveBlendAr(currentBlendIndex); //send to animator blend tree param
        armsAnimScript.ArMoveBlendArms(currentBlendIndex); //send to animator blend tree param
    }
    private void PlayerActionControl()
    {
        //ads fov change
        if (isAiming)
        {
            if (aimingTimeIndex < 1)
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



    private void WeaponSway()
    {
        Vector2 invertLook = mouseMovement * -swayMoveScale * 0.01f;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxSwayDistance, maxSwayDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxSwayDistance, maxSwayDistance);

        swayPos = invertLook;

        Vector2 invertRot = new Vector2(mouseMovement.x * -swayRotScale * 0.01f, mouseMovement.y * swayRotScale * 0.01f);
        invertRot.x = Mathf.Clamp(invertRot.x, -maxSwayRotation, maxSwayRotation);
        invertRot.y = Mathf.Clamp(invertRot.y, -maxSwayRotation, maxSwayRotation);

        swayRot = new Vector3(invertRot.y, invertRot.x, invertRot.x);

        camRig.localPosition = Vector3.Lerp(camRig.localPosition, swayPos, smoothSway * Time.deltaTime);
        camRig.localRotation = Quaternion.Slerp(camRig.localRotation, Quaternion.Euler(swayRot), smoothSwayRot * Time.deltaTime);

    }
}

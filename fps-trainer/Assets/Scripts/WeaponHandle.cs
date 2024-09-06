using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponHandle : NetworkBehaviour
{
    [SerializeField] private float arFireRange = 10f;

    [SerializeField] private Transform rearSightPos;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner){
            enabled = false;
            return;
        }
    }

    public void ArFireRaycast(){
        ArFireRaycast_ServerRPC(rearSightPos.position, transform.forward, arFireRange);
    }
    [ServerRpc]
    private void ArFireRaycast_ServerRPC(Vector3 rearSightPos, Vector3 forwardPos, float arFireRange){
        Ray ray = new Ray(rearSightPos, forwardPos);
        if(Physics.Raycast(ray, out RaycastHit hit, arFireRange)){
            IShotable shotable = hit.collider.GetComponent<IShotable>();
            shotable?.GotShot(this.OwnerClientId);
        }
    }

    void Update(){
        Debug.DrawRay(rearSightPos.position, transform.forward * arFireRange, Color.green);
    }
}
public interface IShotable{
    public void GotShot(ulong shooterId);
}

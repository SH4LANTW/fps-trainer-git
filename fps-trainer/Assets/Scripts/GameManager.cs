using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour
{
    private NetworkVariable<int> net_totalhitCount = new NetworkVariable<int>(0);
    private int playerHitCount = 0;
    [SerializeField] private GameObject pfTargetBox;
    [SerializeField] private Camera startCamara;

    public override void OnNetworkSpawn()
    {
        startCamara.enabled = false;
        Debug.Log(OwnerClientId);
        //net_totalhitCount.OnValueChanged += (int oldValue, int newValue) => { Debug.Log(net_totalhitCount.Value); };
        if (IsServer) //methods only run on server
        {
            StartSpawnBoxTarget();
        }
    }

    public void RegisterPlayerOnSpawn(FpsPlayerUI playerUiScript)
    {
        net_totalhitCount.OnValueChanged += playerUiScript.HitCountUiUpdate;
    }


    public void StartSpawnBoxTarget()
    {
        Vector3 generatePos = new Vector3(Random.Range(-0.5f, 1), Random.Range(0.1f, 1f), 1.865f); //generate new pos
        GameObject clientSpawnBox = Instantiate(pfTargetBox, generatePos, Quaternion.identity); //spawn new box on server
        clientSpawnBox.GetComponent<NetworkObject>().Spawn(true); //spawn the same box on clients

        generatePos = new Vector3(Random.Range(-0.5f, 1), Random.Range(0.1f, 1f), 1.865f);
        clientSpawnBox = Instantiate(pfTargetBox, generatePos, Quaternion.identity);
        clientSpawnBox.GetComponent<NetworkObject>().Spawn(true);

        generatePos = new Vector3(Random.Range(-0.5f, 1), Random.Range(0.1f, 1f), 1.865f);
        clientSpawnBox = Instantiate(pfTargetBox, generatePos, Quaternion.identity);
        clientSpawnBox.GetComponent<NetworkObject>().Spawn(true);
    }

    public void TargetBoxHitAdd(GameObject hittedBox, ulong shooterClientId)
    {
        net_totalhitCount.Value++;
        PlayerTargetBoxHitAdd_ClientRpc(new ClientRpcParams  //only call to the shooter's clientRpc 
        { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { shooterClientId } } });
        Destroy(hittedBox);


        Vector3 generatePos = new Vector3(Random.Range(-0.5f, 1), Random.Range(0.1f, 1f), 1.865f);
        GameObject clientSpawnBox = Instantiate(pfTargetBox, generatePos, Quaternion.identity); //spawn new box on server
        clientSpawnBox.GetComponent<NetworkObject>().Spawn(true); //spawn the same box on clients   
    }

    [ClientRpc] //client player target box hit add
    private void PlayerTargetBoxHitAdd_ClientRpc(ClientRpcParams crp)
    {
        playerHitCount++;
        Debug.Log("Local Player hit Count" + playerHitCount);
    }



}

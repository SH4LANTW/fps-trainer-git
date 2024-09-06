using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;


public class GameManager : NetworkBehaviour
{
    private NetworkVariable<int> totalhitCount = new NetworkVariable<int>(0);
    [SerializeField] private GameObject pfTargetBox;
    public FpsPlayerUI playerUiScript;

    public override void OnNetworkSpawn()
    {
        // if(!IsOwner){ //only enable local owner's script
        //     Debug.Log("turned off not owner's scripts");
        //     enabled = false;
        //     return;
        // }
        playerUiScript = FindObjectOfType<FpsPlayerUI>(); //not correct, need to specify proper player than only find a script
        playerUiScript.InitializePlayerCanvas(IsClient);
        
        totalhitCount.OnValueChanged += (int oldValue, int newValue) => { Debug.Log(totalhitCount.Value); };
        totalhitCount.OnValueChanged += playerUiScript.HitCountUiUpdate;

        if (IsServer) //methods only run on server
        {
            Debug.Log("is Server");
            StartSpawnBoxTarget();
            Debug.Log("Server spawned boxes");
        }

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
        totalhitCount.Value++;

        Destroy(hittedBox);


        Vector3 generatePos = new Vector3(Random.Range(-0.5f, 1), Random.Range(0.1f, 1f), 1.865f);
        GameObject clientSpawnBox = Instantiate(pfTargetBox, generatePos, Quaternion.identity); //spawn new box on server
        clientSpawnBox.GetComponent<NetworkObject>().Spawn(true); //spawn the same box on clients   
    }

}

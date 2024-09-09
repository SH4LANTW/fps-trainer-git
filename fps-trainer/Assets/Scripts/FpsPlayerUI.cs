using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class FpsPlayerUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI hitCountUi;

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            FindObjectOfType<GameManager>().RegisterPlayerOnSpawn(this); //register player object to gamemanager on start up
            Debug.Log("send");
        }
        else{
            InitializePlayerCanvas(); //turn off notLocalPlayer's Canvas
        }
    }



    public void InitializePlayerCanvas()
    {
        transform.gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void HitCountUiUpdate(int oldV, int newV)
    {
        hitCountUi.text = $"Total Hit: {newV}";
    }
}

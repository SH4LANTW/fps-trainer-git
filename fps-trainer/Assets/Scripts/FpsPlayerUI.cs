using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsPlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hitCountUi;

    public void InitializePlayerCanvas(bool isOwner)
    {
        if (isOwner)
        {
            transform.gameObject.GetComponent<Canvas>().enabled = true;
        }
    }

    public void HitCountUiUpdate(int oldV, int newV)
    {
        hitCountUi.text = $"Total Hit: {newV}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreteWin : MonoBehaviour
{
    public GameObject WinnerScreen;
    public bool isWin;

    void Update()
    {
        ActivateWinnerScreen();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            isWin = true;
        }
    }

    private void ActivateWinnerScreen()
    {
        if(isWin == true)
        {
            WinnerScreen.gameObject.SetActive(true);
        }
    }
}

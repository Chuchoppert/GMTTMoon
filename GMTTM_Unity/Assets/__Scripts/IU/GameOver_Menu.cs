using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver_Menu : MonoBehaviour
{
    public GameObject MenuGameOver;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            MenuGameOver.gameObject.SetActive(true);
        }
    }
}

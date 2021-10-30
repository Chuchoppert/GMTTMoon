using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour 
{
    //public GameObject OptionsMenu;

    public void BotonQuit()
    {
        Application.Quit();
    }
    
    public void BotonChangeScene(string escena)
    {
        SceneManager.LoadScene(escena);
    }

   /* public void BotonDeTeclado()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            gameObject.SetActive.
        }

        
    }*/
}
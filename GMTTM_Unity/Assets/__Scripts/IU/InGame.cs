using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour
{
    [Header("Set basics for HUD")]
    public TextMeshProUGUI Text_Time;
    public TextMeshProUGUI Distance_Tx;
    public GameObject Minimap;
    public GameObject Pos0;
    public GameObject Player;
    public GameObject MenuGameOver;
    public GameObject GO_Timer;
    public float Timer = 60f;


    //public GameObject MensajeInitial;
    //public float AmountPerTime;
    //public float SlowTime;


    private float DistanciaRecorrida;

    
    //private bool onceTime;
    //private bool onceTimer;

    private void Start()
    {
        //onceTimer = true;
        //Time.timeScale = 1f;
        //MensajeInitial.gameObject.SetActive(true);
    }
    void Update()
    {
        DistanciaRecorrida = Vector3.Distance(Pos0.transform.position, Player.transform.position);

        if (Player != null && Player.activeSelf == true) //Cronometro
        {
            Timer -= Time.deltaTime;
            Text_Time.text = "Fuel: " + Timer.ToString("F2");

            Distance_Tx.text = DistanciaRecorrida.ToString("F2") + " m";
        }

        if(Timer <= 0)
        {
            MenuGameOver.gameObject.SetActive(true);
            GO_Timer.gameObject.SetActive(false);     
            Minimap.gameObject.SetActive(false);
        }

        /*if (onceTimer == true)
        {
            if (Time.timeScale > 0.1)
            {
                SlowTime -= AmountPerTime;
                Time.timeScale = SlowTime;
            }
        }

        if(Time.timeScale <= 0.1)
        {
            onceTimer = false;
            onceTime = true;
        }

        if(onceTime == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MensajeInitial.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                onceTime = false;
            }
        }*/
    }
}

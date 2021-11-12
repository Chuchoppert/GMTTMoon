using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerMovements : MonoBehaviour
{
    [Header("Look Actions")]
    public bool isFirstResetReady;
    public bool isOnGround = false;
    public bool isSetRotation = false;
    public bool isReadyToLaunch = false;
    public bool isSubtractPowerGraph = false;
    public float PowerGraphAmount;
    public static float MaxPowerGpForAnotherAttempt;
    public float POV = 24;


    [Header("Set for PreLaunch Phase")]
    public Vector3 SetGravity = new Vector3(0, -9.8f, 0);
    public float RotationSpeed = 2;
    public Slider PowerGraph;
    public float Impulse = 10;
    public float PowerAmount = 1;
    public float MinPower = 0;


    [Header("Set for trayectory line")]
    public GameObject ViewRotation;
    public GameObject point;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    public float CorrecionLineas;

    [Header("Look actions")]
    public Vector3 StartPosLr;
    public Vector3 preEndpos;
    public Vector3 ForceDir;
    public GameObject[] points;
    public Camera UpVision_Camera;

    [Header("Config sfx")]
    public AudioSource SoundPlayer;
    public AudioClip[] SoundsBall;
    //Lista de SFX para que no te pierdas
    //0 = 
    //1 =
    //2 =
    //3 =
    //4 =
    //5 =
    //6 =
    //7 =


    private Rigidbody rb;
    private float yMovement;
    private float xMovement;
    private float zMovement;
    private Vector3 heading;
    private float distance;
    private Vector3 OriginalMovementAxis;
    private Quaternion initialRotation;
    private GameObject[] GuiasAEliminar;
    

    void Start()
    {
        Physics.gravity = SetGravity;
        rb = GetComponent<Rigidbody>();
        transform.rotation = initialRotation;
        UpVision_Camera.orthographicSize = POV;
    }


    void Update()
    {

        PowerGraph.value = Mathf.Clamp(PowerAmount, MinPower, MaxPowerGpForAnotherAttempt); //limite de la barra de fuerza 

        if (isOnGround == true) //Si la pelota esta parada entonces...  //cambiar por check ground en OnCollision para resetear las posiciones de la curva y la rotacion  
        {
            if(rb.velocity == Vector3.zero) //resetea la rotacion para el siguiente lanzamiento
            {
                ////////////////////////// AQUI PUEDE HABER UN SONIDO DE INICIO DE LANZAMIENTO ////////////////////////
                ///SoundToPlay(0);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                
                startingAttempt();
                StartPosLr = gameObject.transform.position;
                preEndpos = ViewRotation.transform.position;
                if (Input.GetKeyUp(KeyCode.Space) && isSetRotation == false) //Detiene la rotacion y cualquier movimiento e inicia la barra de fuerza
                {
                    PreLaunchPhase();
                }
                if (isSetRotation == true && isReadyToLaunch == false) //permite el movimiento de la barra si es que esta activada
                {
                    StartPowerGraph();
                }

                if (isSetRotation == false) //crea la fuerza para la rotacion
                {
                    OriginalMovementAxis = new Vector3((Input.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime), (Input.GetAxis("Vertical") * RotationSpeed * Time.deltaTime), 0);
                    StayPhase();
                }
                if (Input.GetKeyDown(KeyCode.Space) && isSetRotation == true) //detiene el movimiento de la barra
                {
                    StopPowerGraph();
                }
            }           

            PowerGraph.value = Math.Abs((PowerGraphAmount-1)); //setea el valor a la barra de poder

            heading = preEndpos - StartPosLr;
            distance = heading.magnitude;
            ForceDir = ( ((heading / distance) * Impulse) * PowerGraphAmount);                
        }
    }

    private void FixedUpdate() 
    {       
        if (points.Length != 0)
        {
            if (points.Length != 0)
            {
                for (int i = 0; i < numberOfPoints; i++) //Guia de lanzamiento part2 (hace de separador de cada punto de la guia)  
                {
                    points[i].transform.position = PointPosition(i * spaceBetweenPoints);
                }
            }
        }        
    }


    Vector3 PointPosition(float t) //Guia de lanzamiento part3 (aqui es donde se "predice" la trayectoria)
    {
        Vector3 position = gameObject.transform.position + (((ForceDir.normalized * Impulse * t) + 0.5f * (Physics.gravity * PowerGraphAmount) * (t * t)) * PowerGraphAmount) * CorrecionLineas;  //NUNCA TOQUEN PLIS, ni yo se como funciono grax :c
        return position;
    }


    public void startingAttempt()
    {
        if (isFirstResetReady == false)
        {
            transform.rotation = initialRotation;


            points = new GameObject[numberOfPoints]; //Guia de lanzamiento part1 (inicia cuantas bolitas deberian aparecer para la guia, colocando la primera desde la pos de la bolita)
            for (int i = 0; i < numberOfPoints; i++)
            {
                points[i] = Instantiate(point, StartPosLr, Quaternion.identity);
            }
        }
        isFirstResetReady = true;
    }


    public void StayPhase() //Permite la "rotacion" de la pelota
    {
        xMovement = OriginalMovementAxis.x;
        yMovement = OriginalMovementAxis.y;
        zMovement = OriginalMovementAxis.z;
        ViewRotation.transform.position = new Vector3((Mathf.Clamp( ViewRotation.transform.position.x, (transform.position.x - 4), (transform.position.x + 4) )), (Mathf.Clamp( ViewRotation.transform.position.y, (transform.position.y - 1), (transform.position.y + 6) )), (transform.position.z + 2)); //limita el movimiento 

        ViewRotation.transform.Translate(new Vector3(xMovement, yMovement, 0), Space.World); //esta es la pos de referencia mas importante del codigo

        ////////////////////////// AQUI PUEDE HABER UN SONIDO DE CUANDO SE MUEVEN LAS GUIAS//////////////////////
        /*if(xMovement != 0 && yMovement != 0) //Si hay movimiento en las teclas WASD entonces se escuchara ese sonido (utiliza un sonido de 1sg o menos, si no se repetira el mismo sonido)
        {
            SoundToPlay(1);
        }*/
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
    }


    public void PreLaunchPhase() //Congela Movimiento de la pelota y activa barra de poder
    {
        isSetRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        PowerGraph.gameObject.SetActive(true);

        ///////////////////// AQUI PUEDE HABER UN SONIDO DE ACTIVACION DE BARRA DE FUERZA /////////////////////
        ///SoundToPlay(2);
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
    }


    public void StartPowerGraph() //permite el movimiento de la fuerza de la barra de poder
    {
        if (isSubtractPowerGraph == false)
        {
            PowerGraphAmount += PowerAmount * Time.deltaTime;
            if (PowerGraphAmount > MaxPowerGpForAnotherAttempt)
            {
                isSubtractPowerGraph = true;
            }
            ///////////////////// AQUI PUEDE HABER UN SONIDO DE SUBIDA DE FUERZA //////////////////////////////////
            ///SoundToPlay(3);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
        }
        else
        {
            PowerGraphAmount -= PowerAmount * Time.deltaTime;
            if (PowerGraphAmount < MinPower)
            {
                isSubtractPowerGraph = false;
            }
            ///////////////////// AQUI PUEDE HABER UN SONIDO DE BAJADA DE FUERZA //////////////////////////////////
            ///SoundToPlay(4);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }


    public void StopPowerGraph()  //quita las restinciones de movimiento y rotacion y prepara el lanzamiento
    {
        rb.constraints = RigidbodyConstraints.None;
        isReadyToLaunch = true;
        Invoke("LaunchPhase", 1);

        GuiasAEliminar = GameObject.FindGameObjectsWithTag("Guias"); //Guia de lanzamiento partFinal (Elimina las guias)
        for (int i = 0; i < numberOfPoints; i++)
        {
            Destroy(GuiasAEliminar[i]);
        }
        points = new GameObject[] { };
    }


    public void LaunchPhase() //oculta la barra e inicia el lanzamiento 
    {
        PowerGraph.gameObject.SetActive(false);
        rb.AddForce( ForceDir, ForceMode.Impulse);
        Invoke("ResetBooleans", 1);

        ///////////////////// AQUI PUEDE HABER UN SONIDO DE LANZAMIENTO ///////////////////////////////////////
        ///SoundToPlay(5);
        ///////////////////////////////////////////////////////////////////////////////////////////////////////
    }


    public void ResetBooleans() //reset a los valores para el siguiente lanzamiento
    {       
        isReadyToLaunch = false;
        isSetRotation = false;
        isFirstResetReady = false;
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            isOnGround = true;

            ///////////////////// AQUI PUEDE HABER UN SONIDO DE CAIDA DE LA PELOTA  ///////////////////////////////
            ///SoundToPlay(6);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }


    private Vector3 respawnOffset = new Vector3(0f, 0.5f, 0f);

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Respawn")
        {
            xMovement = 0;
            zMovement = 0;
            yMovement = 0;
            this.transform.position = StartPosLr + respawnOffset;
            rb.velocity = Vector3.zero;

            //////////////////////////// AQUI PUEDE HABER UN SONIDO DE "RESPAWN" //////////////////////////////////
            ///SoundToPlay(7);
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }

    private void SoundToPlay(int IndexSound)
    {
        ///Primera opcion (si falla o se bugea algo, utiliza la segunda opcion)
        SoundPlayer.clip = SoundsBall[IndexSound];
        SoundPlayer.PlayOneShot(SoundPlayer.clip);


        ///Segunda Opcion
        /*if(IndexSound == 0)
        {
            SoundPlayer.clip = SoundsBall[0];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }

        if (IndexSound == 1)
        {
            SoundPlayer.clip = SoundsBall[1];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }

        if (IndexSound == 2)
        {
            SoundPlayer.clip = SoundsBall[2];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }

        if (IndexSound == 3)
        {
            SoundPlayer.clip = SoundsBall[3];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }*/

        /*if(IndexSound == 4)
        {
            SoundPlayer.clip = SoundsBall[4];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }

        if (IndexSound == 5)
        {
            SoundPlayer.clip = SoundsBall[5];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }

        if (IndexSound == 6)
        {
            SoundPlayer.clip = SoundsBall[6];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }

        if (IndexSound == 7)
        {
            SoundPlayer.clip = SoundsBall[7];
            SoundPlayer.PlayOneShot(SoundPlayer.clip);
        }*/
    }
}

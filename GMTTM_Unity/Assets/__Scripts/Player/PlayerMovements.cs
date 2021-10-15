using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    [Header("Set basics for Movements")]
    public Vector3 SetGravity = new Vector3(0, -9.8f, 0);
    public float RotationSpeed = 2;


    [Header("Look Actions")]
    public bool isFirstResetReady;
    public bool isOnGround = false;
    public bool isSetRotation = false;
    public bool isReadyToLaunch = false;
    public bool isSubtractPowerGraph = false;
    public float PowerGraphAmount;
    public float MaxPowerGpForAnotherAttempt =1;


    [Header("Set for PreLaunch Phase")]
    public Slider PowerGraph;
    public float Impulse = 10;
    public float PowerAmount = 1;
    public float MinPower = 0;
    public float MaxPower = 1;


    [Header("Set for trayectory line")]
    public GameObject ViewRotation;
    public GameObject point;
    public int numberOfPoints;
    public float spaceBetweenPoints;


    [Header("Look actions")]
    public Vector3 StartPosLr;
    public Vector3 preEndpos;
    public Vector3 ForceDir;
    public GameObject[] points;


    private Rigidbody rb;
    private float yMovement;
    private float xMovement;
    private Vector3 heading;
    private float distance;
    private Vector3 OriginalMovementAxis;
    private Quaternion initialRotation;
    private GameObject[] GuiasAEliminar;


    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = SetGravity;
        rb = GetComponent<Rigidbody>();
        transform.rotation = initialRotation;
    }

    // Update is called once per frame
    void Update()
    {
        PowerGraph.value = Mathf.Clamp(PowerAmount, MinPower, (MaxPower * MaxPowerGpForAnotherAttempt)); //limite de la barra de fuerza 

        if (isOnGround == true) //Si la pelota esta parada entonces...  //cambiar por check ground en OnCollision para resetear las posiciones de la curva y la rotacion  
        {  
            if(rb.velocity == Vector3.zero) //resetea la rotacion para el siguiente lanzamiento
            {
                startingAttempt();
                StartPosLr = transform.position;
                preEndpos = ViewRotation.transform.position;
            }           
            //preEndpos = ViewRotation.transform.position + offsetEndPos;

            if (Input.GetKeyUp(KeyCode.Space) && isSetRotation == false) //Detiene la rotacion y cualquier movimiento e inicia la barra de fuerza
            {
                PreLaunchPhase();
            }

            if (isSetRotation == true && isReadyToLaunch == false) //permite el movimiento de la barra si es que esta activada
            {
                StartPowerGraph();                          
            }
            PowerGraph.value = PowerGraphAmount; //setea el valor a la barra de poder

            heading = preEndpos - StartPosLr;
            distance = heading.magnitude;
            ForceDir = ((heading / distance) * Impulse) * PowerGraphAmount;
            //ForceDir = ((preEndpos - StartPosLr) * Impulse) * PowerGraphAmount;                     
        }
    }

    private void FixedUpdate() //Es mejor que update() cuando se trata de fisicas
    {
        //Debug.DrawLine(transform.position, transform.forward * 10, Color.red);
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
        //rb.AddTorque(AxisRotation * yRotation, ForceMode.Force);

        xMovement = OriginalMovementAxis.x;
        yMovement = OriginalMovementAxis.y;
        ViewRotation.transform.position = new Vector3((Mathf.Clamp(ViewRotation.transform.position.x, -4, 4)), (Mathf.Clamp(ViewRotation.transform.position.y, -1, 6)), (transform.position.z + 2)); //limita el movimiento 

        ViewRotation.transform.Translate(new Vector3(xMovement, yMovement, 0), Space.World); //esta es la pos de referencia mas importante del codigo


        //CHECAR QUE FUNCIONE SOLO SI HAY GUIAS EN LA ESCENA 
        for (int i = 0; i < numberOfPoints; i++) //Guia de lanzamiento part2 (hace de separador de cada punto de la guia)  
        {
            points[i].transform.position = PointPosition(i * spaceBetweenPoints);
        }
    }

    public void PreLaunchPhase() //Congela Movimiento de la pelota y activa barra de poder
    {
        isSetRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        PowerGraph.gameObject.SetActive(true);   
    }

    public void StartPowerGraph() //permite el movimiento de la fuerza de la barra de poder
    {
        if (isSubtractPowerGraph == false)
        {
            PowerGraphAmount += PowerAmount * Time.deltaTime;
            if (PowerGraphAmount > MaxPower)
            {
                isSubtractPowerGraph = true;
            }
        }
        else
        {
            PowerGraphAmount -= PowerAmount * Time.deltaTime;
            if (PowerGraphAmount < MinPower)
            {
                isSubtractPowerGraph = false;
            }
        }
    }

    public void StopPowerGraph()  //quita las restinciones de movimiento y rotacion y prepara el lanzamiento
    {
        rb.constraints = RigidbodyConstraints.None;
        isReadyToLaunch = true;
        Invoke("LaunchPhase", 1);

        GuiasAEliminar = GameObject.FindGameObjectsWithTag("Guias");
        for (int i = 0; i < GuiasAEliminar.Length; i++)
        {
            Destroy(GuiasAEliminar[i]);
        }
    }

    public void LaunchPhase() //oculta la barra e inicia el lanzamiento 
    {       
        PowerGraph.gameObject.SetActive(false);
        rb.AddForce( ForceDir, ForceMode.Impulse);
        Invoke("ResetBooleans", 1);
    }

    public void ResetBooleans() //reset a los valores para el siguiente lanzamiento
    {       
        isReadyToLaunch = false;
        isSetRotation = false;
        isFirstResetReady = false;
    }

    Vector3 PointPosition(float t) //Guia de lanzamiento part3 (aqui es donde se "predice" la trayectoria)
    {
        //Vector3 Dir = StartPosLr + preEndpos;      
        Vector3 position = ((Vector3)transform.position + (ForceDir.normalized * Impulse * t) + 0.5f * (Physics.gravity * PowerGraphAmount) * (t * t)) * PowerGraphAmount;  //NUNCA TOQUEN PLIS, ni yo se como funciono grax :c
        return position;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            isOnGround = true;            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //aqui agregar lo de distance entre dos puntos y usar el MaxPowerGpForAnotherAttempt que se usa en la linea 6 para decrementar la fuerza maxima de la barra de poder
        
    }
}

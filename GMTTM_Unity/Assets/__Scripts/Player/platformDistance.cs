using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platformDistance : MonoBehaviour
{
    public float distanceV;
    public float limiter;  
    public float divisor;
    public float LimitadorMax = 0.3f;

    private void Update()
    {
        //distanceV = Mathf.Clamp(distanceV, 0.5f, 1.0f);
        if (limiter < LimitadorMax)
        {
            limiter = LimitadorMax;
        }       
        PlayerMovements.MaxPowerGpForAnotherAttempt = limiter;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            distanceV = Mathf.Abs(((Vector3.Distance(transform.position, collision.contacts[0].point))/divisor)-1);
            limiter = distanceV;
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow_UpVision : MonoBehaviour
{
    [Header("Set Basic for Follow Camera")]
    public GameObject Player;
    public float smoothSpeed = 0.125f;

    private Vector3 SmoothedPosition;

    // Update is called once per frame
    void Update()
    {
        SmoothedPosition = Vector3.Lerp(transform.position, Player.transform.position, smoothSpeed);

        gameObject.transform.position = new Vector3(SmoothedPosition.x, gameObject.transform.position.y, SmoothedPosition.z);
    }
}

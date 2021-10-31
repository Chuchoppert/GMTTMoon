using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	//this script goes to camera
	[Header("Set Basic for Follow Camera")]
	public Transform target; //object to follow
	public float smoothSpeed = 0.125f;
	public Vector3 ViewOffset; //put the position of the camera in the perspective you want
	public Vector3 TargetOffset;


    void FixedUpdate()
	{
		Vector3 desiredPosition = target.position + ViewOffset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

		transform.LookAt((target.position + TargetOffset));
	}

}


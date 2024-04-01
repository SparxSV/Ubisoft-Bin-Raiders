using NaughtyAttributes;

using System;

using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("Target Object")]
	[SerializeField] private Transform targetPos;

	[Header("Camera Position Settings")]
	[SerializeField] private float followDistance = 8f;
	[SerializeField] private float elevationAngle = 6f;
	[SerializeField] private float orbitalAngle = 0f;

	[Header("Camera Smoothing Settings")]
	[SerializeField] private bool isMovementSmoothing = true;
	[SerializeField] private bool isRotationSmoothing = true;
	[SerializeField, EnableIf("isRotationSmoothing")] private float rotationSmoothing;

	private Vector3 desiredPosition;
	private Transform cameraPos;

	private void Start()
	{
		// Get Camera Position
		cameraPos = GetComponent<Transform>();

		// Get Target Position
		if(targetPos == null)
		{
			try
			{
				targetPos = FindObjectOfType<PlayerController>().transform;
			}
			catch(Exception e)
			{
				NullReferenceException nullCharacter = new NullReferenceException("TargetPos is NULL, please supply targetPos with a valid Transform.");
				Console.WriteLine(nullCharacter);

				throw;
			}
		}
	}

	private void FixedUpdate()
	{
		// Check for valid target
		if(targetPos != null)
		{
			desiredPosition = targetPos.position + targetPos.TransformDirection(Quaternion.Euler(elevationAngle, orbitalAngle, 0f) * (new Vector3(0, 0, -followDistance)));

		// Movement Smoothing
		cameraPos.position = isMovementSmoothing ? Vector3.Lerp(cameraPos.position, desiredPosition, Time.deltaTime * 5.0f) : desiredPosition;
			
		// Rotation Smoothing
		if (isRotationSmoothing)
			cameraPos.rotation = Quaternion.Lerp(cameraPos.rotation, Quaternion.LookRotation(targetPos.position - cameraPos.position), rotationSmoothing * Time.deltaTime);
		else
			cameraPos.LookAt(targetPos);
		}
	}
}

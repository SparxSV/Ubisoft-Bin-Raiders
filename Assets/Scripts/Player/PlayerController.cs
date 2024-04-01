using UnityEngine;
using NaughtyAttributes;

using System;

using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlayerController : MonoBehaviour
{
	[Header("Controls")] 
	[SerializeField] private float jumpForce;
	
	[Header("Input Actions")]
	[SerializeField] private InputActionReference horizontalActionReference;
	[SerializeField] private InputActionReference verticalActionReference;
	[SerializeField] private InputActionReference jumpActionReference;

	[Header("Debugging")]
	[SerializeField, ReadOnly] private bool isGrounded;
	[SerializeField, Range((float)0.5, 1)] private float rayLength;
	[SerializeField] private LayerMask groundMask;
	
	[SerializeField, ReadOnly] private float horizontalMovement;
	[SerializeField, ReadOnly] private float verticalMovement;
	[SerializeField, ReadOnly] private bool jumpMovement;
	
	private SphereCollider sphereCollider;
	private Rigidbody rigidBody;

	private void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		GetInputs();
		
		GroundChecking();
	}

	private void FixedUpdate()
	{
		Movement();
	}

	private void GetInputs()
	{
		horizontalMovement = horizontalActionReference.action.ReadValue<float>();
		verticalMovement = verticalActionReference.action.ReadValue<float>();

		jumpMovement = jumpActionReference.action.ReadValue<bool>();
	}

	private void GroundChecking() => isGrounded = Physics.Raycast(transform.position, Vector3.down, sphereCollider.radius, groundMask);

	private void Movement()
	{
		
	}

	private void OnEnable()
	{
		jumpActionReference.action.started += Jump;
	}

	private void Jump(InputAction.CallbackContext obj)
	{
		rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
	}

	private void OnDisable()
	{
		
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 endPos = new (transform.position.x, transform.position.y - rayLength, transform.position.z);
		Gizmos.DrawLine(transform.position, endPos);
	}
}

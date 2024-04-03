using UnityEngine;
using NaughtyAttributes;

using System.Collections;

using UnityEngine.InputSystem;
using System;

public enum States
{
	Walking,
	OffGround,
	Gliding,
	Dashing
}

public class PlayerController : MonoBehaviour
{
	[Header("Player Parts")]
	[SerializeField] private Transform playerNormal;
	[SerializeField] private Rigidbody rigidBody;
	[SerializeField] private SphereCollider sphereCollider;
	[SerializeField] private CustomGravity gravity;

	[Header("Movement")] 
	[SerializeField] private AnimationCurve movementSpeed;
	[SerializeField] private float turnSpeed;
	[SerializeField, ReadOnly] private float currentSpeed;
	
	[Header("Jump Controls")] 
	[SerializeField, Range(5, 30)] private float jumpForce;

	[Header("Dash Controls")]
	[SerializeField, Range(10, 40)] private float dashForce;
	[SerializeField] private float dashCooldown;
	[SerializeField, ReadOnly] private bool readyToDash;

	[Header("Input Actions")]
	[SerializeField] private InputActionReference movementActionReference;
	[SerializeField] private InputActionReference jumpActionReference;
	[SerializeField] private InputActionReference glideActionReference;
	[SerializeField] private InputActionReference dashActionReference;
	
	[Header("Debugging")]
	[SerializeField, Range(0.6f, 1)] private float rayLength;
	[SerializeField, ReadOnly] private bool isGrounded;

	private Quaternion normalPosition;

	private void Awake()
	{
		jumpActionReference.action.performed += Jump;
		dashActionReference.action.performed += Dash;
	}

	private void Start()
	{
		rigidBody.drag = 1;

		readyToDash = true;
	}

	private void Update()
	{
		transform.position = rigidBody.transform.position - new Vector3(0, -sphereCollider.radius, 0);
		currentSpeed = rigidBody.velocity.magnitude;
		
		GroundCheck();
	}

    private void FixedUpdate()
    {
        Movement();
		Glide();
    }

	private void GroundCheck()
	{
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength))
        {
            playerNormal.up = Vector3.Lerp(playerNormal.up, hit.normal, Time.deltaTime * 8.0f);
            playerNormal.Rotate(0, transform.eulerAngles.y, 0);

            isGrounded = true;
        }
        else
            isGrounded = false;

	}

    #region Input Functions

    private void Movement()
	{
		Vector2 movement = movementActionReference.action.ReadValue<Vector2>();
		transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * movement.x);

		float speed = movementSpeed.Evaluate(movement.y);

		if (isGrounded)
			rigidBody.AddForce(transform.forward * Time.deltaTime * speed * movement.y, ForceMode.Acceleration);

		else
			rigidBody.AddForce(Vector3.zero * Time.deltaTime * movement.y);
	}

	private void Glide()
	{
		if (!isGrounded && glideActionReference.action.ReadValue<float>() > 0)
			gravity.gravityScale = 0.1f;
		
		else
			gravity.gravityScale = 1f;
	}

	private void Jump(InputAction.CallbackContext obj)
	{
		if(isGrounded)
		{
			rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
			rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
	}

    private void Dash(InputAction.CallbackContext obj)
	{
		if(readyToDash)
		{
			readyToDash = false;
			
			rigidBody.AddForce(transform.forward * dashForce, ForceMode.Impulse);
			
			Invoke(nameof(ResetDash), dashCooldown);
		}
	}

	private void ResetDash() => readyToDash = true;
	
	private void OnEnable()
	{
		movementActionReference.action.Enable();
		
		jumpActionReference.action.Enable();
		glideActionReference.action.Enable();
		dashActionReference.action.Enable();
	}

	private void OnDisable()
	{
		movementActionReference.action.Disable();		
		
		jumpActionReference.action.Disable();
		glideActionReference.action.Disable();
		dashActionReference.action.Disable();
	}
	
#endregion

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 endPos = new (transform.position.x, transform.position.y - rayLength, transform.position.z);
		Gizmos.DrawLine(transform.position, endPos);
	}
}
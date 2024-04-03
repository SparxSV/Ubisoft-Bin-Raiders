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
	[SerializeField, Range(10, 30)] private float movementSpeed;
	
	[Header("Jump Controls")] 
	[SerializeField, Range(5, 30)] private float jumpForce;

	[Header("Dash Controls")]
	[SerializeField, Range(10, 40)] private float dashForce;
	[SerializeField] private float dashCooldown;
	[SerializeField, ReadOnly] private bool readyToDash;

	[Header("Glide Controls")]
	[SerializeField] private float glideDuration;
	[SerializeField] private float glideCooldown;
	[SerializeField, ReadOnly] private bool readyToGlide;

	[Header("Input Actions")]
	[SerializeField] private InputActionReference movementActionReference;
	[SerializeField] private InputActionReference jumpActionReference;
	[SerializeField] private InputActionReference glideActionReference;
	[SerializeField] private InputActionReference dashActionReference;
	
	[Header("Debugging")]
	[SerializeField, Range(0.6f, 1)] private float rayLength;
	[SerializeField, ReadOnly] private bool isGrounded;
	[SerializeField, ReadOnly] private Vector2 movement;
	[SerializeField, ReadOnly] private float glide;

	private Vector3 forceDirection = Vector3.zero;

	private Quaternion normalPosition;

	private void Awake()
	{
		jumpActionReference.action.performed += Jump;
		dashActionReference.action.performed += Dash;
	}

	private void Start()
	{
		rigidBody.drag = 1;

		readyToJump = true;
		readyToGlide = true;
		readyToDash = true;
	}

	private void Update()
	{
		transform.position = rigidBody.transform.position - new Vector3(0, -sphereCollider.radius, 0);
		
		GetInputs();
	}

    private void FixedUpdate()
    {
        Movement();
		Glide();
    }

    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength))
        {
            playerNormal.up = Vector3.Lerp(playerNormal.up, hit.normal, Time.deltaTime * 8.0f);
            playerNormal.Rotate(0, transform.eulerAngles.y, 0);

            return true;
        }
        else
            return false;
    }

    #region Input Functions

    private void GetInputs()
    {
        movement = movementActionReference.action.ReadValue<Vector2>();
		glide = glideActionReference.action.ReadValue<float>();
    }

    private void Movement()
	{
	}

	private void Glide()
	{
		if (!isGrounded && glide > 0)
			gravity.gravityScale = 0.1f;
		
		else
			gravity.gravityScale = 1f;
	}

	private void Jump(InputAction.CallbackContext obj)
	{
		if(IsGrounded())
		{
			//rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
			forceDirection += Vector3.up * jumpForce;
		}
	}

    private void Dash(InputAction.CallbackContext obj)
	{
		if(readyToDash)
		{
			readyToDash = false;
			
			rigidBody.AddForce(forceDirection * dashForce, ForceMode.Impulse);
			
			Invoke(nameof(ResetDash), dashCooldown);
		}
	}

	private void ResetJump() => readyToJump = true;

	private void ResetGlide() => readyToGlide = true;

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
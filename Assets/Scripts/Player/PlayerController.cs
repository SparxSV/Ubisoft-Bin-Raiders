using UnityEngine;
using NaughtyAttributes;

using System;

using Unity.VisualScripting;

using UnityEngine.InputSystem;

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
	[SerializeField] private Animator animator;

	[Header("Player Stats")] 
	public int playerHealth = 3;
	
	[Header("Movement")] 
	[SerializeField] private AnimationCurve movementSpeed;
	[SerializeField] private float turnSpeed;
	[SerializeField, ReadOnly] private float currentSpeed;
	
	[Header("Jump Controls")] 
	[SerializeField, Range(5, 30)] private float jumpForce;
	[SerializeField, ReadOnly] private bool readyToJump;
	[SerializeField, Range(0, 10)] private float jumpCooldown;

	[Header("Attack Controls")]
	[SerializeField, Range(0, 10)] private float attackCooldown;
	[SerializeField, ReadOnly] private bool readyToAttack;
	
	[Header("Glide Controls")]
	[SerializeField, Range(0, 1)] private float glideStrength;

	[Header("Dash Controls")]
	[SerializeField, Range(10, 40)] private float dashForce;
	[SerializeField] private float dashCooldown;
	[SerializeField, ReadOnly] private bool readyToDash;

	[Header("Input Actions")]
	[SerializeField] private InputActionReference movementActionReference;
	[SerializeField] private InputActionReference jumpActionReference;
	[SerializeField] private InputActionReference glideActionReference;
	[SerializeField] private InputActionReference dashActionReference;
	[SerializeField] private InputActionReference slashActionReference;
	
	[Header("Debugging")]
	[SerializeField, Range(0.6f, 1)] private float rayLength;
	[SerializeField, ReadOnly] private bool isGrounded;

	private Vector2 movement;
	private Quaternion normalPosition;

	private void Awake()
	{
		jumpActionReference.action.performed += Jump;
		dashActionReference.action.performed += Dash;
		slashActionReference.action.performed += SwordAttack;
	}

	private void Start()
	{
		rigidBody.drag = 1;

		readyToDash = true;
		readyToJump = true;
	}

	private void Update()
	{
		transform.position = rigidBody.transform.position - new Vector3(0, /*-sphereCollider.radius*/0.25f, 0);
		currentSpeed = rigidBody.velocity.magnitude;

		movement = movementActionReference.action.ReadValue<Vector2>();
		
		GroundCheck();

		Animations();
	}

	private void FixedUpdate()
    {
        Movement();
		Glide();
    }

	private void GroundCheck()
	{
        if (Physics.Raycast(sphereCollider.transform.position, Vector3.down, out RaycastHit hit, rayLength))
        {
            playerNormal.up = Vector3.Lerp(playerNormal.up, hit.normal, Time.deltaTime * 8.0f);
            playerNormal.Rotate(0, transform.eulerAngles.y, 0);

            isGrounded = true;
        }
        else
            isGrounded = false;
	}
	
	private void Animations()
	{
		animator.SetFloat("Forward", movement.y);
		animator.SetFloat("Turn", movement.x);
		
		if(!isGrounded)
			animator.SetBool("Grounded", true);
		else
			animator.SetBool("Grounded", false);
	}

    #region Input Functions

    private void Movement()
	{
		transform.Rotate(Vector3.up * (Time.deltaTime * turnSpeed * movement.x));

		float speed = movementSpeed.Evaluate(movement.y);

		if(isGrounded)
		{
			Vector3 newForward = transform.forward * movement.y;
			rigidBody.AddForce(newForward * (Time.deltaTime * speed), ForceMode.Acceleration);
		}

		else
			rigidBody.AddForce(Vector3.zero * (Time.deltaTime * movement.y));
	}

	private void Glide()
	{
		if(!isGrounded && glideActionReference.action.ReadValue<float>() > 0)
		{
			gravity.gravityScale = glideStrength;
		}

		else
			gravity.gravityScale = 1f;
	}

	private void Jump(InputAction.CallbackContext obj)
	{
		if(isGrounded && readyToJump)
		{
			animator.SetTrigger("Jumping");
			
			rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
			rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			
			Invoke(nameof(ResetJump), jumpCooldown);
		}
	}
	
	private void SwordAttack(InputAction.CallbackContext obj)
	{
		
	}

	private void ResetJump()
	{
		readyToJump = true;
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

	private void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.layer == 8)
		{
			Destroy(other.gameObject);
			playerHealth--;
		}
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 endPos = new (sphereCollider.transform.position.x, sphereCollider.transform.position.y - rayLength, sphereCollider.transform.position.z);
		Gizmos.DrawLine(sphereCollider.transform.position, endPos);
	}
}
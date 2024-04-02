using UnityEngine;
using NaughtyAttributes;

using System.Collections;

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

	[Header("Movement")] 
	[SerializeField, Range(10, 30)] private float movementSpeed;
	[SerializeField] private float dragAmount;
	
	[Header("Jump Controls")] 
	[SerializeField, Range(5, 30)] private float jumpForce;
	[SerializeField] private float jumpCooldown;
	[SerializeField, ReadOnly] private bool readyToJump;

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

	private Vector3 currentDirection;
	private Quaternion normalPosition;

	private void Awake()
	{
		jumpActionReference.action.performed += Jump;
		dashActionReference.action.performed += Dash;
		glideActionReference.action.performed += Glide;
	}

	private void Start()
	{
		normalPosition = new Quaternion(playerNormal.rotation.x, playerNormal.rotation.y, playerNormal.rotation.z, 0);

		rigidBody.drag = dragAmount;

		readyToJump = true;
		readyToGlide = true;
		readyToDash = true;
	}

	private void Update()
	{
		transform.position = rigidBody.transform.position - new Vector3(0, -sphereCollider.radius, 0);

		GroundChecking();
		
		GetInputs();
	}
	
	private void FixedUpdate() => Movement();
	
	private void GroundChecking()
	{
		if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength))
		{
			playerNormal.up = Vector3.Lerp(playerNormal.up, hit.normal, Time.deltaTime * 8.0f);
			playerNormal.Rotate(0, transform.eulerAngles.y, 0);

			isGrounded = true;
		}
		else
		{
			playerNormal.Rotate(normalPosition.x, normalPosition.y, normalPosition.z);

			isGrounded = false;
		}
	}

	#region Input Functions

	private void GetInputs() => movement = movementActionReference.action.ReadValue<Vector2>();

	private void Movement()
	{
		currentDirection = new Vector3(transform.right.x * movement.x, 0, transform.forward.z * movement.y);

		if(isGrounded)
			rigidBody.AddForce(currentDirection * movementSpeed, ForceMode.Acceleration);
		else
			rigidBody.AddForce(rigidBody.velocity);
	}

	private void Jump(InputAction.CallbackContext obj)
	{
		if(isGrounded && readyToJump)
		{
			readyToJump = false;

			rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
			rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			
			Invoke(nameof(ResetJump), jumpCooldown);
		}
	}
	
	private void Dash(InputAction.CallbackContext obj)
	{
		if(readyToDash)
		{
			readyToDash = false;
			
			rigidBody.AddForce(currentDirection * dashForce, ForceMode.Impulse);
			
			Invoke(nameof(ResetDash), dashCooldown);
		}
	}
	
	private void Glide(InputAction.CallbackContext obj)
	{
		if(!isGrounded && readyToGlide && obj.duration > 0.4f)
		{
			readyToGlide = false;
			
			StartCoroutine(GlideTime());

			Invoke(nameof(ResetGlide), glideCooldown);
		}
		
		gravity.gravityScale = 1f;
	}

	private IEnumerator GlideTime()
	{
		gravity.gravityScale = 0.3f;
		yield return new WaitForSeconds(glideDuration);
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
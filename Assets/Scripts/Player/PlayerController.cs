using UnityEngine;
using NaughtyAttributes;

using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlayerController : MonoBehaviour
{
	[Header("Movement")] 
	[SerializeField] private float movementSpeed;
	
	[Header("Jump Controls")] 
	[SerializeField] private float jumpForce;
	[SerializeField] private float jumpCooldown;
	
	[Header("Input Actions")]
	[SerializeField] private InputActionReference horizontalActionReference;
	[SerializeField] private InputActionReference verticalActionReference;
	[SerializeField] private InputActionReference jumpActionReference;

	[Header("Debugging")]
	[SerializeField, ReadOnly] private bool isGrounded;
	[SerializeField, ReadOnly] private bool readyToJump;
	[SerializeField, Range((float)0.5, 1)] private float rayLength;
	
	[SerializeField, ReadOnly] private float horizontalMovement;
	[SerializeField, ReadOnly] private float verticalMovement;
	
	private SphereCollider sphereCollider;
	private Rigidbody rigidBody;

	private void Awake()
	{
		jumpActionReference.action.performed += Jump;
	}

	private void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
		rigidBody = GetComponent<Rigidbody>();

		readyToJump = true;
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
	}

	private void GroundChecking() => isGrounded = Physics.Raycast(transform.position, Vector3.down, sphereCollider.radius);

	private void Movement()
	{
		if(horizontalMovement > 0 && isGrounded)
			Walking(Vector3.right);
		
		if(horizontalMovement < 0 && isGrounded)
			Walking(Vector3.left);
		
		if(verticalMovement > 0 && isGrounded)
			Walking(Vector3.forward);
		
		if(verticalMovement < 0 && isGrounded)
			Walking(Vector3.back);
	}
	
	private void Walking(Vector3 _direction)
	{
		rigidBody.AddForce(_direction * movementSpeed);
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

	private void ResetJump() => readyToJump = true;

	private void OnEnable()
	{
		horizontalActionReference.action.Enable();
		verticalActionReference.action.Enable();
		
		jumpActionReference.action.Enable();
	}


	private void OnDisable()
	{
		horizontalActionReference.action.Disable();
		verticalActionReference.action.Disable();
		
		jumpActionReference.action.Disable();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Vector3 endPos = new (transform.position.x, transform.position.y - rayLength, transform.position.z);
		Gizmos.DrawLine(transform.position, endPos);
	}
}

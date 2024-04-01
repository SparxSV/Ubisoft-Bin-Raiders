using UnityEngine;
using NaughtyAttributes;

using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[Header("Player Parts")]
	//[SerializeField] private Transform playerBody;
	[SerializeField] private Transform playerNormal;
	[SerializeField] private Rigidbody rigidBody;

	[Header("Movement")] 
	[SerializeField] private float movementSpeed;
	[SerializeField] private float dragAmount;
	
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
	
	private Quaternion normalPosition;

	private void Awake() => jumpActionReference.action.performed += Jump;

	private void Start()
	{
		normalPosition = new Quaternion(playerNormal.rotation.x, playerNormal.rotation.y, playerNormal.rotation.z, 0);

		rigidBody.drag = dragAmount;

		readyToJump = true;
	}

	private void Update()
	{
		transform.position = rigidBody.transform.position - new Vector3(0, 0, 0);
		
		GetInputs();
		
	}

	private void FixedUpdate()
	{
		GroundChecking();
		
		Movement();
	}

	private void GetInputs()
	{
		horizontalMovement = horizontalActionReference.action.ReadValue<float>();
		verticalMovement = verticalActionReference.action.ReadValue<float>();
	}

	private void GroundChecking()
	{
		if(Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out RaycastHit hit, rayLength))
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

	private void Movement()
	{
		switch(horizontalMovement)
		{
			case -1:
				Walking(Vector3.left);
				break;
			
			case 1:
				Walking(Vector3.right);
				break;
		}

		switch(verticalMovement)
		{
			case -1:
				Walking(-transform.forward);
				break;
			
			case 1:
				Walking(transform.forward);
				break;
		}
		
		/*if(horizontalMovement > 0 && isGrounded)
			Walking(Vector3.right);
		
		if(horizontalMovement < 0 && isGrounded)
			
		
		if(verticalMovement > 0 && isGrounded)
			Walking(Vector3.forward);
		
		if(verticalMovement < 0 && isGrounded)
			Walking(Vector3.back);*/
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

using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerAttack : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private InputActionReference attackActionReference;
		
		private PlayerController player;

		private void Awake() => attackActionReference.action.performed += Attack;

		private void Start() => player = GetComponent<PlayerController>();

		private void Attack(InputAction.CallbackContext obj)
		{
			animator.SetTrigger("Attack");
		}

		private void OnEnable() => attackActionReference.action.Enable();

		private void OnDisable() => attackActionReference.action.Disable();
	}
}
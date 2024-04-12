using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
	public class PlayerAttack : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private InputActionReference attackActionReference;

		[SerializeField] private Collider weaponCollider;
		
		private PlayerController player;

		private void Awake() => attackActionReference.action.performed += Attack;

		private void Start() => player = GetComponent<PlayerController>();

		private void Attack(InputAction.CallbackContext obj)
		{
			animator.SetTrigger("Attack");
		}

		public void EnableWeaponCollider() => weaponCollider.enabled = true;

		public void DisableWeaponCollider() => weaponCollider.enabled = false;

		private void OnEnable() => attackActionReference.action.Enable();

		private void OnDisable() => attackActionReference.action.Disable();
	}
}
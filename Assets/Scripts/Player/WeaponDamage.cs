using System;

using UnityEngine;

namespace Player
{
	public class WeaponDamage : MonoBehaviour
	{
		[SerializeField] private float damage;

		private void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag("Enemy"))
			{
				
			}
		}
	}
}
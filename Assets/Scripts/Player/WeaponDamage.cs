using AI;

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
				EnemyStats enemy = other.GetComponent<EnemyStats>();
				enemy.TakeDamage(damage);
			}
		}
	}
}
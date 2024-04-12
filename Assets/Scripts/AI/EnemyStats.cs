using UnityEngine;

namespace AI
{
	public class EnemyStats : MonoBehaviour
	{
		public float Health
		{
			get => health;
			set => health = value;
		}

		[SerializeField] private float health;

		public void TakeDamage(float _damage)
		{
			health -= _damage;
			Debug.Log("Health");
		}
	}
}
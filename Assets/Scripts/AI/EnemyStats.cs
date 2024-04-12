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
		[SerializeField] private GameObject deathEffect;

		public void TakeDamage(float _damage)
		{
			health -= _damage;
			Debug.Log("Health: " + health);

			if(health <= 0)
			{
				Destroy(gameObject);
				Destroy(Instantiate(deathEffect), 2);
			}
		}
	}
}
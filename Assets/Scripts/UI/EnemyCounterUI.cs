using System;

using TMPro;

using UnityEngine;

namespace AI
{
	public class EnemyCounterUI : MonoBehaviour
	{
		public int enemyCounter;

		[SerializeField] private TextMeshProUGUI text;

		private void Start() => enemyCounter = transform.childCount;

		private void Update() => text.text = "Enemies: " + enemyCounter;
	}
}
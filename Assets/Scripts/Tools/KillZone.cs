using System;

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillZone : MonoBehaviour
{
	[SerializeField] private Transform entityRespawnPosition;
	
	private new BoxCollider collider;

	private void Start()
	{
		collider = GetComponent<BoxCollider>();

		if(collider != null)
			collider.isTrigger = true;
		
		if(entityRespawnPosition == null)
			Debug.LogError("ERROR: Respawn Position must have a spawn point value!");
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == 6)
		{
			other.transform.position = new Vector3(entityRespawnPosition.position.x, entityRespawnPosition.position.y + 1, entityRespawnPosition.position.z);
		}
	}
}

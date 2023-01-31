using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelArea : MonoBehaviour {
	// Variables
	[SerializeField]
	private Transform startLocation;
	[SerializeField]
	private Transform endLocation;
	[SerializeField]
	private bool finishArea;

	private PlayerController player;

	private bool active = false;
	private int numberOfEnemies = 0;

	// Start is called before the first frame update
	private void Start () {
		//
	}

	// Update is called once per frame
	private void Update () {
		// if (active) {
		// 	if (numberOfEnemies <= 0) {
		// 		Invoke(nameof(AreaComplete), 2.0f);
		// 		active = false;
		// 	}
		// }
	}

	public void ActivateLevelArea (PlayerController newPlayer) {
		// if (active) {
		// 	return;
		// }

		// active = true;
		// player = newPlayer;
		// numberOfEnemies = 0;

		// if (finishArea) {
		// 	Debug.LogError("Game over - resetting in 2 seconds");
		// 	GameController.instance.Invoke(nameof(GameController.instance.RestartGame), 2.0f);

		// 	return;
		// }

		// // Find enemies
		// Collider[] _hits = Physics.OverlapBox(transform.position + this.GetComponent<BoxCollider>().center, this.GetComponent<BoxCollider>().size / 2.0f, transform.rotation);

		// for (int i = 0; i < _hits.Length; i++) {
		// 	if (_hits[i].CompareTag("Enemy")) {
		// 		_hits[i].GetComponent<EnemyController>().InitEnemy(this, player);
		// 	}
		// 	if (_hits[i].CompareTag("Civilian")) {
		// 		_hits[i].GetComponent<HostageController>().InitHostage(this);
		// 	}
		// }

		// if (numberOfEnemies <= 0) {
		// 	// No enemies so end level for player now
		// 	AreaComplete();

		// 	return;
		// }
	}

	public void IncreaseEnemyCount () {
		numberOfEnemies++;
	}

	public void DecreaseEnemyCount () {
		numberOfEnemies--;
	}

	private void AreaComplete () {
		// CancelInvoke(nameof(AreaComplete));

		// player.GoToNewLocation(endLocation.position);
		// active = false;
	}

	private void OnDisable () {
		CancelInvoke();
	}
}

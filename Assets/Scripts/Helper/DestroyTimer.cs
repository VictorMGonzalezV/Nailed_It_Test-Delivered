using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {
	// Variables
	public float timer = 1;

	// Start is called before the first frame update
	private void Start () {
		if (timer <= 0) {
			DestroyObject();
		} else {
			Invoke(nameof(DestroyObject), timer);
		}
	}

	private void DestroyObject () {
		Destroy(this.gameObject);
	}

	private void OnDisable () {
		CancelInvoke();
	}
}

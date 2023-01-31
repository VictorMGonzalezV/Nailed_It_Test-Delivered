using UnityEngine;
using System.Collections;

public class DontDestroy : MonoBehaviour {
	// Use this for initialization
	private void Awake () {
		// Causes game object not to be destroyed when loading a new scene.
		// If you want it to be destroyed, destroy it manually via script.
		DontDestroyOnLoad(this.gameObject);
	}
}

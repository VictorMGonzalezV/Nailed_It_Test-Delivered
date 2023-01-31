using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderer : MonoBehaviour {
	// Start is called before the first frame update
	void Start () {
		if (this.GetComponent<MeshRenderer>()) {
			this.GetComponent<MeshRenderer>().enabled = false;
		} else if (this.GetComponent<Renderer>()) {
			this.GetComponent<Renderer>().enabled = false;
		}
	}
}

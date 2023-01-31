using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterParent : MonoBehaviour {
	// Variables
	[SerializeField]
	private GameObject parent;

	public GameObject GetCharacter () {
		return parent;
	}
}

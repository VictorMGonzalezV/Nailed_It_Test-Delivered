using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableParent : MonoBehaviour {
    [SerializeField] private GameObject parent;
    public GameObject GetParent() {
        return parent;
    }
}

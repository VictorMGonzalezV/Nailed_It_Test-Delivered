using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    // Variables
    [SerializeField]
    private Animator animComp;
    private bool doorOpen = false;
    private void OnTriggerEnter(Collider other) {
        if (!doorOpen) {
            if (other.CompareTag("Player")) {
                doorOpen = true;
                animComp.SetTrigger("open");
            }
        }
    }
}

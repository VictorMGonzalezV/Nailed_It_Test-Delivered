using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGun : MonoBehaviour {
    // Start is called before the first frame update
    Collider colComp;
    void Start() {
        colComp = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update() {

    }
    private bool IgnoreObject(string tag) {
        bool _ignore = false;

        switch (tag) {
            case "Bullet":
            case "Level Area":
            case "GameController":
            case "Player":
            case "Enemy":
            case "Civilian":
            case "Level Trigger":
                _ignore = true;
                break;
            default:
                _ignore = false;
                break;
        }

        return _ignore;
    }
    private void OnCollisionEnter(Collision other) {
        if (IgnoreObject(other.gameObject.tag)) {
            Physics.IgnoreCollision(other.collider, colComp, true);
        }
    }
}

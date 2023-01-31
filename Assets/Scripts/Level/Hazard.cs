using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
    // Start is called before the first frame update
    public bool IsDone = false;
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    private void OnCollisionEnter(Collision col) {
        Collider other = col.collider;
        if (IsDone) {
            return;
        }
        // Check if physics object
        if (other.GetComponent<Rigidbody>()) {
            if (!IgnoreObject(other.tag)) {
                if (other.CompareTag("Character Body Part")) {
                    GameObject character = other.GetComponent<CharacterParent>().GetCharacter();
                    if (character.CompareTag("Enemy")) {
                        EnemyController _enemyController = character.GetComponent<EnemyController>();
                        if (_enemyController.IsCharacterAlive()) {
                            _enemyController.CharacterHit();
                        }
                    } else if (character.CompareTag("Civilian")) {
                        HostageController _hostageController = character.GetComponent<HostageController>();
                        if (_hostageController.IsCharacterAlive()) {
                            _hostageController.CharacterHit();
                        }
                    }
                }
            }
        } else {
            IsDone = true;
        }
    }

    // Check whether to ignore this object
    private bool IgnoreObject(string tag) {
        bool _ignore = false;

        switch (tag) {
            case "Bullet":
            case "Level Area":
            case "Untagged":
            case "Ground":
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
}

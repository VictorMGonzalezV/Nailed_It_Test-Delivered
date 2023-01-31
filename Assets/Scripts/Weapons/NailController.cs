using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using LuckyKat;

public class NailController : MonoBehaviour {
    // Variables
    [SerializeField]
    private Timeline timeComp;
    [SerializeField]
    private Collider colliderComp;
    [SerializeField]
    private Rigidbody rbComp;
    [SerializeField]
    private Transform meshTransform;

    [SerializeField]
    private float bulletForce = 30.0f;

    [SerializeField]
    private GameObject hitFX;
    [SerializeField]
    private GameObject dustFX;

    private bool active = false;
    private float bulletSize = 1.0f;
    private int numberOfObjectsHit = 0;
    private Quaternion bulletRotation;

    private List<GameObject> skewered = new List<GameObject>();


    private float headPosition = 0f;
    private float tipPosition = 3.8f;

    [SerializeField] private GameObject trail;

    //Pre-existing severed head attached to the prefab -VMG
    [SerializeField] private GameObject severedHead;
    

    private List<Joint> jointList = new List<Joint>();
    // Update is called once per frame
    private void Update() {
        if (active) {
            //transform.rotation = bulletRotation;
            //rbComp.velocity = transform.forward * bulletForce;
            // transform.position += transform.forward * bulletForce * timeComp.deltaTime;

            // raycast (maybe multiple) distance from head to 0.75 towards end of tip plus movement
            Vector3 originalPosition = transform.position;
            Ray r = new Ray();
            Ray r2 = new Ray();
            float movement = bulletForce * timeComp.deltaTime;
            Vector3 head = transform.position + transform.forward * headPosition;
            r.origin = transform.position;
            r.direction = transform.forward;
            RaycastHit[] hits = Physics.RaycastAll(r, movement + 0.75f + headPosition);
            // raycast back to head from a further distance to get the exit location of all objects hit and populate and array with its locations
            r2.origin = head + (movement + 5f) * transform.forward;
            r2.direction = transform.forward * -1;
            RaycastHit[] exits = Physics.RaycastAll(r2, movement + 5f);
            float moveHead = 0;
            bool deactivate = false;
            // Vector3 deactivatePosition = new Vector3();
            transform.position = originalPosition + transform.forward * movement;
            // skewer objects onto the nail
            Collider hitStatic = null;
            for (int i = 0; i < hits.Length; i++) {
                Collider col = hits[i].collider;
                // check applicable
                if (!canSkewer(col)) {
                    continue;
                }
                // check for exit positions
                Vector3 exit = new Vector3();
                bool didExit = false;
                foreach (RaycastHit exitHit in exits) {
                    if (exitHit.collider == col) {
                        exit = exitHit.point;
                        didExit = true;
                        break;
                    }
                }
                // check if exit position is greater than current one, we will take the largest distance to move the head
                if (didExit) {
                    moveHead = Mathf.Max(moveHead, Vector3.Distance(hits[i].point, exit));
                }
                // move nail so that skewering will be more accurate for high speeds
                transform.position = originalPosition - head + head + (hits[i].point - head) - transform.forward * 0.6f;
                // skewer
                bool ds = !doSkewer(col);
                deactivate = deactivate || ds;
                if (ds) {
                    // 	deactivatePosition = hits[0].point;
                    hitStatic = col;
                }
            }
            // move towards new position
            transform.position = originalPosition + transform.forward * movement;
            // head position moves to exit location
            headPosition += moveHead;
            // grow nail 3.8 meter further from exit location
            if (moveHead > 0f) {
                Vector3 doScale = meshTransform.localScale;
                // doScale.z += doScale.z / (2.5f + headPosition) * moveHead;
                doScale.z += doScale.z / (3.8f + headPosition) * moveHead;
                meshTransform.localScale = doScale;
            }

            if (deactivate && rbComp.isKinematic) {
                // move to position that caused deactivation
                // transform.position = deactivatePosition - (transform.position + transform.forward * headPosition) + transform.position;
                transform.position += -transform.forward * 0.5f;
                active = false;
                trail.SetActive(false);
                if (hitStatic != null && hitStatic.tag == "Movable") {
                    MovableParent hsParentComp = hitStatic.gameObject.GetComponent<MovableParent>();
                    if (hsParentComp) {
                        GameObject hsParent = hsParentComp.GetParent();
                        if (hsParent) {
                            transform.SetParent(hsParent.transform);
                        } else {
                            transform.SetParent(hitStatic.transform);
                        }
                    } else {
                        transform.SetParent(hitStatic.transform);
                    }
                }
            }
        }
    }

    private bool canSkewer(Collider other) {
        // Check other object
        // Debug.Log(other.tag);
        if (!IgnoreObject(other.tag)) {
            if (other.CompareTag("Character Body Part")) {
                GameObject character = other.GetComponent<CharacterParent>().GetCharacter();
                if (character) {
                    return !skewered.Contains(character);
                }
                return !skewered.Contains(other.gameObject);
            } else if (other.CompareTag("Actionable")) {
                Debug.Log("ISACTIONABLE!");
                NailAction action = other.gameObject.GetComponent<NailAction>();
                if (action) {
                    Debug.Log("HAS ACTION!");
                    action.DoHit();
                }
                return false;
            } else {
                return true;
            }
        }
        return false;
    }
    int headshots = 0;
    private bool doSkewer(Collider other) {
        // Check if physics object
        if (other.GetComponent<Rigidbody>()) {
            if (other.CompareTag("Character Body Part")) {
                GameObject character = other.GetComponent<CharacterParent>().GetCharacter();
                EnemyArmor armor = other.GetComponent<EnemyArmor>();
                if (skewered.Contains(character)) {
                    return true;
                }
                if (armor && armor.active) {
                    armor.DamageArmor();
                    active = false;
                    trail.SetActive(false);
                    // rbComp.isKinematic = false;
                    timeComp.rigidbody.isKinematic = false;
                    colliderComp.isTrigger = false;
                    rbComp.useGravity = true;
                    rbComp.mass = 1f;
                    rbComp.velocity = bulletForce * -transform.forward * 0.2f + bulletForce * transform.up * 0.08f;
                    rbComp.AddTorque(new Vector3(-20f, 0, 0), ForceMode.VelocityChange);
                    EnemyController _enemyController = character.GetComponent<EnemyController>(); // we assume only enemies have armor
                    if (_enemyController) {
                        _enemyController.walkSpeed = -0.02f;
                        _enemyController.transform.position = _enemyController.transform.position + transform.forward * 0.5f;
                    }
                    Animator anim = _enemyController.GetComponent<Animator>();
                    if (anim != null) {
                        // anim.speed;
                        new Tween().SetEase(Tween.Ease.InQuad).SetTime(0.2f).SetStart(-0.3f).SetEnd(anim.speed).SetOnUpdate((float v, float t) => {
                            anim.speed = v;
                        });
                    }

                    // Rigidbody bodyRb = other.GetComponent<Rigidbody>();
                    // if (bodyRb) {
                    //     string bodyName = other.gameObject.name;
                    //     other.gameObject.name = bodyName + "_ARMORHIT";
                    //     TimelineChild timelineChild = other.GetComponent<TimelineChild>();
                    //     // bodyRb.isKinematic = false;
                    //     timelineChild.rigidbody.isKinematic = false;
                    //     timelineChild.rigidbody.AddForce(transform.forward * 1000f, ForceMode.VelocityChange);
                    //     new Tween().SetTime(0.4f).SetOnComplete(() => {
                    //         // timelineChild.rigidbody.isKinematic = true;
                    //         // bodyRb.isKinematic = true;
                    //         // other.gameObject.name = bodyName;
                    //     });
                    // }

                    Vector3 doScale = meshTransform.localScale;
                    // doScale.z += doScale.z / (2.5f + headPosition) * moveHead;
                    doScale.z = 0.4f;
                    meshTransform.localScale = doScale;
                    CreateHitFX();
                    skewered.Add(character);
                    return false;
                    // set 
                } else if (character.CompareTag("Enemy")) {
                    EnemyController _enemyController = character.GetComponent<EnemyController>();
                    SoundPlayer.instance.play("nail-hit-body", Random.Range(0.9f, 1.1f));
                    if (_enemyController.IsCharacterAlive()) {
                        if (other.name != "mixamorig:Head") {
                            _enemyController.CharacterHit(transform.forward);
                        } else {
                            SoundPlayer.instance.play("headshot", 1 + headshots++ * 0.15f);
                            _enemyController.CharacterHit(transform.forward, true);

                            //Instruction to simulate severing the enemy's head, makes the already present mesh head visible -VMG
                           
                            severedHead.SetActive(true);
                        
                          
                        }
                        //Here I disabled the attachment to prevent the nail from "pulling" the enemy in case of a headshot-VMG
                        //AttachNewObjectToBullet(other.gameObject, true);
                        //skewered.Add(character);
                    } else {
                        if (other.gameObject.GetComponent<FixedJoint>() == null) {
                            //FixedJoint _joint = other.gameObject.AddComponent<FixedJoint>();
                            //_joint.connectedBody = rbComp;
                            //Debug.LogWarning("Body part of dead character hit - decide what to do");
                            AttachNewObjectToBullet(other.gameObject, true);
                            skewered.Add(character);
                        } else {
                            BulletHit();
                            return false;
                        }
                    }
                } else if (character.CompareTag("Civilian")) {
                    HostageController _hostageController = character.GetComponent<HostageController>();
                    SoundPlayer.instance.play("nail-hit-body", Random.Range(0.9f, 1.1f));

                    if (_hostageController.IsCharacterAlive()) {
                        _hostageController.CharacterHit();
                        AttachNewObjectToBullet(other.gameObject, true);
                        skewered.Add(character);
                    } else {
                        if (other.gameObject.GetComponent<FixedJoint>() == null) {
                            //FixedJoint _joint = other.gameObject.AddComponent<FixedJoint>();
                            //_joint.connectedBody = rbComp;
                            //Debug.LogWarning("Body part of dead character hit - decide what to do");
                            AttachNewObjectToBullet(other.gameObject, true);
                            skewered.Add(character);
                        }
                    }
                } else {
                    Debug.LogError("Body part of unknown character found");
                }
            } else if (other.GetComponent<Rigidbody>().isKinematic) {
                // if (other.gameObject.GetComponent<FixedJoint>() == null) {
                if (skewered.Count > 0) {
                    SoundPlayer.instance.play("nail-hit-wall-body", Random.Range(0.9f, 1.1f));
                } else {
                    SoundPlayer.instance.play("nail-hit-wall", Random.Range(0.9f, 1.1f));
                }

                BulletHit();
                return false;
                // }
            } else {
                if (other.GetComponent<FixedJoint>() == null) {
                    AttachNewObjectToBullet(other.gameObject, false);
                    skewered.Add(other.gameObject);
                    //Rigidbody _rb = other.GetComponent<Rigidbody>();
                    //_rb.velocity = rbComp.velocity;
                    //_rb.useGravity = false;
                    //_rb.isKinematic = true;
                }
            }
            return true;
        } else {
            if (skewered.Count > 0) {
                SoundPlayer.instance.play("nail-hit-wall-body", Random.Range(0.9f, 1.1f));
            } else {
                SoundPlayer.instance.play("nail-hit-wall", Random.Range(0.9f, 1.1f));
            }
            BulletHit();
            return false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        // if (active) {
        // 	// Check other object
        // 	if (!IgnoreObject(other.tag)) {
        // 		// Check if physics object
        // 		if (other.GetComponent<Rigidbody>()) {
        // 			if (other.CompareTag("Character Body Part")) {
        // 				GameObject character = other.GetComponent<CharacterParent>().GetCharacter();
        // 				if (skewered.Contains(character)) {
        // 					return;
        // 				}
        // 				if (character.CompareTag("Enemy")) {
        // 					EnemyController _enemyController = character.GetComponent<EnemyController>();

        // 					if (_enemyController.IsCharacterAlive()) {
        // 						_enemyController.CharacterHit();
        // 						AttachNewObjectToBullet(other.gameObject, true);
        // 						skewered.Add(character);
        // 					} else {
        // 						if (other.gameObject.GetComponent<FixedJoint>() == null) {
        // 							//FixedJoint _joint = other.gameObject.AddComponent<FixedJoint>();
        // 							//_joint.connectedBody = rbComp;
        // 							//Debug.LogWarning("Body part of dead character hit - decide what to do");
        // 							AttachNewObjectToBullet(other.gameObject, true);
        // 							skewered.Add(character);
        // 						}
        // 					}
        // 				} else if (character.CompareTag("Civilian")) {
        // 					HostageController _hostageController = character.GetComponent<HostageController>();

        // 					if (_hostageController.IsCharacterAlive()) {
        // 						_hostageController.CharacterHit();
        // 						AttachNewObjectToBullet(other.gameObject, true);
        // 						skewered.Add(character);
        // 					} else {
        // 						if (other.gameObject.GetComponent<FixedJoint>() == null) {
        // 							//FixedJoint _joint = other.gameObject.AddComponent<FixedJoint>();
        // 							//_joint.connectedBody = rbComp;
        // 							//Debug.LogWarning("Body part of dead character hit - decide what to do");
        // 							AttachNewObjectToBullet(other.gameObject, true);
        // 							skewered.Add(character);
        // 						}
        // 					}
        // 				} else {
        // 					Debug.LogError("Body part of unknown character found");
        // 				}
        // 			} else if (other.GetComponent<Rigidbody>().isKinematic) {
        // 				if (other.gameObject.GetComponent<FixedJoint>() == null) {
        // 					BulletHit();
        // 				}
        // 			} else {
        // 				if (other.GetComponent<FixedJoint>() == null) {
        // 					AttachNewObjectToBullet(other.gameObject, false);
        // 					//Rigidbody _rb = other.GetComponent<Rigidbody>();
        // 					//_rb.velocity = rbComp.velocity;
        // 					//_rb.useGravity = false;
        // 					//_rb.isKinematic = true;
        // 				}
        // 			}
        // 		} else {
        // 			BulletHit();
        // 		}
        // 	}
        // }
    }

    public void ActivateBullet(Vector3 newPos, Quaternion newRot) {
        if (active) {
            return;
        }

        // Initialize variables
        active = true;


        // Shoot bullet
        transform.position = newPos;
        transform.rotation = newRot;
        bulletRotation = newRot;

        //timeComp.rigidbody.velocity = transform.forward * bulletForce;
        //rbComp.velocity = transform.forward * bulletForce;
    }

    private void BulletHit() {
        rbComp.velocity = Vector3.zero;
        colliderComp.enabled = false;
        // active = false;

        // Rotate bullet
        // if (transform.position.y < 1) {
        // 	transform.rotation = Quaternion.Euler(Random.Range(10.0f, 25.0f), transform.eulerAngles.y, transform.eulerAngles.z);
        // } else if (transform.position.x > 0) {
        // 	transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(-25.0f, -10.0f), transform.eulerAngles.z);
        // } else {
        // 	transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(10.0f, 25.0f), transform.eulerAngles.z);
        // }
        // Rotate bullet slightly to show that we are shooting nails
        float minRange = 4f;
        // float maxRange = 15f;
        float maxRange = 8f;
        Vector3 offsetEuler = new Vector3(Random.Range(-maxRange, maxRange), Random.Range(-maxRange, maxRange), Random.Range(-maxRange, maxRange));
        if (offsetEuler.magnitude < minRange) {
            offsetEuler = offsetEuler.normalized * minRange;
        }
        transform.rotation *= Quaternion.Euler(offsetEuler);

        // push it back out a little bit because the collider does not cover the full bullet
        // transform.position += (-transform.forward * (bulletSize * 0.45f));

        // FX
        CreateDustFX();

        // Set Joints unbreakable to prevent some issues
        foreach (Joint j in jointList) {
            if (j != null) {
                j.breakForce = Mathf.Infinity;
            }
        }
    }

    // Check whether to ignore this object
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

    private void AttachNewObjectToBullet(GameObject newObject, bool isCharacter) {
        // Update position of object
        // if (!isCharacter) {
        // 	newObject.transform.position = transform.position + (-transform.forward * ((numberOfObjectsHit * 1) + 1));
        // }
        numberOfObjectsHit++;

        // Attach joint
        FixedJoint _joint = newObject.AddComponent<FixedJoint>();
        // CharacterJoint _joint = newObject.AddComponent<CharacterJoint>();
        _joint.connectedBody = rbComp;
        // _joint.enableCollision = true;
        _joint.enablePreprocessing = false;
        _joint.breakForce = 2000000f;
        jointList.Add(_joint);

        if (isCharacter) {
            // FX
            CreateHitFX();
            //Vibration.VibratePeek();
        } else {
            //newObject.GetComponent<Rigidbody>().useGravity = false;
            //newObject.GetComponent<Rigidbody>().isKinematic = true;
            newObject.GetComponent<Collider>().enabled = false;
            Hazard h = newObject.GetComponent<Hazard>();
            if (h != null) {
                h.IsDone = true;
            }

            // FX
            CreateDustFX();
        }

        // Increase bullet size
        // IncreaseBulletSize();
    }

    private void CreateHitFX() {
        GameObject _fx = Instantiate(hitFX, transform.position, transform.rotation);
        _fx.SetActive(true);
    }

    private void CreateDustFX() {
        GameObject _fx = Instantiate(dustFX, transform.position, transform.rotation);
        _fx.SetActive(true);
    }

    private void IncreaseBulletSize() {
        bulletSize += 1.0f;

        // meshTransform.localScale = new Vector3(1f, 1f, ClampSize(1 + (bulletSize * 0.5f))); clampsize?
        meshTransform.localScale = new Vector3(1f, 1f, 1 + bulletSize * 0.5f);
    }

    private float ClampSize(float newValue) {
        return Mathf.Clamp(newValue, 1, newValue);
    }
}
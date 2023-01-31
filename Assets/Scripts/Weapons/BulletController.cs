using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class BulletController : MonoBehaviour {
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

    // Update is called once per frame
    private void Update() {
        if (active) {
            //transform.rotation = bulletRotation;
            //rbComp.velocity = transform.forward * bulletForce;
            transform.position += transform.forward * bulletForce * timeComp.deltaTime;
            //transform.Translate(transform.forward * bulletForce * timeComp.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (active) {
            // Check other object
            if (!IgnoreObject(other.tag)) {
                // Check if physics object
                if (other.GetComponent<Rigidbody>()) {
                    if (other.CompareTag("Character Body Part")) {
                        GameObject character = other.GetComponent<CharacterParent>().GetCharacter();
                        if (skewered.Contains(character)) {
                            return;
                        }
                        if (character.CompareTag("Enemy")) {
                            EnemyController _enemyController = character.GetComponent<EnemyController>();

                            if (_enemyController.IsCharacterAlive()) {
                                _enemyController.CharacterHit();
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
                        } else if (character.CompareTag("Civilian")) {
                            HostageController _hostageController = character.GetComponent<HostageController>();

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
                        if (other.gameObject.GetComponent<FixedJoint>() == null) {
                            BulletHit();
                        }
                    } else {
                        if (other.GetComponent<FixedJoint>() == null) {
                            AttachNewObjectToBullet(other.gameObject, false);
                            //Rigidbody _rb = other.GetComponent<Rigidbody>();
                            //_rb.velocity = rbComp.velocity;
                            //_rb.useGravity = false;
                            //_rb.isKinematic = true;
                        }
                    }
                } else {
                    BulletHit();
                }
            }
        }
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
        active = false;

        // Rotate bullet
        // if (transform.position.y < 1) {
        // 	transform.rotation = Quaternion.Euler(Random.Range(10.0f, 25.0f), transform.eulerAngles.y, transform.eulerAngles.z);
        // } else if (transform.position.x > 0) {
        // 	transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(-25.0f, -10.0f), transform.eulerAngles.z);
        // } else {
        // 	transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(10.0f, 25.0f), transform.eulerAngles.z);
        // }
        // Rotate bullet slightly to show that we are shooting nails
        float minRange = 8f;
        float maxRange = 20f;
        Vector3 offsetEuler = new Vector3(Random.Range(-maxRange, maxRange), Random.Range(-maxRange, maxRange), Random.Range(-maxRange, maxRange));
        if (offsetEuler.magnitude < minRange) {
            offsetEuler = offsetEuler.normalized * minRange;
        }
        transform.rotation *= Quaternion.Euler(offsetEuler);

        // push it back out a little bit because the collider does not cover the full bullet
        transform.position += (-transform.forward * (bulletSize * 0.45f));

        // FX
        CreateDustFX();
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
        if (!isCharacter) {
            newObject.transform.position = transform.position + (-transform.forward * ((numberOfObjectsHit * 1) + 1));
        }
        numberOfObjectsHit++;

        // Attach joint
        FixedJoint _joint = newObject.AddComponent<FixedJoint>();
        _joint.connectedBody = rbComp;
        // _joint.enableCollision = true;
        _joint.enablePreprocessing = false;
        _joint.breakForce = 120000f;

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
        IncreaseBulletSize();
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

        meshTransform.localScale = new Vector3(ClampSize(1 + (bulletSize * 0.25f)), ClampSize(1 + (bulletSize * 0.25f)), ClampSize(1 + (bulletSize * 0.5f)));
    }

    private float ClampSize(float newValue) {
        return Mathf.Clamp(newValue, 1, newValue);
    }
}

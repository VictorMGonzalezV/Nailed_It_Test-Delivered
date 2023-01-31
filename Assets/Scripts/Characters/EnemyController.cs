using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Chronos;
using LuckyKat;

public class EnemyController : MonoBehaviour {
    // Variables
    [SerializeField]
    private Timeline timeComp;
    [SerializeField]
    private Animator animComp;
    [SerializeField]
    private Collider colliderComp;
    [SerializeField]
    // private NavMeshAgent agentComp;
    // [SerializeField]
    private GameObject[] characterBodyParts;
    // [SerializeField]
    // private Renderer rendererComp;
    [SerializeField]
    private Renderer[] rendererCompList;
    [SerializeField]
    private GameObject[] deactivateOnDeath;

    [SerializeField]
    private float moveSpeed = 10.0f;

    public bool active = false;
    private LevelArea currentArea;
    private PlayerController playerTarget;

    static private bool resetting = false;
    [SerializeField]
    private GameObject dieFx;
    [SerializeField]
    private GameObject headshotFx;
    [SerializeField]
    private MafiaAccessory Glasses;
    [SerializeField]
    private MafiaAccessory Weapon;

    public NavigationNode currentPath;
    private NavigationNode currentNode;
    public float walkSpeed = 0;
    private float accelerate = 0.01f;
    private float maxSpeed = 0.1f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    // Start is called before the first frame update
    private void Start() {
        startPosition = transform.position;
        startRotation = transform.rotation;
        maxSpeed = moveSpeed * 0.01f;
    }

    // Update is called once per frame
    private void Update() {
        if (active) {
            if (!GameController.instance.HasGameStarted() || GameController.instance.IsGameOver()) {
                // timeComp.navMeshAgent.speed = 0;
                animComp.SetBool("move", false);

                return;
            }

            float frameTime = timeComp.deltaTime * 60f;
            // Chase player
            if (playerTarget) {
                // agentComp.SetDestination(playerTarget.transform.position + playerTarget.transform.forward * 2f);
                // timeComp.navMeshAgent.speed = moveSpeed;
                animComp.SetBool("move", true);
                // if (!GameController.instance.gameOver && !agentComp.pathPending && agentComp.remainingDistance < 1.6f) {
                //     Debug.LogError("Game over - show fail screen");
                //     // Invoke(nameof(gameOver), 4.0f);
                //     GameController.instance.Fail();
                //     PlayerController.instance.Die();
                //     resetting = true;
                // }
                if (currentNode) {
                    // distance from node
                    Vector3 displacement = currentNode.transform.position - transform.position;
                    float nodeDistance = displacement.magnitude;
                    // add forces
                    walkSpeed = Mathf.MoveTowards(walkSpeed, maxSpeed, accelerate * frameTime);
                    // walk path (contains logic what to do when arriving)
                    if (walkSpeed > 0) {
                        float togo = walkSpeed;
                        while (togo > 0 && currentNode) {
                            if (nodeDistance <= togo) { // arrive, figure out what to do next based on node type
                                transform.position = currentNode.transform.position;
                                togo -= nodeDistance;
                                currentNode = currentNode.Next;
                                if (currentNode != null) {
                                    // faceNode(0);
                                    displacement = currentNode.transform.position - transform.position;
                                    nodeDistance = displacement.magnitude;
                                } else {
                                    togo = 0;
                                }
                            } else { // just walk towards target
                                displacement /= nodeDistance;
                                transform.position = transform.position + displacement * togo * frameTime;
                                togo = 0f;
                                Vector3 nodeVector = currentNode.transform.position - (currentNode.Previous ? currentNode.Previous.transform.position : startPosition);
                                faceNode(currentNode.transform.position);
                                // faceNode(Mathf.Clamp(1f - nodeDistance / nodeVector.magnitude, 0, 1));
                            }
                        }
                    }
                } else if (!GameController.instance.gameOver) { // walk straight towards player
                    walkSpeed = Mathf.MoveTowards(walkSpeed, maxSpeed, accelerate * frameTime);
                    Vector3 displacement = playerTarget.transform.position - transform.position;
                    float nodeDistance = displacement.magnitude;
                    if (nodeDistance > 4.2f) {
                        displacement /= nodeDistance;
                        transform.position = transform.position + displacement * walkSpeed * frameTime;
                        faceNode(playerTarget.transform.position);
                    } else {
                        Debug.LogError("Game over - show fail screen");
                        // Invoke(nameof(gameOver), 4.0f);
                        GameController.instance.Fail();
                        PlayerController.instance.Die();
                        resetting = true;
                    }
                }
            } else {
                // timeComp.navMeshAgent.speed = 0;
                // animComp.SetBool("move", false);
            }
        }
    }

    private void gameOver() {
        // GameController.instance.RestartGame();
        // resetting = false;
    }

    private void faceNode(Vector3 target) {
        Vector3 displacement = target - transform.position;
        displacement.y = 0;
        if (displacement.sqrMagnitude > 0) {
            Quaternion q = Quaternion.LookRotation(displacement, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, 0.1f * timeComp.deltaTime * 60f);
        }
    }
    // private void faceNode(float progress) {
    //     // Debug.Log("Progress" + progress);

    //     float frameTime = timeComp.deltaTime * 60f;
    //     if (currentNode == null) return;

    //     Quaternion getRotation(NavigationNode n) {
    //         if (n == null) return startRotation;
    //         Vector3 posA = n.transform.position;
    //         Vector3 posB = n.Previous ? n.Previous.transform.position : startPosition;
    //         Vector3 lookVector = posA - posB;
    //         Quaternion q = Quaternion.LookRotation(lookVector, Vector3.up);
    //         return q;
    //     }
    //     Quaternion angleA = getRotation(currentNode);
    //     Quaternion angleB = getRotation(currentNode.Previous);

    //     transform.rotation = Quaternion.Lerp(angleB, angleA, progress);
    // }

    #region Helper functions
    // public void InitEnemy (LevelArea newArea, PlayerController newPlayerTarget) {
    public void InitEnemy(PlayerController newPlayerTarget) {
        active = true;
        // currentArea = newArea;
        playerTarget = newPlayerTarget;

        UpdateBodyParts(false);

        // currentArea.IncreaseEnemyCount();
        currentNode = currentPath;
    }

    public void CharacterHit() {
        nailForce = Vector3.zero;
        _CharacterHit();
    }

    public void CharacterHit(Vector3 force) {
        nailForce = force;
        _CharacterHit();
    }

    public void CharacterHit(Vector3 force, bool isHeadshot) {
        nailForce = force;
        headshot = isHeadshot;
        _CharacterHit();
    }
    private bool headshot = false;
    private Vector3 nailForce = new Vector3();
    private void _CharacterHit() {
        if (!active) {
            return;
        }
        GameController.instance.KilledEnemy();
        if (!headshot) {
            Instantiate(dieFx, transform.position + transform.up * 6.5f, new Quaternion());
        } else {
            Instantiate(headshotFx, transform.position + transform.up * 6.5f, new Quaternion());
            GameController.instance.Headshot();

            //Dirty solution to create the illusion that the head is gone
            //Breaking the joint didn't work because the model's skin stretched along the path of the severed head
            //I manually scale the head to 0,0,0 so it becomes invisible to create the illusion of being severed-VMG
            characterBodyParts[8].transform.localScale = new Vector3(0f, 0f, 0f);
        }

        // Update variables
        active = false;

        // FX
        GameController.instance.ActivateHitEffect();

        // Update area count
        // currentArea.DecreaseEnemyCount();

        // Update components
        timeComp.globalClockKey = "Level";
        animComp.enabled = false;
        colliderComp.enabled = false;
        // agentComp.enabled = false;

        // Activate physics
        UpdateBodyParts(true);
        foreach (Renderer r in rendererCompList) {
            Renderer rendererComp = r;
            // Update color
            new Tween().SetEase(Tween.Ease.OutCubic).SetTime(0.5f).SetStart(1.0f).SetEnd(0.01f).SetOnUpdate((float v, float t) => {
                MaterialPropertyBlock _customMaterial = new MaterialPropertyBlock();
                rendererComp.GetPropertyBlock(_customMaterial);
                _customMaterial.SetFloat("greyout", v);
                rendererComp.SetPropertyBlock(_customMaterial);
            }).SetOnComplete(() => {
                MaterialPropertyBlock _customMaterial = new MaterialPropertyBlock();
                rendererComp.GetPropertyBlock(_customMaterial);
                _customMaterial.SetFloat("greyout", 0.01f);
                rendererComp.SetPropertyBlock(_customMaterial);
            });
        }

        foreach (GameObject g in deactivateOnDeath) {
            g.SetActive(false);
        }

        // if (Random.value < 0.2f) {
        Invoke(nameof(DoAccessories), 0.1f);
        // }
    }

    private void DoAccessories() {
        if (Glasses != null) {
            Glasses.EnablePhysics();
            Glasses.GreyOut();
            Glasses.ScaleDown();
            // Glasses.transform.parent = null;
            float gTorque = 0.08f * 50;
            Glasses.thisRigidbody.AddForce((nailForce * 0.12f + Vector3.up * 0.06f) * 50f, ForceMode.VelocityChange);
            Glasses.thisRigidbody.AddTorque(new Vector3(Random.Range(-gTorque, gTorque), Random.Range(-gTorque, gTorque), Random.Range(-gTorque, gTorque)), ForceMode.VelocityChange);
        }

        if (Weapon != null) {
            Weapon.EnablePhysics();
            Weapon.GreyOut();
            Weapon.ScaleDown();
            float wTorque = 0.12f * 50;
            // Weapon.transform.parent = null;
            Weapon.thisRigidbody.AddForce((nailForce * 0.06f + Vector3.up * 0.12f) * 50f, ForceMode.VelocityChange);
            Weapon.thisRigidbody.AddTorque(new Vector3(Random.Range(-wTorque, wTorque), Random.Range(-wTorque, wTorque), Random.Range(-wTorque, wTorque)), ForceMode.VelocityChange);
        }
    }

    private void UpdateBodyParts(bool active) {
        for (int i = 0; i < characterBodyParts.Length; i++) {
            //characterBodyParts[i].GetComponent<Collider>().enabled = active;
            characterBodyParts[i].GetComponent<Collider>().isTrigger = false;

            characterBodyParts[i].GetComponent<TimelineChild>().rigidbody.isKinematic = !active;
            characterBodyParts[i].GetComponent<TimelineChild>().rigidbody.useGravity = active;
            characterBodyParts[i].GetComponent<TimelineChild>().rigidbody.velocity = Vector3.zero;
            characterBodyParts[i].GetComponent<Rigidbody>().detectCollisions = true;
        }
    }

    public bool IsCharacterAlive() {
        return active;
    }

    private void OnDisable() {
        //
    }
    #endregion
    // private void OnCollisionEnter(Collision other) {
    // 	Debug.Log("HITTING" + other.gameObject.name);

    // 			// if (!agentComp.pathPending && agentComp.remainingDistance < 0.2f && !resetting) { // reached will reset soon
    // 			// 	Debug.LogError("Game over - resetting in 4 seconds");
    // 			// 	Invoke(nameof(gameOver), 4.0f);
    // 			// }
    // }
    // private void OnTriggerEnter(Collider other) {
    // 	Debug.Log("HITTING" + other.gameObject.name);

    // }
    // private void OnControllerColliderHit(ControllerColliderHit hit) {
    // 	Debug.Log("HITTING" + hit.gameObject.name);
    // 	// if (!agentComp.pathPending && agentComp.remainingDistance < 0.2f && !resetting) { // reached will reset soon
    // 	// 	Debug.LogError("Game over - resetting in 4 seconds");
    // 	// 	Invoke(nameof(gameOver), 4.0f);
    // 	// }
    // }
}

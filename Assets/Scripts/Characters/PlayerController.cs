using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Chronos;
using LuckyKat;

public class PlayerController : MonoBehaviour {
    static public PlayerController instance;
    #region Variables
    [Header("References")]
    [SerializeField]
    private Timeline timeComp;
    [SerializeField]
    private Animator animComp;
    [SerializeField]
    private CharacterController charComp;
    [SerializeField]
    private NavMeshAgent agentComp;
    // [SerializeField]
    // private GunController gunComp;
    [SerializeField]
    private GameObject bullet;

    [Header("Control")]
    [SerializeField]
    private float moveSpeed = 25.0f;

    // Level
    private LevelArea currentArea;

    // Control
    private bool init = true;
    private bool moveToNewLocation;
    private Vector3 newLocation;
    // private Vector3 inputOffset = new Vector3(0, Screen.height * 0.02f, 0);
    private Vector3 inputOffset = new Vector3(0, 0, 0);
    #endregion
    [SerializeField]
    private BackgroundContainer backgroundContainer;
    private NavigationNode currentPath;
    private NavigationNode currentNode;
    public float PathProgress = 0;
    public float PathSize = 0;
    private float nodeTime = 0f;
    private float turnSpeed = 0.1f;
    private bool dead = false;
    [SerializeField]
    private GameObject gunDeath;
    [SerializeField]
    private GameObject playerCam;
    [SerializeField]
    private CameraController camComp;

    private float walkSpeed = 0;
    private float accelerate = 0.02f;
    private float brakeSpeed = 0.04f;
    static public float maxSpeed = 0.4f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void OnEnable() {
        InitPlayer();
    }

    // Start is called on the frame when a script is enabled
    void Start() {
        if (backgroundContainer != null) {
            Camera.main.backgroundColor = backgroundContainer.colors[ThemeSwitcher.Theme % backgroundContainer.colors.Length];
        }
        instance = this;
        startPosition = transform.position;
        startRotation = transform.rotation;
        GetPathSize();
    }

    void GetPathSize() {
        Vector3 walkPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        NavigationNode node = currentPath;
        PathSize = 0;
        while (node) {
            PathSize += Vector3.Distance(walkPosition, node.transform.position);
            walkPosition = node.transform.position;
            node = node.Next;
        }
    }

    private void faceNode(float progress) {
        // Debug.Log("Progress" + progress);

        float frameTime = timeComp.deltaTime * 60f;
        if (currentNode == null) return;

        Quaternion getRotation(NavigationNode n) {
            if (n == null) return startRotation;
            if (n.alignRotation) return n.transform.rotation;

            Vector3 posA = n.transform.position;
            Vector3 posB = n.Previous ? n.Previous.transform.position : startPosition;
            Vector3 lookVector = posA - posB;
            if (lookVector.sqrMagnitude == 0f) return transform.rotation;
            Quaternion q = Quaternion.LookRotation(lookVector, Vector3.up);
            return q;
        }
        Quaternion angleA = getRotation(currentNode);
        Quaternion angleB = getRotation(currentNode.Previous);

        transform.rotation = Quaternion.Lerp(angleB, angleA, progress);
    }

    float getBrakingDistance() {
        float speed = walkSpeed;
        float distance = 0;
        while (speed > 0) {
            speed -= brakeSpeed;
            distance += speed;
        }
        return distance;
    }
    bool NodeAction() {
        switch (currentNode.NodeType) {
            case NavigationNode.Activities.Shoot:
                if (currentNode.focusOnEnemies && !dead) {
                    EnemyController enemy = null;
                    float distSqr = Mathf.Infinity;
                    foreach (EnemyController e in currentNode.Enemies) {
                        if (e == null) continue;
                        if (!e.active) continue;
                        float mag = Vector3.SqrMagnitude(e.transform.position - transform.position);
                        if (mag < distSqr) {
                            enemy = e;
                            distSqr = mag;
                        }
                    }
                    if (enemy) {
                        camComp.focusTarget = enemy.gameObject;
                    }
                }
                if (currentNode.activated) {
                    if (currentNode.CheckStatus()) {
                        currentNode.NodeTime += Time.deltaTime;
                        if (currentNode.NodeTime >= 1f) {
                            camComp.focusTarget = null;
                            NextNode();
                            return true;
                        }
                    }
                }
                currentNode.ActivateNode(this);
                return false;
            case NavigationNode.Activities.Save:
                if (currentNode.activated) {
                    if (currentNode.CheckStatus()) {
                        currentNode.NodeTime += Time.deltaTime;
                        if (currentNode.NodeTime >= 1f) {
                            NextNode();
                            return true;
                        }
                    }
                }
                currentNode.ActivateNode(this);
                return false;
            case NavigationNode.Activities.Exit:
                // Invoke(nameof(nextLevel), 4.0f);
                GameController.instance.Win();
                NextNode();
                return false;
            default:
                currentNode.ActivateNode(this);
                NextNode();
                return true;
        }
    }

    void NextNode() {
        currentNode = currentNode.Next;
        if (currentNode != null) {
            // add extra turning logic

            // switch (currentNode.NodeType) {
            //     case NavigationNode.Activities.Shoot
            // }
        }
    }

    // Update is called once per frame
    private void Update() {
        if (!GameController.instance.HasGameStarted() || GameController.instance.IsGameOver()) {
            return;
        }
        bool canShoot = true;
        // float frameTime = Time.deltaTime * 60f;
        float frameTime = timeComp.deltaTime * 60f;
        if (currentNode) {
            // decide if needs to brake when arriving based on node type
            bool brake = true;
            switch (currentNode.NodeType) {
                case NavigationNode.Activities.Walk:
                case NavigationNode.Activities.Jump:
                    brake = false;
                    break;
            }
            // get brake distance
            float brakeDistance = 0;
            if (brake) {
                brakeDistance = getBrakingDistance();
            }
            // distance from node
            Vector3 displacement = currentNode.transform.position - transform.position;
            float nodeDistance = displacement.magnitude;
            if (nodeDistance > brakeDistance) {
                brake = false;
            }
            // add forces
            if (brake) {
                walkSpeed = Mathf.MoveTowards(walkSpeed, 0, brakeSpeed * frameTime);
            } else {
                walkSpeed = Mathf.MoveTowards(walkSpeed, maxSpeed, accelerate * frameTime);
            }
            // walk path (contains logic what to do when arriving)
            if (walkSpeed > 0) {
                float togo = walkSpeed;
                while (togo > 0 && currentNode) {
                    if (nodeDistance <= togo || brake && nodeDistance < brakeSpeed) { // arrive, figure out what to do next based on node type
                        transform.position = currentNode.transform.position;
                        togo -= nodeDistance;
                        PathProgress += nodeDistance;
                        if (NodeAction()) {
                            if (currentNode != null) {
                                faceNode(0);
                                displacement = currentNode.transform.position - transform.position;
                                nodeDistance = displacement.magnitude;
                            } else {
                                togo = 0;
                            }
                        } else {
                            faceNode(1);
                            togo = 0;
                        }
                    } else { // just walk towards target
                        PathProgress += togo * frameTime;
                        displacement /= nodeDistance;
                        transform.position = transform.position + displacement * togo * frameTime;
                        togo = 0f;
                        Vector3 nodeVector = currentNode.transform.position - (currentNode.Previous ? currentNode.Previous.transform.position : startPosition);
                        faceNode(Mathf.Clamp(1f - nodeDistance / nodeVector.magnitude, 0, 1));
                    }
                }
            } else {
                // check node type action because apparently we have stopped moving
                if (NodeAction()) {
                    faceNode(0f);
                } else {
                    faceNode(1f);
                }
            }
        }

        if (Input.GetMouseButton(0) && canShoot) {
            // Shoot
            if (Input.GetMouseButtonDown(0)) {
                Shoot(true);
            } else {
                Shoot();
            }
        }
    }

    // private void nextLevel() {
    //     GameController.instance.LoadLevel(GameController.instance.CurrentLevel + 1);
    // }

    private void Shoot(bool force = false) {
        // Rotate gun in direction of shooting
        Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition + inputOffset);
        RaycastHit _hit;
        GunController gunComp = WeaponManager.instance.controller;
        Debug.Log(gunComp == null);
        if (Physics.Raycast(_ray, out _hit, 100.0f)) {
            // Update gun rotation
            gunComp.transform.rotation = Quaternion.LookRotation(_hit.point - gunComp.transform.position);
        } else {
            // Update gun rotation
            gunComp.transform.rotation = Quaternion.LookRotation(_ray.direction);
        }

        //Debug.DrawRay(_ray.origin, _ray.direction * 50.0f, Color.red, 10.0f);
        //Debug.DrawLine(gunComp.GetGunMuzzle().position, gunComp.GetGunMuzzle().position + (gunComp.GetGunMuzzle().forward * 50.0f), Color.blue, 10.0f);

        // Shoot
        if (gunComp.CanShoot() || force) {
            Transform gunMuzzle = gunComp.GetGunMuzzle();
            CreateBullet(gunMuzzle.position, gunMuzzle.rotation);

            // FX
            //Vibration.VibratePop();
            CameraController.instance.UpdateScreenShake(0.15f);

            // SFX
            SoundPlayer.instance.play("nailgun", Random.Range(0.9f, 1.1f));
        }
    }

    private void CreateBullet(Vector3 newPos, Quaternion newRot) {
        // Update gun
        WeaponManager.instance.controller.Shoot();

        // Create bullet
        GameObject _bullet = Instantiate(bullet, newPos, newRot);
        _bullet.SetActive(true);
        _bullet.GetComponent<NailController>().ActivateBullet(newPos, newRot);
    }

    public void Die() {
        if (dead) {
            return;
        }
        dead = true;
        // turn on red overlay
        // camera drop and look up
        new Tween().SetEase(Tween.Ease.InCubic).SetTime(1.2f).SetStart(playerCam.transform.position.y).SetEnd(0.7f).SetOnUpdate((float v, float t) => {
            Vector3 pV = playerCam.transform.position;
            pV.y = v;
            playerCam.transform.position = pV;
        });

        new Tween().SetEase(Tween.Ease.InOutCubic).SetTime(0.7f).SetStart(playerCam.transform.rotation.eulerAngles.x).SetEnd(-50f).SetOnUpdate((float v, float t) => {
            Vector3 rV = playerCam.transform.rotation.eulerAngles;
            rV.x = v;
            playerCam.transform.rotation = Quaternion.Euler(rV);
        });

        // gun fly away dramatically
        GunController gunComp = WeaponManager.instance.controller;
        gunComp.Die();
        camComp.active = false;
        // GameObject gd = Instantiate(gunDeath, gunComp.transform.position, gunComp.transform.rotation);
        // Rigidbody gunRb = gd.GetComponent<Rigidbody>();
        // Vector3 f = transform.rotation * new Vector3(Random.Range(-3f, 3f), Random.Range(10, 13), -Random.Range(2f, 3f));
        // gunRb.AddForce(f, ForceMode.VelocityChange);
        // float randRot = 8f;
        // gunRb.AddTorque(new Vector3(Random.Range(-randRot, randRot), Random.Range(-randRot, randRot), Random.Range(-randRot, randRot)), ForceMode.VelocityChange);

        // gunComp.gameObject.SetActive(false);
    }

    #region Helper functions
    private void InitPlayer() {
        // if (init) {
        // 	// Initialize variables
        // 	init = false;

        // 	moveToNewLocation = false;
        // 	newLocation = transform.position;
        // }
    }

    public void PlayerStartGame() {
        // FindCurrentArea();
        // Find path
        if (!currentPath) {
            Debug.Log("PATH NOT DEFINED, LOOKING FOR ONE");
            GameObject[] pathObjects = GameObject.FindGameObjectsWithTag("Path");
            foreach (GameObject p in pathObjects) {
                if (p.transform.parent == null) {
                    currentPath = p.GetComponent<NavigationNode>();
                    break;
                }
            }
            GetPathSize();
        }
        currentNode = currentPath;
    }

    private void FindCurrentArea() {
        // Find current area
        currentArea = null;
        Collider[] _hits = Physics.OverlapBox(transform.position + Vector3.up, Vector3.one, transform.rotation);

        for (int i = 0; i < _hits.Length; i++) {
            if (_hits[i].CompareTag("Level Area")) {
                currentArea = _hits[i].GetComponent<LevelArea>();
                break;
            }
        }

        // Activate current area
        if (currentArea) {
            currentArea.ActivateLevelArea(this);
        } else {
            Debug.LogError("Current area not found");
        }

    }

    public void GoToNewLocation(Vector3 newLocPos) {
        moveToNewLocation = true;
        newLocation = newLocPos;

        agentComp.SetDestination(newLocation);
        timeComp.navMeshAgent.speed = moveSpeed;
    }
    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        // Debug.Log("HITTING" + hit.gameObject.name);
        // if (!agentComp.pathPending && agentComp.remainingDistance < 0.2f && !resetting) { // reached will reset soon
        // 	Debug.LogError("Game over - resetting in 4 seconds");
        // 	Invoke(nameof(gameOver), 4.0f);
        // }
    }

    void JumpStuff() {
        // Vector3 origin = new Vector3(3, 0, 12);
        // Vector3 target = new Vector3(8, 4, 24);
        // Vector2 hOffset = new Vector2(target.x - origin.x, target.z - origin.z);
        // Vector2 offset2D = new Vector2(hOffset.magnitude, target.y - origin.y);
        // float gravity = -Physics2D.gravity.y;
        // float distance = offset2D.x;
        // float time = distance / maxSpeed;
        // Vector2 vec = new Vector2(maxSpeed, offset2D.y / time + time * gravity / 2);

        // float vForcePerSecond = vec.y * 60f;
        // float timeToPeak = vForcePerSecond / gravity;
        // float peakHeight = 1 / 2 * Mathf.Pow(vForcePerSecond, 2f) / 9.81f;

        // float parabolaStart
        // y = x2
    }
}

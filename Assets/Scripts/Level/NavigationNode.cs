using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NavigationNode : MonoBehaviour {
    public enum Activities {
        Walk,
        Jump,
        Shoot,
        Save,
        Exit
    };
    public Activities NodeType = Activities.Walk;

    public bool BlendWithNext = false;
    public EnemyController[] Enemies;
    public bool focusOnEnemies = false;
    public HostageController[] Hostages;
    public DangerController[] Dangers;
    public bool Initialized = false;
    public bool activated = false;
    public float NodeTime = 0;
    public bool alignRotation = false;
    public NavigationNode TriggerNode;
    public NavigationNode Next;
    public NavigationNode Previous;

    [SerializeField] private bool copyThisNodeRotation = true;
    [SerializeField] bool AddNode = false; // editor var
    [SerializeField] bool RemoveNode = false; // editor var
    private Activities _type = Activities.Walk;

    static private Vector3 groundOffset = new Vector3(0, 0.2f, 0);
    void Start() {
        if (Next) {
            Next.Previous = this;
        }
    }

    public void ActivateNode(PlayerController player) {
        if (activated) return;
        if (TriggerNode) {
            TriggerNode.ActivateNode(player);
        }
        switch (NodeType) {
            case Activities.Jump:
                break;
            case Activities.Shoot:
                foreach (EnemyController e in Enemies) {
                    e.InitEnemy(player);
                }
                foreach (HostageController h in Hostages) {
                    h.InitHostage();
                }
                break;
            case Activities.Save:
                foreach (HostageController h in Hostages) {
                    h.InitHostage();
                }
                foreach (DangerController d in Dangers) {
                    d.InitDanger();
                }
                break;
            default:
                break;
        }
        activated = true;
    }

    public bool CheckStatus() {
        bool done = true;
        switch (NodeType) {
            case Activities.Jump:
                return false;
            case Activities.Shoot:
                foreach (EnemyController e in Enemies) {
                    done = done && !e.active;
                }
                foreach (HostageController h in Hostages) {
                    done = done && h.active;
                }
                if (done) {
                    foreach (HostageController h in Hostages) {
                        h.Celebrate();
                    }
                }
                return done;
            case Activities.Save:
                foreach (HostageController h in Hostages) {
                    done = done && h.active;
                }
                if (done) {
                    foreach (DangerController d in Dangers) {
                        done = done && d.Status();
                    }
                }
                if (done) {
                    foreach (HostageController h in Hostages) {
                        h.Celebrate();
                    }
                }
                return done;
            default:
                return false;
        }
    }

#if UNITY_EDITOR
    private void OnValidate() {
        if (AddNode) {
            // GameObject nodeObject = Instantiate(NodePrefab, transform.position, Quaternion.identity);

            GameObject go = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            GameObject nodeObject = PrefabUtility.InstantiatePrefab(go) as GameObject;
            NavigationNode navNode = nodeObject.GetComponent<NavigationNode>();
            // Set parent
            if (transform.parent != null && transform.parent.tag == "Path") {
                nodeObject.transform.SetParent(transform.parent);
                nodeObject.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            } else {
                nodeObject.transform.SetParent(transform);
                nodeObject.transform.SetSiblingIndex(0);
            }

            // Add as next or inbetween this and next.
            if (Next != null) { // add in between
                nodeObject.transform.position = Vector3.Lerp(transform.position, Next.transform.position, 0.5f);
                navNode.Next = Next;
                Next.Previous = navNode;
                Next = navNode;
                navNode.Previous = this;
            } else {
                nodeObject.transform.position = transform.position;
                Next = navNode;
                navNode.Previous = this;
            }

            if (navNode.Previous != null && copyThisNodeRotation) {
                nodeObject.transform.rotation = navNode.Previous.transform.rotation;
            }

            AddNode = false;
            Selection.activeGameObject = navNode.gameObject;
            return;
        }

        if (RemoveNode) {
            if (Previous == null) {
                RemoveNode = false;
                if (Next) {
                    Selection.activeGameObject = Next.gameObject;
                }
                return;
            }
            if (Next) {
                Next.Previous = Previous;
            }
            Previous.Next = Next;
            Selection.activeGameObject = Previous.gameObject;
            UnityEditor.EditorApplication.delayCall += () => {
                UnityEditor.Undo.DestroyObjectImmediate(gameObject);
            };
            RemoveNode = false;
            return;
        }

        if (_type != NodeType) {
            switch (NodeType) {
                case Activities.Walk:
                    gameObject.name = "Path";
                    break;
                case Activities.Jump:
                    gameObject.name = "PathJump";
                    break;
                case Activities.Shoot:
                    gameObject.name = "PathShoot";
                    break;
                case Activities.Save:
                    gameObject.name = "PathSave";
                    break;
                case Activities.Exit:
                    gameObject.name = "PathExit";
                    break;
                default:
                    NodeType = Activities.Walk;
                    gameObject.name = "Path";
                    break;
            }
            _type = NodeType;
            return;
        }
    }

    void OnDrawGizmos() {
        switch (NodeType) {
            case Activities.Jump:
                Gizmos.color = Color.yellow;
                break;
            case Activities.Shoot:
                Gizmos.color = Color.red;
                break;
            case Activities.Save:
                Gizmos.color = Color.green;
                break;
            case Activities.Exit:
                Gizmos.color = Color.black;
                break;
            default:
                Gizmos.color = Color.white;
                break;
        }
        Gizmos.DrawSphere(transform.position + groundOffset, 0.4f);
        if (Next != null) {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position + groundOffset, Next.transform.position + groundOffset);
        }
    }
    void OnDrawGizmosSelected() {
        // Draw selected gizmos
        Vector3 humanOffset = new Vector3(0, 3f, 0);
        switch (NodeType) {
            case Activities.Shoot:
                Gizmos.color = Color.red;
                foreach (EnemyController e in Enemies) {
                    Gizmos.DrawLine(transform.position + groundOffset, e.transform.position + humanOffset);
                }
                break;
            case Activities.Save:
                Gizmos.color = Color.green;
                foreach (HostageController h in Hostages) {
                    Gizmos.DrawLine(transform.position + groundOffset, h.transform.position + humanOffset);
                }
                Gizmos.color = Color.red;
                foreach (DangerController d in Dangers) {
                    Gizmos.DrawLine(transform.position + groundOffset, d.transform.position + groundOffset);
                }
                break;
            case Activities.Jump:
                if (Next != null) {
                    Gizmos.color = Color.black;
                    Vector3 origin = transform.position;
                    Vector3 target = Next.transform.position;
                    Vector3 offset = target - origin;
                    Quaternion rotation = Quaternion.LookRotation(new Vector3(offset.x, 0, offset.z));
                    Vector2 hOffset = new Vector2(offset.x, offset.z);
                    Vector2 offset2D = new Vector2(hOffset.magnitude, target.y - origin.y);
                    float maxSpeed = PlayerController.maxSpeed;
                    float gravity = -Physics2D.gravity.y * 3;
                    float distance = offset2D.x;
                    float time = distance / maxSpeed / 60f;
                    Vector2 vec = new Vector2(maxSpeed, offset2D.y / time + time * gravity / 2);

                    // float vForcePerSecond = vec.y;
                    float timeToPeak = vec.y * 60f / gravity;
                    // Debug.Log(time + "in seconds");
                    // Debug.Log(vec + "in seconds");
                    // Debug.Log(vec + "in seconds");
                    // float peakHeight = 1f / 2f * Mathf.Pow(vec.y, 2f) / Mathf.Pow(gravity, 2f);
                    // float peakHeight = Mathf.Pow(vec.y * 60f / (2 * gravity), 2f);
                    // float peakHeight = vec.y * timeToPeak - (0.5f * (gravity * (timeToPeak * timeToPeak)));
                    float peakHeight = 0.5f * Mathf.Pow(vec.y, 2f) / gravity;
                    // float peakHeight = 0;
                    // float t = timeToPeak;
                    // float f = vec.y;
                    // while (t > 0) {
                    //     peakHeight += f;
                    //     f -= gravity / 60f;
                    //     t -= 1 / 60;
                    // }

                    // Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight)
                    // Debug.Log(peakHeight + "jumpheight");


                    float fullParaboleDistance = timeToPeak * maxSpeed;// * 60f;
                    float paraboleStart = 0;
                    float paraboleEnd = 1;

                    if (fullParaboleDistance < 0) { // the parabole is already in downwards motion (this is probably an illegal jump)
                        float realParabole = (-fullParaboleDistance + distance) * 2;
                        paraboleStart = (realParabole - fullParaboleDistance) / realParabole;
                        fullParaboleDistance = realParabole;
                    } else {
                        float realParabole = fullParaboleDistance * 2;
                        Debug.Log(realParabole + "," + distance);
                        paraboleEnd = distance / realParabole;
                        fullParaboleDistance = realParabole;
                    }
                    float paraboleLength = paraboleEnd - paraboleStart;

                    float doParabole(float v) {
                        return 1 - (Mathf.Pow(v * 2 - 1, 2));
                    }

                    // draw points across the parabole path that the jump will take
                    int accuracy = 20;
                    float pathPiece = 1 / accuracy;
                    float parabolePiece = paraboleLength / accuracy;
                    Vector3 previous = new Vector3();
                    Vector3 b = new Vector3();
                    for (int i = 0; i < accuracy; i++) {
                        b.z = (i + 1) * parabolePiece;
                        b.y = doParabole(paraboleStart + b.z) * peakHeight;
                        b.z *= fullParaboleDistance;
                        Gizmos.DrawLine(origin + rotation * previous, origin + rotation * b);
                        previous.x = b.x;
                        previous.y = b.y;
                        previous.z = b.z;
                    }
                }
                break;
        }
    }
#endif
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Chronos;
using LuckyKat;

public class HostageController : MonoBehaviour {
    // Variables
    [SerializeField]
    private Timeline timeComp;
    [SerializeField]
    private Animator animComp;
    [SerializeField]
    private Collider colliderComp;
    [SerializeField]
    private GameObject[] characterBodyParts;
    [SerializeField]
    private Renderer rendererComp;

    [SerializeField]
    private float moveSpeed = 10.0f;

    public bool active = false;
    public bool celebrating = false;
    private LevelArea currentArea;

    [SerializeField]
    private GameObject[] screams;
    private float screamTimer;

    [SerializeField]
    private GameObject dieFx;

    [SerializeField]
    private Material happyFace;


    // Start is called before the first frame update
    private void Start() {
        //
    }

    // Update is called once per frame
    private void Update() {
        if (active && !celebrating) {
            // 	if (!GameController.instance.HasGameStarted() || GameController.instance.IsGameOver()) {
            // 		timeComp.navMeshAgent.speed = 0;
            // 		animComp.SetBool("move", false);

            // 		return;
            screamTimer = Mathf.MoveTowards(screamTimer, 0, Time.deltaTime);
            if (screamTimer == 0) {
                if (screams.Length == 0) {
                    return;
                }
                //pick a scream
                GameObject scream = screams[Random.Range(0, screams.Length)];
                Instantiate(scream, transform.position + transform.up * 8f, new Quaternion());
                screamTimer = Random.Range(2f, 4f);
            }
        }

        // 	// Chase player
        // 	if (playerTarget) {
        // 		agentComp.SetDestination(playerTarget.transform.position);
        // 		timeComp.navMeshAgent.speed = moveSpeed;
        // 		animComp.SetBool("move", true);
        // 	} else {
        // 		timeComp.navMeshAgent.speed = 0;
        // 		animComp.SetBool("move", false);
        // 	}
        // }

        // TODO: Should say help and such
    }

    #region Helper functions
    public void InitHostage() {
        active = true;
        // currentArea = newArea;

        UpdateBodyParts(false);

        // currentArea.IncreaseEnemyCount();
        // TODO: Tell current area about hostages
    }

    public void Celebrate() {
        if (!celebrating && active) {
            animComp.SetTrigger("celebrate");
            animComp.SetFloat("cType", (float)Random.Range(0, 2));

            if (happyFace != null) {
                Material[] m = rendererComp.materials;
                m[1] = happyFace;
                rendererComp.materials = m;
            }
            celebrating = true;
        }
    }

    public void CharacterHit() {
        if (!active && !celebrating) {
            return;
        }

        // Instantiate(dieFx, transform.position + transform.up * 8f, new Quaternion());

        // Debug.LogError("Game over - resetting in 2 seconds");
        // GameController.instance.Invoke(nameof(GameController.instance.RestartGame), 4.0f);
        if (!GameController.instance.gameOver) { // walk straight towards player
            Debug.LogError("Game over - show fail screen");
            // GameController.instance.Fail();
            GameController.instance.Invoke(nameof(GameController.instance.Fail), 2.0f);
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

        // Activate physics
        UpdateBodyParts(true);

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
}

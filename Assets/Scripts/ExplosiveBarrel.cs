using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour {
    public NailAction nailAction;
    public GameObject SmokePrefab;
    private float explosionRadius = 10f;
    private Vector3 explosionCenter;
    // Start is called before the first frame update
    void Start() {
        nailAction.OnHit = delegate () {
            Explode();
        };
        explosionCenter = transform.position + Vector3.up * 3;
    }

    void Explode() {
        Destroy(gameObject);
        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, explosionRadius);
        void pushObject(Collider col) {
            Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = false;
                rb.AddExplosionForce(10f, explosionCenter, explosionRadius, 4f, ForceMode.VelocityChange);
            }

            NailAction action = col.gameObject.GetComponent<NailAction>();
            if (action && action.Active) {
                action.DoHit();
            }
        }
        //Commenting out reference to the deleted Vibration asset -VMG
        //Vibration.VibrateNope();
        nailAction.Active = false;
        foreach (var hitCollider in hitColliders) {
            switch (hitCollider.gameObject.tag) {
                case "Character Body Part":
                    GameObject character = hitCollider.GetComponent<CharacterParent>().GetCharacter();
                    if (character.CompareTag("Enemy")) {
                        EnemyController _enemyController = character.GetComponent<EnemyController>();
                        if (_enemyController.IsCharacterAlive()) {
                            _enemyController.CharacterHit(transform.forward);
                        }
                    } else if (character.CompareTag("Civilian")) {
                        HostageController _hostageController = character.GetComponent<HostageController>();
                        if (_hostageController.IsCharacterAlive()) {
                            _hostageController.CharacterHit();
                        }
                    }
                    pushObject(hitCollider);
                    break;
                case "Actionable":
                    if (hitCollider.gameObject == nailAction.gameObject) continue;
                    pushObject(hitCollider);
                    Debug.Log("HIT" + hitCollider.gameObject.name);
                    break;
            }
        }
        if (SmokePrefab != null) {
            Vector3 spawnPosition = transform.position + transform.up * 0.4f;
            for (int i = 0; i < 24; i++) {
                GameObject p = Instantiate(SmokePrefab, spawnPosition, Quaternion.identity);
                if (i >= 20) {
                    p.GetComponent<ExplosionParticle>().upwards = true;
                }
            }
        }
        // cutAction.OnCut = null;
        SoundPlayer.instance.play("explosion", Random.Range(0.9f, 1.1f));
    }
    private void OnCollisionEnter(Collision other) {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.velocity.magnitude > 6f) {
            Explode();
        }
    }
    // // Update is called once per frame
    // void Update()
    // {

    // }
#if UNITY_EDITOR
    Color radiusGizmoColor = new Color(1, 0, 0, 0.3f);
    void OnDrawGizmosSelected() {
        // Draw selected gizmos
        Vector3 groundOffset = new Vector3(0, 3f, 0);
        Gizmos.color = radiusGizmoColor;
        Gizmos.DrawSphere(transform.position + groundOffset, explosionRadius);
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyArmor : MonoBehaviour {
    public bool active = true;
    public int health = 1;
    [SerializeField] private GameObject armor;
    [SerializeField] private GameObject[] pieces;
    [SerializeField] private EnemyArmor[] bodyTargets;
    [SerializeField] private int spawnDebris = 8;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private Image healthBarImage;

    public bool DamageArmor(int damage = 1) {
        if (!active) return true;
        health -= damage;
        if (health <= 0) {
            damage = 0;
            DestroyArmor();
            active = false;
            if (bodyTargets.Length > 0) {
                foreach (EnemyArmor bodypart in bodyTargets) {
                    bodypart.active = false;
                    Destroy(bodypart);
                }
            }
            return true;
        } else if (bodyTargets.Length > 0) {
            foreach (EnemyArmor bodypart in bodyTargets) {
                bodypart.health = health;
            }
        }
        return false;
    }
    public void DestroyArmor() {
        if (!active) return;
        if (armor != null) { // armor is different from gameobject
            armor.SetActive(false);
        } else {
            gameObject.SetActive(false);
        }
        if (pieces.Length > 0) { // predefined pieces 
            foreach (GameObject debris in pieces) {
                debris.SetActive(true);
                if (debris.transform.parent.gameObject.activeSelf == false) {
                    debris.transform.parent.gameObject.SetActive(true);
                }
                Rigidbody rb = debris.GetComponent<Rigidbody>();
                if (rb) {
                    rb.AddExplosionForce(4f, transform.position, 6, 0, ForceMode.VelocityChange);
                }
            }
        } else if (debrisPrefab != null) { // use prefabs
            while (spawnDebris-- > 0) {
                GameObject debris = Instantiate(debrisPrefab, transform.position + Random.insideUnitSphere, Quaternion.Euler(Random.value * 360f, Random.value * 360f, Random.value * 360f));
                Rigidbody rb = debris.GetComponent<Rigidbody>();
                if (rb) {
                    rb.AddExplosionForce(4f, transform.position, 6, 0, ForceMode.VelocityChange);
                } else {
                    // maybe children with rb?
                    Rigidbody[] rbList = debris.GetComponentsInChildren<Rigidbody>();
                    if (rbList != null && rbList.Length > 0) {
                        foreach (Rigidbody childRb in rbList) {
                            rb.AddExplosionForce(4f, transform.position, 6, 0, ForceMode.VelocityChange);
                        }
                    }
                    // rb = debris.AddComponent<Rigidbody>();
                    // rb.useGravity = true;
                    // rb.AddExplosionForce(4f, transform.position, 6, 0, ForceMode.VelocityChange);
                }
            }
        }
        Destroy(this);
    }

    // // Start is called before the first frame update

    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}

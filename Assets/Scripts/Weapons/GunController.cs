using System.Collections.Generic;
using UnityEngine;
using Chronos;
using LuckyKat;

public class GunController : MonoBehaviour {
    // Variables
    [SerializeField]
    private TimelineChild timeComp;
    [SerializeField]
    private Animator animComp;
    [SerializeField]
    private GameObject colliderObject;
    [SerializeField]
    private GameObject gunObject;
    private Vector3 gunPosition;
    private Quaternion gunRotation;
    private Quaternion gunFiredRotation;
    private Tween fireTween;
    [SerializeField]
    private GameObject hand;
    [SerializeField]
    private Transform muzzle;
    [SerializeField]
    private ParticleSystem psComp;

    private bool canShoot = true;
    private bool dead = false;
    private float timer;
    [SerializeField]
    // private float shootResetTime = 0.5f;
    private float shootResetTime = 0.2f;

    // Start is called before the first frame update
    void Start() {
        if (animComp == null) {
            if (gunObject) {
                gunPosition = gunObject.transform.localPosition;
                gunRotation = gunObject.transform.localRotation;
                gunFiredRotation = gunObject.transform.localRotation * Quaternion.Euler(30, 0, 0);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        if (!canShoot) {
            if (timer > 0) {
                timer -= timeComp.parent.deltaTime;
            } else {
                timer = 0;
                canShoot = true;
            }
        }
    }

    public Transform GetGunMuzzle() {
        return muzzle;
    }

    public bool CanShoot() {
        return canShoot;
    }

    public void Shoot() {
        if (dead) return;
        // if (canShoot) {
        if (animComp != null) {
            animComp.SetTrigger("shoot");
        } else if (gunObject != null) {
            Vector3 gPos = gunPosition;
            Quaternion gRot = gunRotation;
            if (fireTween != null) {
                gPos = gunObject.transform.localPosition;
                gRot = gunObject.transform.localRotation;
                fireTween.Stop();
            }
            fireTween = new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.05f).SetOnUpdate((float v, float t) => {
                gunObject.transform.localPosition = Vector3.Lerp(gPos, gunPosition + new Vector3(0, 0, -1.2f), v);
                gunObject.transform.localRotation = Quaternion.Lerp(gunRotation, gunFiredRotation, v);
            }).SetOnComplete(() => {
                fireTween = new Tween().SetEase(Tween.Ease.InOutQuad).SetTime(0.032f).SetOnUpdate((float v, float t) => {
                    gunObject.transform.localPosition = gunPosition + new Vector3(0, 0, -1.2f + 0.9f * v);
                    gunObject.transform.localRotation = Quaternion.Lerp(gunFiredRotation, gunRotation, v);
                }).SetOnComplete(() => {
                    fireTween = new Tween().SetEase(Tween.Ease.InOutQuad).SetStart(-0.3f).SetEnd(-0.5f).SetTime(0.018f).SetOnUpdate((float v, float t) => {
                        gunObject.transform.localPosition = gunPosition + new Vector3(0, 0, v);
                    }).SetOnComplete(() => {
                        fireTween = new Tween().SetEase(Tween.Ease.InOutQuad).SetStart(-0.5f).SetEnd(0f).SetTime(0.15f).SetOnUpdate((float v, float t) => {
                            gunObject.transform.localPosition = gunPosition + new Vector3(0, 0, v);
                        }).SetOnComplete(() => {
                            fireTween = null;
                        });
                    });
                });
            });
        }
        psComp.Play();
        canShoot = false;
        timer = shootResetTime;
        // }
    }

    public void Die() {
        if (dead) return;
        dead = true;
        colliderObject.SetActive(true);
        hand.SetActive(false);
        Rigidbody gunRb = gameObject.AddComponent<Rigidbody>();
        Vector3 f = transform.rotation * new Vector3(Random.Range(-3f, 3f), Random.Range(10, 13), -Random.Range(2f, 3f));
        gunRb.AddForce(f, ForceMode.VelocityChange);
        float randRot = 8f;
        gunRb.AddTorque(new Vector3(Random.Range(-randRot, randRot), Random.Range(-randRot, randRot), Random.Range(-randRot, randRot)), ForceMode.VelocityChange);
    }
}

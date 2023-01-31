using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuckyKat;

public class MafiaAccessory : MonoBehaviour {
    [SerializeField]
    private GameObject[] accesories;
    [SerializeField]
    private GameObject currentObject;
    [SerializeField]
    private Collider currentCollider;
    // Start is called before the first frame update
    public Rigidbody thisRigidbody;
    void Start() {
        if (accesories != null && accesories.Length > 0) {
            currentObject = accesories[Random.Range(0, accesories.Length)];
            currentCollider = currentObject.GetComponent<Collider>();
            currentObject.SetActive(true);
        }
    }
    public void EnablePhysics() {
        thisRigidbody = gameObject.AddComponent<Rigidbody>();
        thisRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        currentCollider.enabled = true;
    }

    public void GreyOut() {
        Renderer rendererComp = currentObject.GetComponent<Renderer>();
        // Update color
        new Tween().SetEase(Tween.Ease.OutCubic).SetDelay(0.3f).SetTime(0.5f).SetStart(1.0f).SetEnd(0.01f).SetOnUpdate((float v, float t) => {
            MaterialPropertyBlock _customMaterial = new MaterialPropertyBlock();
            rendererComp.GetPropertyBlock(_customMaterial);
            _customMaterial.SetFloat("greyout", v);
            rendererComp.SetPropertyBlock(_customMaterial);
        }).SetOnComplete(() => {
            MaterialPropertyBlock _customMaterial = new MaterialPropertyBlock();
            rendererComp.GetPropertyBlock(_customMaterial);
            _customMaterial.SetFloat("greyout", 0.08f);
            rendererComp.SetPropertyBlock(_customMaterial);
        });
    }

    public void ScaleDown() {
        // Update scale
        Vector3 originalScale = transform.localScale;
        new Tween().SetEase(Tween.Ease.OutCubic).SetDelay(1.3f).SetTime(0.5f).SetStart(1.0f).SetEnd(0.01f).SetOnUpdate((float v, float t) => {
            transform.localScale = originalScale * v;
        }).SetOnComplete(() => {
            MaterialPropertyBlock _customMaterial = new MaterialPropertyBlock();
            Destroy(gameObject);
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuckyKat;

public class DebrisDisappear : MonoBehaviour {
    [SerializeField]
    private GameObject currentObject;
    [SerializeField]
    private bool grey = true;
    // Start is called before the first frame update
    void Start() {
        if (currentObject == null) {
            currentObject = gameObject;
        }
        transform.parent = null;
        GreyOut();
        ScaleDown();
    }

    public void GreyOut() {
        if (!grey) return;
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

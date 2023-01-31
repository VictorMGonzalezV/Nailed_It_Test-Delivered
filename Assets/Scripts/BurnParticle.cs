using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnParticle : MonoBehaviour
{
    public int originalId; 
    private Color col;
    private MaterialPropertyBlock pBlock;
    private Renderer burnRenderer;
    public float totalTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        burnRenderer = GetComponent<Renderer>();
        pBlock = new MaterialPropertyBlock();
        if (burnRenderer) {
            // Debug.Log(pBlock.GetColor("Color"));
            // Debug.Log(pBlock.GetColor("Base Color"));
            // Debug.Log(pBlock.GetColor("BaseColor"));
            // Debug.Log(pBlock.GetColor("_Color"));
            // Debug.Log(pBlock.GetColor("_BaseColor"));
            // Debug.Log(pBlock.GetColor("_BaseMap"));
            col = new Color(1f, 0f, 0f, 1f);
            if (originalId != gameObject.GetInstanceID()) {
                if (transform.parent != null) {
                    Collider pCol = transform.parent.GetComponent<MeshCollider>();
                    if (pCol != null) {
                        // Ray r = new Ray(transform.position, -transform.forward);
                        // RaycastHit hit;
                        // if (pCol.Raycast(r, out hit, 0.04f)){

                        Vector3 closestPoint = pCol.ClosestPoint(transform.position);
                        Vector3 difference = closestPoint - transform.position;
                        if (difference.magnitude < 0.04){
                            col.r = 0f;
                        } else {
                            // Debug.Log("RAYCASTNOHIT");
                            DestroyImmediate(gameObject);
                            return;
                        }
                    } else {
                        DestroyImmediate(gameObject);
                        return;
                    }
                } else {
                    DestroyImmediate(gameObject);
                    return;
                }
            }
            pBlock.SetColor("_BaseColor", col);
            pBlock.SetColor("_EmissionColor", col);
            burnRenderer.SetPropertyBlock(pBlock);
        }

        // transform.rotation = Quaternion.LookRotation(-Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
		totalTime += Time.deltaTime;
        if (totalTime > 2f) {
            Destroy(gameObject);
        } else if (burnRenderer) {
            col.r = Tween(1f, 0f, totalTime, 0.2f, 0.2f);
            col.a = Tween(1f, 0f, totalTime, 1f, 1f);
            // Debug.Log(col + "," + Mathf.Clamp(0f, ((time - delay) / endTime), 1f) + "," + Mathf.Clamp(0f, ((time - delay) / endTime));
            pBlock.SetColor("_BaseColor", col);
            pBlock.SetColor("_EmissionColor", col);
            burnRenderer.SetPropertyBlock(pBlock);
        }
    }

    float Tween(float startValue, float endValue, float time, float delay, float endTime) {
		// Increment time

		// Do nothing until the delay is passed
		if ((time - delay) < 0f) {
			return startValue;
        }

		// Get time and value
		float t = (time - delay);
		float timePercent = Mathf.Clamp((t / endTime), 0f, 1f);
		float deltaVal = endValue - startValue;
        return deltaVal * (timePercent < 0.5f ? 2f * timePercent * timePercent : 1f - Mathf.Pow(-2f * timePercent + 2f, 2f) / 2f) + startValue;
    }
}

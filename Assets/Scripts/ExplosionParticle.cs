using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticle : MonoBehaviour {
    private Color col;
    private static Color BlackColor = new Color(0, 0, 0);
    private static Color SmokeColor = new Color(0.34f, 0.34f, 0.34f);
    private static Color WhiteColor = new Color(1f, 1f, 1f);
    private static Color YellowColor = new Color(1f, 1f, 0);
    private static Color OrangeColor = new Color(1f, 0.55f, 0);
    private static Color RedColor = new Color(1f, 0.22f, 0);
    private MaterialPropertyBlock pBlock;
    private Renderer explosionRenderer;
    public float totalTime = 0f;
    public bool upwards = false;
    private Vector3 scale;
    private Vector3 direction;
    private Vector3 antiGravity = new Vector3(0, 0.02f, 0);
    private float speed;
    // Start is called before the first frame update
    void Start() {
        explosionRenderer = GetComponent<Renderer>();
        pBlock = new MaterialPropertyBlock();
        if (explosionRenderer) {
            pBlock.SetColor("_DiffColor", BlackColor);
            pBlock.SetColor("_EmissionColor", BlackColor);
            pBlock.SetColor("_FresnelColor", WhiteColor);
            pBlock.SetFloat("_FresnelStrength", 0.2f);
            explosionRenderer.SetPropertyBlock(pBlock);
        }
        scale = transform.localScale;
        scale.x = Random.Range(1.0f, 1.3f) * 2.4f;
        scale.y = Random.Range(1.0f, 1.3f) * 2.4f;
        scale.z = Random.Range(1.0f, 1.3f) * 2.4f;
        transform.localScale = scale * 1.5f;
        transform.rotation = Quaternion.Euler(Random.value * 360f, Random.value * 360f, Random.value * 360f);
        speed = Random.Range(0.36f, 0.47f) * 2.4f;
        if (upwards == false) {
            direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.2f, 0.2f), Random.Range(-1f, 1f)).normalized;
        } else {
            direction = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(1.0f, 2.0f), Random.Range(-0.2f, 0.2f)).normalized;
            speed *= 0.3f;
        }
        transform.position = transform.position + direction * 0.7f;
    }

    // Update is called once per frame
    void Update() {
        totalTime += Time.deltaTime;
        float frameTime = Time.deltaTime * 60f;
        if (explosionRenderer) {
            if (totalTime < 0.02f) {
            } else if (totalTime < 0.1f) {
                // fresnelcolor
                pBlock.SetColor("_EmissionColor", WhiteColor);
                col.r = Tween(WhiteColor.r, YellowColor.r, totalTime, 0.02f, 0.08f, "l");
                col.g = Tween(WhiteColor.g, YellowColor.g, totalTime, 0.02f, 0.08f, "l");
                col.b = Tween(WhiteColor.b, YellowColor.b, totalTime, 0.02f, 0.08f, "l");
                pBlock.SetColor("_FresnelColor", col);
                pBlock.SetFloat("_FresnelStrength", 1f);
                transform.localScale = scale * Tween(1.5f, 2.2f, totalTime, 0.02f, 0.08f, "out");
            } else if (totalTime < 0.2f) {
                // emissioncolor
                col.r = Tween(WhiteColor.r, YellowColor.r, totalTime, 0.1f, 0.1f, "l");
                col.g = Tween(WhiteColor.g, YellowColor.g, totalTime, 0.1f, 0.1f, "l");
                col.b = Tween(WhiteColor.b, YellowColor.b, totalTime, 0.1f, 0.1f, "l");
                pBlock.SetColor("_EmissionColor", col);
                // fresnelcolor
                col.r = Tween(YellowColor.r, OrangeColor.r, totalTime, 0.1f, 0.1f, "l");
                col.g = Tween(YellowColor.g, OrangeColor.g, totalTime, 0.1f, 0.1f, "l");
                col.b = Tween(YellowColor.b, OrangeColor.b, totalTime, 0.1f, 0.1f, "l");
                pBlock.SetColor("_FresnelColor", col);
                transform.localScale = scale * Tween(2.2f, 1f, totalTime, 0.1f, 0.5f, "inout");
            } else if (totalTime < 0.3f) {
                // emissioncolor
                col.r = Tween(YellowColor.r, OrangeColor.r, totalTime, 0.2f, 0.1f, "l");
                col.g = Tween(YellowColor.g, OrangeColor.g, totalTime, 0.2f, 0.1f, "l");
                col.b = Tween(YellowColor.b, OrangeColor.b, totalTime, 0.2f, 0.1f, "l");
                pBlock.SetColor("_EmissionColor", col);
                // fresnelcolor
                col.r = Tween(OrangeColor.r, RedColor.r, totalTime, 0.2f, 0.1f, "l");
                col.g = Tween(OrangeColor.g, RedColor.g, totalTime, 0.2f, 0.1f, "l");
                col.b = Tween(OrangeColor.b, RedColor.b, totalTime, 0.2f, 0.1f, "l");
                pBlock.SetColor("_FresnelColor", col);
                transform.localScale = scale * Tween(2.2f, 1f, totalTime, 0.1f, 0.5f, "inout");
            } else if (totalTime < 0.4f) {
                // emissioncolor
                col.r = Tween(OrangeColor.r, RedColor.r, totalTime, 0.3f, 0.1f, "l");
                col.g = Tween(OrangeColor.g, RedColor.g, totalTime, 0.3f, 0.1f, "l");
                col.b = Tween(OrangeColor.b, RedColor.b, totalTime, 0.3f, 0.1f, "l");
                pBlock.SetColor("_EmissionColor", col);
                // fresnecolor
                col.r = Tween(RedColor.r, BlackColor.r, totalTime, 0.3f, 0.1f, "l");
                col.g = Tween(RedColor.g, BlackColor.g, totalTime, 0.3f, 0.1f, "l");
                col.b = Tween(RedColor.b, BlackColor.b, totalTime, 0.3f, 0.1f, "l");
                pBlock.SetColor("_FresnelColor", col);
                // pBlock.SetFloat("_FresnelStrength", Tween(1.0f, 0f, totalTime, 0.6f, 0.8f, "out"));
                transform.localScale = scale * Tween(2.2f, 1f, totalTime, 0.1f, 0.5f, "inout");
            } else if (totalTime < 0.5f) {
                // emissioncolor
                col.r = Tween(RedColor.r, BlackColor.r, totalTime, 0.4f, 0.2f, "l");
                col.g = Tween(RedColor.g, BlackColor.g, totalTime, 0.4f, 0.2f, "l");
                col.b = Tween(RedColor.b, BlackColor.b, totalTime, 0.4f, 0.2f, "l");
                pBlock.SetColor("_EmissionColor", col);
                pBlock.SetFloat("_FresnelStrength", Tween(1.0f, 2f, totalTime, 0.4f, 0.1f, "in"));
                transform.localScale = scale * Tween(2.2f, 1f, totalTime, 0.1f, 0.5f, "inout");
            } else if (totalTime < 0.6f) {
                // emissioncolor
                col.r = Tween(RedColor.r, BlackColor.r, totalTime, 0.4f, 0.2f, "l");
                col.g = Tween(RedColor.g, BlackColor.g, totalTime, 0.4f, 0.2f, "l");
                col.b = Tween(RedColor.b, BlackColor.b, totalTime, 0.4f, 0.2f, "l");
                pBlock.SetColor("_EmissionColor", col);
                // diffcolor
                col.r = Tween(BlackColor.r, SmokeColor.r, totalTime, 0.4f, 0.2f, "inout");
                col.g = Tween(BlackColor.g, SmokeColor.g, totalTime, 0.4f, 0.2f, "inout");
                col.b = Tween(BlackColor.b, SmokeColor.b, totalTime, 0.4f, 0.2f, "inout");
                pBlock.SetColor("_DiffColor", col);
                transform.localScale = scale * Tween(2.2f, 1f, totalTime, 0.1f, 0.5f, "inout");
            } else if (totalTime < 1.1f) {
                transform.localScale = scale * Tween(1f, 0.001f, totalTime, 0.6f, 0.5f, "in");
            } else {
                Destroy(gameObject);
            }
            explosionRenderer.SetPropertyBlock(pBlock);
        }
        transform.position = transform.position + direction * speed * frameTime + antiGravity * frameTime;
        speed -= (speed - speed * 0.89f) * frameTime;
        direction = (direction + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) * frameTime).normalized;
    }

    float Tween(float startValue, float endValue, float time, float delay, float endTime, string type) {
        // Increment time

        // Do nothing until the delay is passed
        if ((time - delay) < 0f) {
            return startValue;
        }

        // Get time and value
        float t = (time - delay);
        float timePercent = Mathf.Clamp((t / endTime), 0f, 1f);
        float deltaVal = endValue - startValue;
        // return deltaVal * (timePercent < 0.5f ? 2f * timePercent * timePercent : 1f - Mathf.Pow(-2f * timePercent + 2f, 2f) / 2f) + startValue;

        switch (type) {
            case "in": // quad in
                return deltaVal * timePercent * timePercent + startValue;
            case "out": // quad out
                return deltaVal * (1f - (1f - timePercent) * (1f - timePercent)) + startValue;
            case "inout": // quad in out
                return deltaVal * (timePercent < 0.5f ? 2f * timePercent * timePercent : 1f - Mathf.Pow(-2f * timePercent + 2f, 2f) / 2f) + startValue;
            default: // linear
                return deltaVal * timePercent + startValue;
        }
    }
}

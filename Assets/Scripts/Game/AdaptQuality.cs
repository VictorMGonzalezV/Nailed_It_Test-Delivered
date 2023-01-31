using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptQuality : MonoBehaviour {
    float averageFPS = 0;
    int qualityCheck = 0;
    int maxQualityCheck = 5;
    int warmup = 5;
    //UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urp;
    // Start is called before the first frame update
    void Start() {
        var rpAsset = QualitySettings.renderPipeline;
        //urp = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)rpAsset;
    }

    // Update is called once per frame
    void Update() {
        averageFPS *= qualityCheck;
        averageFPS += 1f / Time.unscaledDeltaTime;
        qualityCheck++;
        averageFPS /= qualityCheck;
        if (qualityCheck == maxQualityCheck) {
            if (warmup > 0) {
                if (averageFPS <= 58) {
                    warmup--;
                } else {
                    warmup = Mathf.Min(warmup + 1, 5);
                }
                //urp.renderScale = 1f;
            } else {
                //Deleted reference to urp variable in the if statement so the URP package can be removed -VMG
                if (averageFPS >= 58) {
                    warmup++;
                } else {
                    warmup = Mathf.Max(warmup + 1, -5);
                }
                //urp.renderScale = Mathf.Clamp(urp.renderScale * (averageFPS / 60f), 0.5f, 1f);
            }
            averageFPS = 0f;
            qualityCheck = 0;
        }
    }
    private void OnDestroy() {
#if UNITY_EDITOR
        //urp.renderScale = 1f;
#endif
    }
}

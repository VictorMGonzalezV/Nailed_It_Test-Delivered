using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LuckyKat;

public class LoseController : MonoBehaviour {
    [SerializeField] private Image overlay;
    [SerializeField] private RectTransform FailTextTransform;
    [SerializeField] private RectTransform RetryTransform;
    [SerializeField] private Button RetryButton;
    [SerializeField] private AnimationCurve ElasticCurve;
    private void OnEnable() {
        // show background
        Color backgroundCol = overlay.color;
        backgroundCol.a = 0f;
        new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.35f).SetStart(0f).SetEnd(0.9f).SetOnUpdate((float v, float t) => {
            backgroundCol.a = v;
            overlay.color = backgroundCol;
        });
        // show fail
        FailTextTransform.localScale = new Vector3(1, 0, 1);
        new Tween().SetCustomEase(ElasticCurve).SetEase(Tween.Ease.Custom).SetDelay(1.1f).SetTime(0.4f).SetStart(0).SetEnd(1).SetOnUpdate((float v, float t) => {
            FailTextTransform.localScale = new Vector3(1, v, 1);
        }).SetOnComplete(() => {
            FailTextTransform.localScale = new Vector3(1, 1, 1);
        });
        // show button
        RetryTransform.localScale = new Vector3(0, 0, 1);
        RetryButton.gameObject.SetActive(false);
        new Tween().SetCustomEase(ElasticCurve).SetEase(Tween.Ease.Custom).SetDelay(1.7f).SetTime(0.7f).SetStart(0).SetEnd(1).SetOnStart(() => {
            RetryButton.gameObject.SetActive(true);
            RetryButton.enabled = true;
        }).SetOnUpdate((float v, float t) => {
            RetryTransform.localScale = new Vector3(v, v, 1);
        }).SetOnComplete(() => {
            RetryTransform.localScale = new Vector3(1, 1, 1);
        });
    }

    // Update is called once per frame
    void Update() {
    }

    public void RetryLevel() {
        RetryButton.enabled = false;
        GameController.instance.RestartGame();
        gameObject.SetActive(false);
        SoundPlayer.instance.play("mousedown");
    }
}

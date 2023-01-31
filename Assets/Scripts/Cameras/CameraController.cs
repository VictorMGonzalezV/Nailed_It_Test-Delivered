using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Rendering.Universal;
using LuckyKat;

public class CameraController : MonoBehaviour {
    // Variables
    public static CameraController instance;

    private float screenShakeTimer;

    private Vector3 startPos;
    private Quaternion startRot;
    private GameObject _focusTarget;
    private Quaternion focusRootRot;
    private Tween focusTween;
    private float onTarget = 1f;
    public bool active = true;
    public GameObject focusTarget {
        get { return _focusTarget; }
        set {
            if (value != _focusTarget) {
                onTarget = 0;
                focusRootRot = transform.rotation * Quaternion.Inverse(startRot);
                focusTween = new Tween().SetEase(Tween.Ease.OutQuad).SetTime(0.3f).SetOnUpdate((float v, float t) => {
                    onTarget = v;
                }).SetOnComplete(() => {
                    onTarget = 1f;
                    focusTween = null;
                });
            }
            _focusTarget = value;
        }
    }

    // Use this for initializeation
    private void Awake() {
        // Set reference
        if (instance == null) {
            instance = this;
        }
    }

    // Start is called before the first frame update
    private void Start() {
        // Initialize variables
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    // Update is called once per frame
    private void Update() {
        UpdateEffects();
        UpdateFocus();
    }

    #region FX functions
    private void UpdateEffects() {
        ScreenShakeEffect();
    }

    // Screen shake effect
    private void ScreenShakeEffect() {
        // Check if screen shake is not active
        if (screenShakeTimer == 0) {
            return;
        }

        float _cameraShakeRot = (4f * (GameData.screenShake / 10)) * Mathf.Pow(screenShakeTimer, 1.25f) * Random.Range(-1f, 1f);
        float _cameraShakePos = _cameraShakeRot / 1.4f;

        // Shake the screen (via rotation)
        //transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + Random.Range(-_cameraShakeRot, _cameraShakeRot));

        // Shake the screen (via position)
        transform.localPosition += new Vector3(Random.Range(-_cameraShakePos, _cameraShakePos), Random.Range(-_cameraShakePos, _cameraShakePos), 0);

        // Reduce screen shake
        screenShakeTimer -= GameController.instance.GetPlayerClock().deltaTime;

        if (screenShakeTimer < 0) {
            StopScreenShake();
        }
    }

    // Update screen shake effect
    public void UpdateScreenShake(float newScreenShakeTime) {
        // Update variable
        if (newScreenShakeTime > screenShakeTimer) {
            screenShakeTimer = newScreenShakeTime;
        }
    }

    // Stop screen shake
    private void StopScreenShake() {
        screenShakeTimer = 0f;

        transform.localPosition = startPos;
        transform.localRotation = startRot;
    }


    // private Quaternion startRot;
    // private GameObject _focusTarget;
    // private Quaternion focusRootRot;
    // private Tween focusTween;
    // private float onTarget = 1f;
    private void UpdateFocus() {
        if (!active) return;
        if (_focusTarget == null) {
            if (onTarget >= 1f) {
                transform.localRotation = startRot;
            } else {
                transform.rotation = Quaternion.Lerp(focusRootRot, transform.parent.rotation, onTarget) * startRot;
            }
        } else {
            Vector3 lookVector = _focusTarget.transform.position - transform.parent.position;
            if (onTarget >= 1f) {
                transform.rotation = Quaternion.LookRotation(lookVector) * startRot;
            } else {
                transform.rotation = Quaternion.Lerp(focusRootRot, Quaternion.LookRotation(lookVector), onTarget) * startRot;
            }
        }
    }
    #endregion

    // When script is disabled or inactive
    private void OnDisable() {
        // Cancel all invokes
        CancelInvoke();
    }

    // When script is destroyed
    private void OnDestroy() {
        // Cancel all invokes
        CancelInvoke();

        // Reset reference to this script
        if (instance == this) {
            instance = null;
        }
    }
}

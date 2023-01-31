using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyKat {
    public class Tween {
        public Tween() {
            //add this to the tween manager on creation, useless without this step
            if (!TweenManager.instance) {
                Debug.LogError("Attempted to make Tween without a TweenManager, so making one first");
                new GameObject("TweenManager").AddComponent<TweenManager>();
            }
            TweenManager.instance.tweens.Add(this);
        }

        public enum Ease {
            None,
            Custom,
            Linear,
            InQuad,
            OutQuad,
            InOutQuad,
            InCubic,
            OutCubic,
            InOutCubic,
            Exponential
        }

        public delegate void Callback();
        public delegate void UpdateCallback(float v, float t);
        public Callback OnStart;
        public UpdateCallback OnUpdate;
        public Callback OnComplete;

        float start = 0f;
        float end = 1f;
        AnimationCurve customEase;
        Ease ease;
        float delay = 0f;
        float maxTime = 1f;
        float currentTime = 0f;
        bool paused = false;
        bool ignoreGameSpeed = false;
        bool destroyOnLoad = true;

        //Tween algorithms
        float GetValue(float nTime) {
            float delta = end - start;
            // Calculate value from algorithm for tween
            switch (ease) {
                case Ease.Custom: // custom animation curve
                    if (customEase != null) {
                        return start + (customEase.Evaluate(nTime) * delta);
                    }
                    break;
                case Ease.Linear: // standard linear
                    return delta * nTime + start;
                case Ease.InQuad: // quad in
                    return delta * nTime * nTime + start;
                case Ease.OutQuad: // quad out
                    return -delta * ((nTime) - 2f) * nTime * nTime + start;
                case Ease.InOutQuad: // quad in out
                    nTime *= 2f;
                    if (nTime < 1f) {
                        return (delta / 2f) * nTime * nTime + start;
                    } else {
                        nTime--;
                        return -(delta / 2f) * (nTime * (nTime - 2f) - 1f) + start;
                    }
                case Ease.InCubic: // cubic in
                    return delta * nTime * nTime * nTime + start;
                case Ease.OutCubic: // cubic out
                    return -delta * ((nTime) - 2f) * nTime * nTime * nTime + start;
                case Ease.InOutCubic: // cubic in out
                    nTime *= 2f;
                    if (nTime < 1f) {
                        return (delta / 2f) * nTime * nTime * nTime + start;
                    } else {
                        nTime -= 2;
                        return (delta / 2f) * (nTime * nTime * nTime + 2) + start;
                    }
                case Ease.Exponential: // exponential
                    return delta / (Mathf.Exp(-4f) - 1f) * Mathf.Exp(-4f * nTime) + start - delta / (Mathf.Exp(-4f) - 1f);
            }
            return 0f;
        }

        //sets
        public Tween SetStart(float value) {
            start = value;
            return this;
        }
        public Tween SetEnd(float value) {
            end = value;
            return this;
        }
        public Tween SetTime(float value) {
            maxTime = value;
            return this;
        }
        public Tween SetDelay(float value) {
            delay = value;
            return this;
        }
        public Tween SetEase(Ease value) {
            ease = value;
            return this;
        }
        public Tween SetOnStart(Callback callback) {
            OnStart = callback;
            return this;
        }
        public Tween SetOnUpdate(UpdateCallback callback) {
            OnUpdate = callback;
            return this;
        }
        public Tween SetOnComplete(Callback callback) {
            OnComplete = callback;
            return this;
        }
        public Tween SetIgnoreGameSpeed(bool value) {
            ignoreGameSpeed = value;
            return this;
        }
        public Tween SetCustomEase(AnimationCurve value) {
            customEase = value;
            return this;
        }
        public Tween SetDestroyOnLoad(bool value) {
            destroyOnLoad = value;
            return this;
        }

        public bool GetDestroyOnLoad() {
            return destroyOnLoad;
        }

        //playback control
        public Tween SetPause(bool value) {
            paused = value;
            return this;
        }
        public void Stop() {
            //just remove this? 
            TweenManager.instance.tweens.Remove(this);
        }

        //update
        public void Update() {
            try {
                if (currentTime == 0f) {
                    OnStart?.Invoke();
                }

                if (!paused) {
                    currentTime += ignoreGameSpeed ? Time.unscaledDeltaTime : Time.deltaTime;
                    float realTime = currentTime - delay;
                    if (realTime > 0f) {
                        OnUpdate?.Invoke(GetValue(Mathf.Clamp01(realTime / maxTime)), realTime);
                    }
                }

                if (currentTime >= maxTime + delay - Mathf.Epsilon) {
                    OnComplete?.Invoke();
                    Stop();
                }
            } catch (System.Exception) {
                if (TweenManager.instance.debug) {
                    Debug.Log("Caught An Exception in a tween, so it's being stopped.");
                }
                Stop();
            }
        }
    }
}
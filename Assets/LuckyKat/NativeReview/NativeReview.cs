using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using Google.Play.Review;
#endif

namespace LuckyKat {
    class NativeReview : MonoBehaviour {
#if UNITY_ANDROID
        private ReviewManager reviewManager;
        private PlayReviewInfo _playReviewInfo;
#endif
        // ...
        // Requires Google Play Unity plugins
        // Download here: https://github.com/google/play-unity-plugins/releases
        public void Start() {
#if UNITY_EDITOR
#elif UNITY_IOS
            bool available = Device.RequestStoreReview();
            if (!available) {
                Debug.Log("NativeReview Error: The iOS version isn't recent enough or the StoreKit framework is not linked with the app.");
            }
            Destroy(this);
#elif UNITY_ANDROID
            reviewManager = new ReviewManager();
            StartCoroutine(Request());
#endif
        }

#if UNITY_ANDROID
        private IEnumerator Request() {
            var requestFlowOperation = reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError) {
                Debug.Log("REVIEW REQUEST ERROR:" + requestFlowOperation.Error.ToString());
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();
            StartCoroutine(Launch());
        }
        private IEnumerator Launch() {
            var launchFlowOperation = reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError) {
                Debug.Log("REVIEW LAUNCH ERROR:" + launchFlowOperation.Error.ToString());
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
            Destroy(this);
        }
#endif
    }
}

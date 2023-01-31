using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour {
    // Variables
    private bool crossed = false;
    [SerializeField]
    private GameObject confettiRain;
    [SerializeField]
    private GameObject confettiLeft;
    [SerializeField]
    private GameObject confettiRight;
    private void OnTriggerEnter(Collider other) {
        if (!crossed) {
            if (other.CompareTag("Player")) {
                crossed = true;
                // Invoke(nameof(doConfetti), 0.4f);
                doConfetti();
                SoundPlayer.instance.play("finish");
            }
        }
    }

    private void doConfetti() {
        if (confettiRain) {
            confettiRain.SetActive(true);
        }
        if (confettiLeft) {
            confettiLeft.SetActive(true);
        }
        if (confettiRight) {
            confettiRight.SetActive(true);
        }
    }
}
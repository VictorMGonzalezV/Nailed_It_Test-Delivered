using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundPlayer : MonoBehaviour {
    [SerializeField] private AudioSource sounds;
    // public AudioClip door;
    // public AudioClip switched;
    // public AudioClip spray;
    // public AudioClip firedie;
    // public AudioClip grab;
    // public AudioClip drop;
    // public AudioClip newMan;
    // public AudioClip die;
    // public AudioClip jump;
    // public AudioClip fall;
    // public AudioClip confetti;
    // public AudioClip hoseReel;
    // public AudioClip hoseDrop;
    // public AudioClip hoseGet;
    // public AudioClip win;
    // public AudioClip doorBreak;
    public SfxContainer audioClips;
    static private List<string> audioIds;
    static public SoundPlayer instance;

    private void Start() {
        // sounds = GetComponent<AudioSource>();
        if (audioIds == null) {
            audioIds = new List<string>();
            for (var i = 0; i < audioClips.audio.Length; i++) {
                audioIds.Add(audioClips.audio[i].name);
            }
        }
        instance = this;
    }
    public void play(string name, float pitch = 1, float volume = 1) {
        int soundIndex = audioIds.IndexOf(name);
        if (soundIndex == -1) return;
        AudioClip sfx = audioClips.audio[soundIndex];
        sounds.pitch = pitch;
        sounds.volume = volume;
        sounds.PlayOneShot(sfx);
    }
    // public void startHose() {
    //     sounds.clip = spray;
    //     sounds.Play();
    // }
    // public void stopHose() {
    //     sounds.Stop();
    // }

}

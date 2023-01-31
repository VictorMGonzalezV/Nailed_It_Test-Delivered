using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SfxContainer", menuName = "Sfx Container", order = 1)]

public class SfxContainer : ScriptableObject {
    public AudioClip[] audio;
}

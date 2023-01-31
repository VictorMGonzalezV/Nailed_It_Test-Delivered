using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponsData", menuName = "Weapons Data", order = 1)]

public class WeaponsData : ScriptableObject {
    public string[] id;
    public Sprite[] uiImage;
    public Sprite[] uiBackground;

    public int getId(string val) {
        return System.Array.IndexOf(id, val);
    }

    public string getId(int val) {
        return id[val];
    }
}

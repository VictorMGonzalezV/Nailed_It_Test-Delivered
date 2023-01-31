using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorContainer", menuName = "Color Container", order = 1)]
public class ColorContainer : ScriptableObject
{
    public Color[] colors;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MaterialContainer", menuName = "Material Container", order = 1)]
public class MaterialContainer : ScriptableObject
{
    public Material[] materials;
}

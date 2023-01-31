using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MeshContainer", menuName = "Mesh Container", order = 1)]

public class MeshContainer : ScriptableObject
{
    public Mesh[] geometry;
}

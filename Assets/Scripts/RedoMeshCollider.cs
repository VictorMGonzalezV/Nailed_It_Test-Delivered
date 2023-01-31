using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedoMeshCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public PhysicMaterial physicMaterial;
    void Start()
    {
        MeshCollider mc = gameObject.GetComponent<MeshCollider>();
        if (mc != null) {
            DestroyImmediate(mc);
        }
        //update the mesh collider
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf) {
            mc = gameObject.AddComponent<MeshCollider>();
            mc.convex = true;
            mc.sharedMesh = mf.mesh;
            if (physicMaterial != null) {
                mc.material = physicMaterial;
            }
        }
    }
}

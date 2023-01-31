using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeSwitcher : MonoBehaviour
{
    static public int MaxThemes = 5;
    static public int Theme = 0;
    [SerializeField]
    private MaterialContainer materialContainer;
    [SerializeField]
    private MeshContainer geometryContainer;
    [SerializeField]
    private ColorContainer[] colorContainers;
    [SerializeField]
    private Material[] swapMaterialColors;
    private bool resetMaterials = false;
    // Start is called before the first frame update
    void Start()
    {
        if (materialContainer != null) {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null) {
                meshRenderer.material = materialContainer.materials[Theme % materialContainer.materials.Length];
            }
        }

        if (geometryContainer != null) {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null) {
                meshFilter.mesh = geometryContainer.geometry[Theme % geometryContainer.geometry.Length];
            } else {
                SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
                if (skinnedMeshRenderer) {
                    skinnedMeshRenderer.sharedMesh = geometryContainer.geometry[Theme % geometryContainer.geometry.Length];
                }
            }
        }
        if (colorContainers != null && swapMaterialColors != null) {
            resetMaterials = true;
            // Maybe have to change the color id in the future since the artists dont use references so its going to be random if they make a new shader :(
            for (int i = 0; i < colorContainers.Length && i < swapMaterialColors.Length; i++) {
                swapMaterialColors[i].SetColor("Color_caa2574da0064fd9b5ac9e91319b3d89", colorContainers[i].colors[Theme % colorContainers[i].colors.Length]);
            }
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        #if UNITY_EDITOR
            if (resetMaterials) {
                for (int i = 0; i < colorContainers.Length && i < swapMaterialColors.Length; i++) {
                    swapMaterialColors[i].SetColor("Color_caa2574da0064fd9b5ac9e91319b3d89", colorContainers[i].colors[0]);
                }
            }
        #endif
    }
}

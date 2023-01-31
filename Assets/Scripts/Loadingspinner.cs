using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadingspinner : MonoBehaviour
{
    private RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (rect != null) {
            Quaternion rotation = rect.localRotation;
            rotation *= Quaternion.Euler(0f, 0f, -4f * 60f * Time.deltaTime);
            rect.localRotation = rotation;
        }
    }
}

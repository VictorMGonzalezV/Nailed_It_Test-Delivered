using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NailAction : MonoBehaviour {
    // Start is called before the first frame update
    public delegate void Action();
    public Action OnHit;
    public bool Instant = false;
    public bool Active = true;

    public void DoHit() {
        OnHit?.Invoke();
    }
}

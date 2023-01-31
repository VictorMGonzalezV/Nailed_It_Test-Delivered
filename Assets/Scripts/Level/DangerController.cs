using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] hazards;
    // Start is called before the first frame update
    private bool Activated = false;
    private bool IsDone = false;
    private List<Hazard> hazardComponents;
    void Start()
    {
        SetRigidbodies(true);
        hazardComponents = new List<Hazard>();
        foreach(Rigidbody r in hazards) {
            Hazard h = r.gameObject.GetComponent<Hazard>();
            if (h != null) {
                hazardComponents.Add(h);
            }
        }
    }

    public bool Status() {
        if (IsDone) {
            return IsDone;
        }
        bool isDone = true;
        foreach(Hazard h in hazardComponents) {
            isDone = isDone && h.IsDone;
        }
        IsDone = isDone;
        return isDone;
    }

    void SetRigidbodies(bool state) {
        if (hazards != null) {
            foreach (Rigidbody r in hazards) {
                r.isKinematic = state;
            }
        }
    }

    public void InitDanger() {
        SetRigidbodies(false);
    }
}

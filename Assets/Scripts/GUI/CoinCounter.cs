using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour {
    static private List<CoinCounter> current = new List<CoinCounter>();
    static public void UpdateCoins() {
        foreach (CoinCounter cb in current) {
            cb.UpdateText();
        }
    }
    static public void UpdateCoinsDirty(int value) {
        foreach (CoinCounter cb in current) {
            cb.DirtyUpdateText(value);
        }
    }
    [SerializeField]
    private Text coinText;
    // Start is called before the first frame update
    void Start() {
        current.Add(this);
        UpdateText();
    }

    void OnDestroy() {
        current.Remove(this);
    }

    // Update is called once per frame
    void Update() {

    }

    public void UpdateText() {
        coinText.text = GameData.coins.ToString();
    }

    public void DirtyUpdateText(int value) {
        coinText.text = value.ToString();
    }
}

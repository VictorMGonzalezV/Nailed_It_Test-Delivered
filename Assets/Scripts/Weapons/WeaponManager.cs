using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    // Start is called before the first frame update
    static public WeaponManager instance;
    public WeaponsData data;
    public GameObject[] weapons;
    public GameObject active;
    public GunController controller;
    public int selectedId = 0;
    public string selectedKey = "none"; // set to none so we always get the controller
    void Start() {
        instance = this;
        ConfirmWeapon();
    }

    // // Update is called once per frame
    // void Update() {

    // }
    public void SetWeapon(string value, bool save = false) {
        if (data == null) return;
        if (data.getId(value) > -1) {
            GameData.weapon = value;
            if (save) {
                GameData.SaveGameData();
            }
            ConfirmWeapon();
        }
    }
    public void ConfirmWeapon() {
        if (data != null && GameData.weapon != selectedKey) {
            selectedKey = GameData.weapon;
            weapons[selectedId].SetActive(false);
            selectedId = data.getId(GameData.weapon);
            if (selectedId > -1) {
                //Setting the SelectedId to 1 locks the crossbow selection-VMG
                active = weapons[1];
            } else {
                selectedKey = "crossbow";
                selectedId = 1;
                Debug.Log("Weapon Manager: ID does not exist, default weapon selected");
                active = weapons[0];
            }
            controller = active.GetComponent<GunController>();
            active.SetActive(true);
        }
    }
}

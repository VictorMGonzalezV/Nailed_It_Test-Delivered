using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameData : MonoBehaviour {
    #region Variables
    // Game variables
    public static bool init = true;                                                                     // Initialization for game
    public static int build;                                                                            // Game build
    public static string saveFilePath;                                                                  // Save file path

    // Game session variables
    public static GameMode gameMode;                                                                    // Current game mode being played

    // Settings variables
    public static int gifNumber;                                                                        // GIF number
    public static int screenshotNumber;                                                                 // Screenshot number
    public static int screenshotResolution;                                                             // Resolution of screenshot
    public static int screenShake;

    // Audio
    public static int musicVolume;
    public static int sfxVolume;

    // Stats variables
    public static float playTime;

    // Gameplay variables
    public static int level;
    public static int coins;
    public static string weapon;
    public static float weaponProgression;
    public static int unlockWeapon;
    // public static bool[] weaponsUnlocked = new bool[1];
    public static List<int> weaponsUnlocked = new List<int>();


    #endregion

    #region Enums
    public enum GameMode {
        sandbox
    }
    #endregion

    #region Data functions
    // New game data
    private static void NewGameData() {
        // Create directories
        CheckDirectories();

        // Create settings
        gifNumber = 0;
        screenshotNumber = 0;
        screenshotResolution = 2;
        screenShake = 10;

        musicVolume = 0;
        sfxVolume = 0;

        // Create stats
        playTime = 0f;

        // Create game data
        level = 0;
        coins = 0;
        //Setting the weapon to crossbow here makes the game automatically start with the crossbow-VMG
        weapon = "default";
        weaponProgression = 0;
        unlockWeapon = 0;
        weaponsUnlocked = new List<int>();
        weaponsUnlocked.Add(0);
    }

    // Load game data
    public static void LoadGameData() {
        // Check directories
        CheckDirectories();

        // Check if file exists
        if (File.Exists(saveFilePath)) {
            // Decrypt data
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            DataLK data = (DataLK)bf.Deserialize(file);

            // Load game build
            int tempBuild = data.build;

            // Check for new game build and load applicable code
            if (tempBuild < build) {
                // Loop through versions to get new code
                while (tempBuild < build) {
                    switch (tempBuild) {
                        default:
                            // No new code
                            break;
                    }

                    // Increment build number
                    tempBuild++;
                }
            }

            // Load settings
            gifNumber = data.gifNumber;
            screenshotNumber = data.screenshotNumber;
            screenshotResolution = data.screenshotResolution;
            screenShake = data.screenShake;

            musicVolume = data.musicVolume;
            sfxVolume = data.sfxVolume;

            // Load stats
            playTime = data.playTime;

            // Load game data
            level = data.level;
            coins = data.coins;
            weapon = data.weapon;
            weaponProgression = data.weaponProgression;
            unlockWeapon = data.unlockWeapon;

            if (data.weaponsUnlocked != null) {
                weaponsUnlocked = data.weaponsUnlocked;
            } else { // save file already exists from older version so we need to populate the list with default values
                weaponsUnlocked = new List<int>();
                weaponsUnlocked.Add(0);
            }

            // Close file
            file.Close();
        } else {
            // Create new data
            NewGameData();
        }

        // Save game data
        SaveGameData();
    }

    // Save game data
    public static void SaveGameData() {
        // Check directories
        CheckDirectories();

        // Check platform
        if (saveFilePath == "") {
            Debug.LogError("Platform save file path not found");
            return;
        }

        // Encrypt data
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFilePath);

        // Instantiate new data
        DataLK data = new DataLK();

        // Save settings
        data.build = build;

        data.gifNumber = gifNumber;
        data.screenshotNumber = screenshotNumber;
        data.screenshotResolution = screenshotResolution;
        data.screenShake = screenShake;

        data.musicVolume = musicVolume;
        data.sfxVolume = sfxVolume;
        data.level = level;
        //Ensuring collected coins aren't saved between runs of the demo -VMG
        data.coins = 0;
        data.weapon = weapon;
        //Ensuring weapon progression isn't saved between runs of the demo-VMG
        data.weaponProgression = weaponProgression;
        data.unlockWeapon = unlockWeapon;
        data.weaponsUnlocked = weaponsUnlocked;

        // Save stats
        data.playTime = playTime;

        // Save data and close file
        bf.Serialize(file, data);
        file.Close();
    }

    // Check directories
    private static void CheckDirectories() {
        // Check platform
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor) {
            // Create directories if required
            if (!System.IO.Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "User")) {
                System.IO.Directory.CreateDirectory(Application.dataPath + Path.DirectorySeparatorChar + "User");
            }

            if (!System.IO.Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "User" + Path.DirectorySeparatorChar + "Data")) {
                System.IO.Directory.CreateDirectory(Application.dataPath + Path.DirectorySeparatorChar + "User" + Path.DirectorySeparatorChar + "Data");
            }
        } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            // Create directories if required
            if (!System.IO.Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Data")) {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Data");
            }
        } else if (Application.platform == RuntimePlatform.Android) {
            // Create directories if required
            if (!System.IO.Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Data")) {
                System.IO.Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Data");
            }
        } else {
            Debug.LogError("Platform not found");
        }
    }
    #endregion

    #region Helper functions
    // Return time
    public static string ReturnTime(float newTime) {
        // Variable to return
        string time = "";

        // Calculate time
        int _minutes = (int)(newTime / 60);
        int _seconds = (int)(newTime % 60);
        int _fraction = (int)(newTime * 1000) % 1000;
        time = string.Format("{00:00} : {1:00}.{2:000}", _minutes, _seconds, _fraction);

        // Return time value
        return time;
    }

    // Return int value with commas string format
    public static string ReturnIntWithCommasStringFormat(int intValue) {
        return String.Format("{0:0,0}", intValue);
    }
    #endregion
}


[Serializable]
class DataLK {
    public int build;

    public int gifNumber;
    public int screenshotNumber;
    public int screenshotResolution;
    public int screenShake;

    public int musicVolume;
    public int sfxVolume;

    public float playTime;

    public int level;
    public int coins;
    public string weapon;
    public float weaponProgression;
    public int unlockWeapon;
    public List<int> weaponsUnlocked;
}

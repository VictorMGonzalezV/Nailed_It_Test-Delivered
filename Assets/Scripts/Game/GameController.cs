using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
#if UNITY_EDITOR
using System.IO;
#endif
using Chronos;
using LuckyKat;
//using GameAnalyticsSDK;-Disabling the GameAnalyticsSDK

public class GameController : MonoBehaviour {
    #region Variables
#pragma warning disable 0649
    // Reference
    public static GameController instance;
    public int bannerStart = 2;
    public int interstitialStart = 3;
    public bool interstitialBeforeWin = false;
    //Setting noAds to true-VMG
    public bool noAds = true;

    [Header("Time")]
    [SerializeField]
    private Clock gameClock;
    [SerializeField]
    private Clock levelClock;
    [SerializeField]
    private Clock enemyClock;
    [SerializeField]
    private Clock playerClock;

    [Header("References")]
    [SerializeField]
    private TransitionController transitionComp;

    [Header("GUI")]
    [SerializeField]
    private GameObject menuMain;
    [SerializeField]
    private WinBonusController winMenu;
    [SerializeField]
    private LoseController loseMenu;
    [SerializeField]
    private IngameController ingameMenu;
    //[SerializeField]
    //private ShopController shopMenu;
    [SerializeField]
    private GameObject shopButton;
    [SerializeField]
    private GameObject cheatMenu;
    private bool doMirror = false;


    [Header("Game")]
    public int CurrentLevel = 0;
    public int MaxLevel = 1;

    // Control
    private bool gameStarted;
    public bool gameOver;

    public int overrideTheme = -1;
    public string version;
    private bool askRating = false;
#pragma warning restore 0649
    #endregion

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // LoadLevel(CurrentLevel);
        version = Application.version;
        if (PlayerPrefs.GetString("gameVersion", "-1") != version) {
            askRating = true;
        }
    }

    //In the end I didn't use this function,  I hardcoded the level number to 1 so Level004 would have been the only option
    //The requirement was to deliver only one level and reduce the project size as much as possible, so I deleted all unused levels-VMG
    public void SetLevel(int lvl = 1) {
        GameData.level = lvl;
        GameData.SaveGameData();
    }

  
    
    public void LoadLevel(int lvl = 0) {
        if (lvl < 0) {
            lvl = (lvl + MaxLevel) % MaxLevel;
            Debug.Log("LoadLevel: level variable below zero, correcting with maxLevel(" + MaxLevel.ToString() + ")... " + lvl.ToString());
        }
        GameData.level = lvl;
        
        /*int level = lvl;
        int startLevels = 2;
        int loopedLevel = MaxLevel - startLevels;
        if (level >= MaxLevel) { // loop back to level 5
            doMirror = Mathf.Floor((level - startLevels) / loopedLevel) % 2 == 1;
            level = (level - startLevels) % loopedLevel + startLevels;
        } else {
            doMirror = false;
        }*/

        GameData.SaveGameData();
        // string levelString = "00" + level;
        // levelString = levelString.Substring(levelString.Length - 3);

        //Disabling theme changing for levels-not needed if there's only one playable demo level -VMG
        // set theme
        /*if (overrideTheme == -1) {
            ThemeSwitcher.Theme = (int)Mathf.Floor(level / 5) % ThemeSwitcher.MaxThemes;
        } else {
            ThemeSwitcher.Theme = overrideTheme;
        }*/

        // SceneManager.LoadScene("Level" + levelString);
        //Manually loading Level004-VMG
        SceneManager.LoadScene("Level004");
        gameStarted = false;
        gameOver = false;
    }

    // Use this for initialization
    private void Awake() {
        // Set reference
        if (instance) {
            //Debug.LogError("GameController already exists");
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
    }

    private void Mirror() {
        GameObject[] mirror = GameObject.FindGameObjectsWithTag("Mirror");
        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] civ = GameObject.FindGameObjectsWithTag("Civilian");
        GameObject[] path = GameObject.FindGameObjectsWithTag("Path");

        Vector3 workScale;
        foreach (GameObject m in mirror) {
            workScale = m.transform.localScale;
            workScale.x = -workScale.x;
            m.transform.localScale = workScale;
        }
        foreach (GameObject e in enemy) {
            workScale = e.transform.localScale;
            workScale.x = -workScale.x;
            e.transform.localScale = workScale;
        }
        foreach (GameObject c in civ) {
            workScale = c.transform.localScale;
            workScale.x = -workScale.x;
            c.transform.localScale = workScale;
        }
        foreach (GameObject p in path) {
            if (p.transform.parent == null) { // only root
                workScale = p.transform.localScale;
                workScale.x = -workScale.x;
                p.transform.localScale = workScale;
            }
        }
    }

    // Use this for initialization
    private void Start() {
        // Initialize variables
        gameStarted = false;
        gameOver = false;

        DisableGUI();

        if (SceneManager.GetActiveScene().name != "Init") {
            //Setting menus to inactive
            menuMain.SetActive(false);
            ingameMenu.gameObject.SetActive(false);
            winMenu.gameObject.SetActive(false);
            loseMenu.gameObject.SetActive(false);
        } else { // asume we started in editor
#if UNITY_EDITOR
            if (GameData.init) {
                // Update initialization variable
                GameData.init = false;

                // Set default game variables
                GameData.build = 1;

                GameData.gameMode = GameData.GameMode.sandbox;

                // Set time scale to 1
                Time.timeScale = 1;

                // Check platform and update save file path
                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxEditor) {
                    GameData.saveFilePath = Application.dataPath + Path.DirectorySeparatorChar + "User" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "savedata.dat";
                } else if (Application.platform == RuntimePlatform.IPhonePlayer) {
                    GameData.saveFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "savedata.dat";
                } else if (Application.platform == RuntimePlatform.Android) {
                    GameData.saveFilePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + "savedata.dat";
                } else {
                    Debug.LogError("Platform not found");
                }

                // Load game data
                GameData.LoadGameData();
            }
#endif
        }
        //Commenting out reference to the deleted Vibration asset-VMG
        //Vibration.Init();
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestartGame();
            return;
        }
    }

    #region Game functions
    public void Fail() {
        if (!gameOver) {
            loseMenu.gameObject.SetActive(true);
            //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "level_" + (GameData.level + 1));
            //Firebase.Analytics.Parameter[] LevelEndParameters = {
                //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevelName, "level_" + (GameData.level + 1)),
                //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterSuccess, 0)
            //};
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelEnd, LevelEndParameters);

            gameOver = true;
        }
    }
    public void Win() {
        if (!gameOver) {
            // winMenu.GiveCoinReward();
            gameOver = true;
            //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "level_" + (GameData.level + 1));
            // SetLevel(GameData.level + 1);
            //Firebase.Analytics.Parameter[] LevelEndParameters = {
                //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterLevelName, "level_" + (GameData.level + 1)),
                //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterSuccess, 1)
            //};
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelEnd, LevelEndParameters);

            // do it manually to avoid saving twice

            //Keep the level number at 1 (Level004 scene is 1 in the Build)
            //GameData.level = 1;

            //I didn't disable the coin reward here on purpose, that way players can see there's a coin reward for completing the level-VMG
            //winMenu.GiveCoinReward();
            // new Firebase.Analytics.Parameter("earn_virtual_currency", "level_" + (GameData.level + 1)),

            //Firebase.Analytics.Parameter[] currencyParameters = {
                //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterVirtualCurrencyName, "coins"),
                //new Firebase.Analytics.Parameter(Firebase.Analytics.FirebaseAnalytics.ParameterValue, 200)
            //};
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventEarnVirtualCurrency, currencyParameters);

            // Show win screen or interstitial+winscreen
            if (!noAds && GameData.level >= GameController.instance.interstitialStart && GameController.instance.interstitialBeforeWin) { // since game level is zero based and interstitial start is one based we dont have to equalize them because game level has already incremented at this point
                /*AdManager.instance.ShowInterstitial(() => {
                    winMenu.ResetIU();
                    winMenu.gameObject.SetActive(true);
                });*/
            } else {
                winMenu.ResetIU();
                winMenu.gameObject.SetActive(true);
            }
        }
    }
    public void RestartGame() {
        CancelInvoke(nameof(RestartGame));

        gameStarted = false;
        gameOver = false;

        // set theme
        if (overrideTheme == -1) {
            ThemeSwitcher.Theme = (int)Mathf.Floor(GameData.level / 5) % ThemeSwitcher.MaxThemes;
        } else {
            ThemeSwitcher.Theme = overrideTheme;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void KilledEnemy() {
        ingameMenu.AddCombo();
    }
    public void Headshot() {
        ingameMenu.DoHeadShot();
    }
    #endregion

    #region FX functions
    public void ActivateHitEffect() {
        //levelClock.localTimeScale = 0.1f;


        StopCoroutine(HitEffect());
        StartCoroutine(HitEffect());

        //CancelInvoke(nameof(CancelHitEffect));
        //Invoke(nameof(CancelHitEffect), 0.2f);
    }

    private void CancelHitEffect() {
        CancelInvoke(nameof(CancelHitEffect));

        levelClock.localTimeScale = 1.0f;
    }
    #endregion

    #region GUI functions
    private void DisableGUI() {
        menuMain.SetActive(false);
        // ingameMenu.gameObject.SetActive(false);
        // winMenu.gameObject.SetActive(false);
        // loseMenu.gameObject.SetActive(false);
    }

    public void StartGame() {
        // Check scene
        if (SceneManager.GetActiveScene().name == "Init") {
            return;
        }

        // Update GUI
        DisableGUI();

        // Update variables
        gameStarted = true;
        gameOver = false;

        if (GameObject.FindGameObjectWithTag("Player")) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerStartGame();
            //Disabling references to GameAnalytics and Firebase-VMG
            //GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "level_" + (GameData.level + 1));
            //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelStart, Firebase.Analytics.FirebaseAnalytics.ParameterLevelName, "level_" + (GameData.level + 1));
        } else {
            Debug.LogError("Player not found");
        }
    }
    //Disabling these two functions because the shop was removed -VMG
    /*public void OpenShop() {
        shopMenu.gameObject.SetActive(true);
        shopButton.SetActive(false);
        cheatMenu.SetActive(false);
    }*/

    /*public void CloseShop() {
        shopButton.SetActive(true);
        shopMenu.gameObject.SetActive(false);
        cheatMenu.SetActive(true);
    }*/
    #endregion

    #region Reference functions
    public TransitionController GetTransitionController() {
        return transitionComp;
    }

    public Clock GetGameClock() {
        return gameClock;
    }

    public Clock GetLevelClock() {
        return levelClock;
    }

    public Clock GetEnemyClock() {
        return enemyClock;
    }

    public Clock GetPlayerClock() {
        return playerClock;
    }

    public bool HasGameStarted() {
        return gameStarted;
    }

    public bool IsGameOver() {
        return gameOver;
    }
    #endregion

    #region Coroutine functions
    private IEnumerator HitEffect() {
        // levelClock.localTimeScale = 0.1f;

        // yield return new WaitForSeconds(0.05f);

        // float _newTimeValue = 0.1f;

        // while (_newTimeValue < 1) {
        // 	_newTimeValue += 0.05f;

        // 	if (_newTimeValue >= 1) {
        // 		_newTimeValue = 1;
        // 	}

        // 	levelClock.localTimeScale = _newTimeValue;

        yield return new WaitForSeconds(0.05f);
        // }
    }
    #endregion

    #region Helper functions
    private void ResetScene() {
        CancelInvoke();
        StopAllCoroutines();

        levelClock.localTimeScale = 1.0f;
    }

    // Quit game
    private void OnApplicationQuit() {
        // Update stats
        GameData.playTime += Time.realtimeSinceStartup;

        // Save game data
        GameData.SaveGameData();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Init") {
            // Do nothing
            // } else if (scene.name == "Main" || scene.name == "Sandbox") {
            // menuMain.SetActive(true);
        } else {
            //Manually disabling the menus-VMG
            menuMain.SetActive(true);
            ingameMenu.gameObject.SetActive(true);
            ingameMenu.ResetUI();
            winMenu.gameObject.SetActive(false);
            loseMenu.gameObject.SetActive(false);
            //Manually disabling the shop button-VMG
            shopButton.SetActive(false);
            //Disabling code to display the shop button-VMG
            /*if (GameData.weaponsUnlocked.Count > 1) {
                shopButton.SetActive(true);
            } else {
                shopButton.SetActive(false);
            }*/
            // Debug.LogError("New scene loaded and not found");
            if (doMirror) Mirror();

            // ask rating
            if (GameData.level > 0 && askRating) {
                askRating = false;
                PlayerPrefs.SetString("gameVersion", version); // remember this version
                gameObject.AddComponent<NativeReview>();
            }

            // only show banner from certain point in game
            /*if (!noAds && GameData.level + 1 >= GameController.instance.bannerStart) {
                AdManager.instance.showBanner();
            } else {
                AdManager.instance.hideBanner();
            }*/
        }

        ResetScene();
    }

    private void OnDisable() {
        CancelInvoke();
        StopAllCoroutines();

        SceneManager.sceneLoaded -= OnSceneLoaded;

        //Commenting out reference to the deleted Vibration asset-VMG
        //Vibration.Cancel();
    }

    // When script is destroyed
    private void OnDestroy() {
        // Reset reference to this script
        if (instance == this) {
            instance = null;
        }
    }
    #endregion
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System;
//using Facebook.Unity; Disabling unused SDK's-VMG
//using GameAnalyticsSDK;
using LuckyKat;
#if UNITY_IOS
using Unity.Notifications.iOS;
using UnityEngine.iOS;
#endif
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class Init : MonoBehaviour {
    static bool ATTEnabled = true;
    static bool notificationGranted = true;
    private bool hasAlreadyContinued = false;
    // Use this for initialization
    private void Start() {
        // Target 60 fps
        Application.targetFrameRate = 60;

        // Set audio listener default value (global)
        AudioListener.volume = 1f;

        // Initialize Tenjin
        //TenjinConnect();

        //The game was getting stuck on the spinning loading icon
        //I added a call to InitPhase2() here at the end of the Start function so it advances towards the -unused- Main Menu-VMG
        InitPhase2();
    }

    private void InitPhase2() {
        if (hasAlreadyContinued) return;
        hasAlreadyContinued = true;

        //Disabling all calls to initialize the removed SDKs-VMG
        // Initialize GA
        //InitGameAnalytics();

        // subscribe to MoPub to get ILRD data
        //GameAnalytics.SubscribeIronSourceImpressions();

        // bootup event
        //GameAnalytics.NewDesignEvent("AppOpen");
        //Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventAppOpen);

        
        // // Initialize Tenjin
        // TenjinConnect();

        // Initialize FB
        //InitFacebook();

        // Initialize IronSource
        //InitIronSource();

        InitGameData();

        // Notifications
        InitNotifications();

        // Initialize everything
        Invoke(nameof(ContinueToMainMenu), 3f);
    }

#if UNITY_IOS
    private void InitNotifications() {
        StartCoroutine(RequestAuthorization());
    }

    IEnumerator RequestAuthorization() {
        using (var req = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge, true)) {
            while (!req.IsFinished) {
                yield return null;
            };

            // string res = "\n RequestAuthorization: \n";
            // res += "\n finished: " + req.IsFinished;
            // res += "\n granted :  " + req.Granted;
            // res += "\n error:  " + req.Error;
            // res += "\n deviceToken:  " + req.DeviceToken;

            // Debug.Log(res);
            notificationGranted = req.Granted;

            iOSNotificationCenter.RemoveScheduledNotification("NailedIt00");
            iOSNotificationCenter.RemoveDeliveredNotification("NailedIt00");
            if (notificationGranted) {
                iosNotificationPlanner();
            }
        }
    }

    void iosNotificationPlanner() {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger() {
            TimeInterval = new TimeSpan(24, 0, 0),
            Repeats = false
        };

        var notification = new iOSNotification() {
            // You can optionally specify a custom identifier which can later be 
            // used to cancel the notification, if you don't set one, a unique 
            // string will be generated automatically.
            Identifier = "NailedIt00",
            Title = "Time to nail some bad guys!",
            Body = "Quick! Save the hostages before it's too late!",
            Subtitle = "",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };
        iOSNotificationCenter.ScheduleNotification(notification);
    }
#elif UNITY_ANDROID
    private void InitNotifications() {
        // create channel
        var c = new AndroidNotificationChannel() {
            Id = "NailedIt3D",
            Name = "Nailed It Channel",
            Importance = Importance.High,
            Description = "Nailed It notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);

        // get old identifier and check if notification exist, clean up if so
        int identifier = PlayerPrefs.GetInt("NailedIt00", -1);
        if (identifier > -1) {
            switch (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier)) {
                case NotificationStatus.Scheduled: // for now just cancel previous notification
                // Replace the currently scheduled notification with a new notification.
                // AndroidNotificationCenter.UpdateScheduledNotification(identifier, newNotification, channel);
                // break;
                case NotificationStatus.Delivered:
                    //Remove the notification from the status bar
                    AndroidNotificationCenter.CancelNotification(identifier);
                    break;
                    // case NotificationStatus.Unknown:
                    //     AndroidNotificationCenter.SendNotification(newNotification, "channel_id");
                    //     break;
            }
        }

        // send new notification
        var notification = new AndroidNotification();
        notification.Title = "Time to nail some bad guys!";
        notification.Text = "Quick! Save the hostages before it's too late!";
        notification.FireTime = System.DateTime.Now.AddDays(1);

        identifier = AndroidNotificationCenter.SendNotification(notification, "NailedIt3D");
        PlayerPrefs.SetInt("NailedIt00", identifier);
    }
#else
    private void InitNotifications() { }
#endif

    // Initialization
   /*private void InitGameAnalytics() {
        void onRemoteConfig() {
            AdManager.instance.interstitialCooldown = int.Parse(LuckyKat.GARemoteConfig.GetOption("INT_TIMER", "45"));
            GameController.instance.interstitialStart = int.Parse(LuckyKat.GARemoteConfig.GetOption("INT_START", "3"));
            GameController.instance.interstitialBeforeWin = LuckyKat.GARemoteConfig.GetOption("INT_PREWIN", "NO").ToUpper() == "YES";
            GameController.instance.bannerStart = int.Parse(LuckyKat.GARemoteConfig.GetOption("BAN_START", "2"));
        }
        LuckyKat.GARemoteConfig.Init(onRemoteConfig);
        // GameAnalytics.SetCustomId("LK01");
        //GameAnalytics.Initialize();
        // InitAfterGameAnalytics();
        //start GA
        // #if UNITY_EDITOR
        //         GameAnalytics.Initialize();
        //         InitAfterGameAnalytics();
        // #elif UNITY_IOS
        //         // Version currentVersion = new Version(Device.systemVersion); // Parse the version of the current OS
        //         // Version ios14 = new Version("14.0"); // Parse the iOS 13.0 version constant
        //         // if (currentVersion >= ios14) { // Tenjin or GA? GA might be replaced so probably Tenjin, will have to figure add next build when adding remote config to the mix
        //         //     GameAnalytics.RequestTrackingAuthorization(this);
        //         // } else {
        //             GameAnalytics.Initialize();
        //             InitAfterGameAnalytics();
        //         // }
        // #else
        //         GameAnalytics.Initialize();
        //         InitAfterGameAnalytics();
        // #endif
    }*/
    public void TenjinConnect() {
        // #if UNITY_IOS 
        //BaseTenjin instance = Tenjin.getInstance("TVITWX4WGSZGBSL58QGWY7XLBYXIAK4B");
        //instance.OptIn();
        // instance.Connect();
        // #endif
        // Sends install/open event to Tenjin
        // if (Application.isEditor) {
        // }
#if UNITY_EDITOR
        //instance.Connect();
        //InitPhase2();
#elif UNITY_IOS
        // bool forceATT = false;
        // float deviceVersion = 0;
        // if (GameAnalytics.IsRemoteConfigsReady())
        // {
        //     if (GameAnalytics.GetRemoteConfigsValueAsString("forceATT", "false") == "true")
        //     {
        //         forceATT = true;
        //     }
        // }
        // Debug.Log("===>FORCE ATT IS: " + forceATT);
        // Debug.Log("===>DEVICE SYSTEMVERSION IS: " + deviceVersion);
        // if (float.TryParse(UnityEngine.iOS.Device.systemVersion, out deviceVersion))
        // {
        //     deviceVersion = float.Parse(UnityEngine.iOS.Device.systemVersion);
        // }
        // don't force ATT for older devices
        // if(deviceVersion < 14)
        // {
        //     forceATT = false;
        // }
        // if (deviceVersion >= 14.5 || forceATT == true)
        // {


        Version currentVersion = new Version(Device.systemVersion); // Parse the version of the current OS
        Version ios14 = new Version("14.0"); // Parse the iOS 13.0 version constant
        if (currentVersion >= ios14) { // Tenjin or GA? GA might be replaced so probably Tenjin, will have to figure add next build when adding remote config to the mix
            instance.RequestTrackingAuthorizationWithCompletionHandler((status) => {
                Debug.Log("===> App Tracking Transparency Authorization Status: " + status);
                instance.Connect();
                Debug.Log("TENJIN CONNECT ATT");
                InitPhase2();
            });
        } else {
            instance.Connect();
            Debug.Log("TENJIN CONNECT NO ATT");
            InitPhase2();
        }
#elif UNITY_ANDROID
        // Sends install/open event to Tenjin
        instance.Connect();
        InitPhase2();
#endif
    }
    /*private void InitFacebook() {
        void onInit() {
            if (FB.IsInitialized) {
                FB.ActivateApp();
                Debug.Log("Facebook Initialized");
            } else {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }
        //start Facebook
        if (!FB.IsInitialized) {
            FB.Init(onInit);
        } else {
            FB.ActivateApp();
            Debug.Log("Facebook Initialized");
        }
    }*/

    public void InitIronSource() {
        //AdManager.instance.Init();

        // SKAdnetwork class
        LuckyKat.SKAdNetwork.Init();
    }

    public void InitGameData() {
        // Check that initialization script needs to run
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
    }
    //Disabling calls to Game Analytics functions-VMG
    // #region Game Analytics functions
    // public void GameAnalyticsATTListenerNotDetermined() {
    //     ATTEnabled = false;
    //     GameAnalytics.Initialize();
    //     InitAfterGameAnalytics();
    // }
    // public void GameAnalyticsATTListenerRestricted() {
    //     ATTEnabled = false;
    //     GameAnalytics.Initialize();
    //     InitAfterGameAnalytics();
    // }
    // public void GameAnalyticsATTListenerDenied() {
    //     ATTEnabled = false;
    //     GameAnalytics.Initialize();
    //     InitAfterGameAnalytics();
    // }
    // public void GameAnalyticsATTListenerAuthorized() {
    //     GameAnalytics.Initialize();
    //     InitAfterGameAnalytics();
    // }
    // #endregion

    #region Invoke functions
    // Continue to main menu
    private void ContinueToMainMenu() {
        CancelInvoke(nameof(ContinueToMainMenu));
        //I replaced the Scene name in this code block with "Level004" so it directly jumps to the fifth level instead of opening the menu-VMG
        if (SceneManager.GetActiveScene().name == "Init") {
            SceneManager.LoadScene("Level004");
            GameController.instance.LoadLevel(GameData.level);
        }
    }
    #endregion

    #region Helper functions
    // When script is disabled or inactive
    private void OnDisable() {
        // Cancel all invokes
        CancelInvoke();
    }
    #endregion
}
